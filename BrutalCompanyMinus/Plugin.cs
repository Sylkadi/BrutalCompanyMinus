using System;
using HarmonyLib;
using BepInEx;
using UnityEngine;
using System.Reflection;
using BrutalCompanyMinus.Minus;
using System.Collections.Generic;
using BrutalCompanyMinus.Minus.Handlers;
using UnityEngine.Diagnostics;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine.InputSystem.HID;

namespace BrutalCompanyMinus
{
    [HarmonyPatch]
    [BepInPlugin(GUID, NAME, VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        private const string GUID = "Drinkable.BrutalCompanyMinus";
        private const string NAME = "BrutalCompanyMinus";
        private const string VERSION = "0.8.2";
        private static readonly Harmony harmony = new Harmony(GUID);

        void Awake()
        {
            // Logger
            Log.Initalize(Logger);

            // Required for netweaving
            var EventTypes = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var EventType in EventTypes)
            {
                var methods = EventType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
            // Load assets
            Assets.Load();

            // Patch all
            harmony.PatchAll();
            harmony.PatchAll(typeof(_LevelParameterRestoring));

            Log.LogInfo(NAME + " " + VERSION + " " + "is done patching.");
        }

        private static bool Initalized = false;
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Terminal), "Awake")]
        private static void OnTerminalAwake()
        {
            if(!Initalized)
            {
                // Initalize Events
                foreach (MEvent e in EventManager.events) e.Initalize();

                Manager.currentTerminal = FindObjectOfType<Terminal>();

                // Config
                Configuration.Initalize();

                // Use config settings
                for (int i = 0; i != EventManager.events.Count; i++)
                {
                    EventManager.events[i].Weight = Configuration.eventWeights[i].Value;
                    EventManager.events[i].Description = Configuration.eventDescriptions[i].Value;
                    EventManager.events[i].ColorHex = Configuration.eventColorHexes[i].Value;
                    EventManager.events[i].Type = Configuration.eventTypes[i].Value;
                    EventManager.events[i].ScaleList = Configuration.eventScales[i];
                    EventManager.events[i].Enabled = Configuration.eventEnables[i].Value;
                }

                // Create disabled events list and update
                List<MEvent> newEvents = new List<MEvent>();
                foreach (MEvent e in EventManager.events)
                {
                    if (!e.Enabled)
                    {
                        EventManager.disabledEvents.Add(e);
                    }
                    else
                    {
                        newEvents.Add(e);
                    }
                }
                EventManager.events = newEvents;

                if (!Configuration.useCustomWeights.Value) EventManager.UpdateAllEventWeights();

                Initalized = true;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.LoadNewLevel))]
        private static void ModifyLevel(ref SelectableLevel newLevel)
        {
            Manager.currentLevel = newLevel;
            Manager.currentTerminal = FindObjectOfType<Terminal>();
            Manager.daysPassed++;

            Configuration.GenerateLevelConfigurations(StartOfRound.Instance); // Bind 
            LevelParameterRestoring.ModifyEnemyScrapSpawns(StartOfRound.Instance); // Set
            Assets.generateLevelScrapLists(); // Store

            Net.Instance.ClearGameObjectsClientRpc(); // Clear all previously placed objects on all clients
            Manager.grabbableLandmines = false; Manager.grabbableTurrets = false;
            if (!RoundManager.Instance.IsHost || newLevel.levelID == 3) return;

            // Reset values
            LevelParameterRestoring.StoreUnmodifiedParamaters(newLevel);
            ResetValues(newLevel);

            // Apply weather multipliers
            foreach (Weather e in Net.Instance.currentWeatherMultipliers)
            {
                if (newLevel.currentWeather == e.weatherType)
                {
                    Manager.scrapValueMultiplier *= e.scrapValueMultiplier;
                    Manager.scrapAmountMultiplier *= e.scrapAmountMultiplier;
                    Manager.currentLevel.factorySizeMultiplier *= e.factorySizeMultiplier;
                }
            }

            // Update Enemy max power counts
            RoundManager.Instance.currentMaxInsidePower += Math.Min(Manager.daysPassed, 10) + Manager.daysPassed;
            RoundManager.Instance.currentMaxOutsidePower += Manager.daysPassed / 2;

            // Choose any apply events
            List<MEvent> additionalEvents = new List<MEvent>();
            List<MEvent> currentEvents = EventManager.ChooseEvents(out additionalEvents);

            foreach (MEvent e in currentEvents) Log.LogInfo("Event chosen: " + e.Name()); // Log Chosen events

            EventManager.ApplyEvents(currentEvents);
            EventManager.ApplyEvents(additionalEvents);

            if (Configuration.showEventsInChat.Value)
            {
                HUDManager.Instance.AddTextToChatOnServer("<color=#FFFFFF>Events:</color>");
                foreach(MEvent e in currentEvents)
                {
                    HUDManager.Instance.AddTextToChatOnServer(string.Format("<color={0}>{1}</color>", e.ColorHex, e.Description));
                }
            }

            Manager.ScrapToSpawn.Clear(); Manager.ScrapToSpawn.AddRange(newLevel.spawnableScrap);

            // Spawn outside scrap
            Manager.Spawn.DoSpawnScrapOutside(Manager.randomItemsToSpawnOutsideCount);

            // Sync values to all clients
            Net.Instance.SyncValuesClientRpc(Manager.currentLevel.factorySizeMultiplier, Manager.scrapValueMultiplier, Manager.scrapAmountMultiplier);

            // Apply UI
            UI.GenerateText(currentEvents);

            // Logging
            Log.LogInfo("MapMultipliers = [scrapValueMultiplier: " + Manager.scrapValueMultiplier + ",     scrapAmountMultiplier: " + Manager.scrapAmountMultiplier + ",     currentLevel.factorySizeMultiplier:" + Manager.currentLevel.factorySizeMultiplier + "]");
        }
        
        private static void ResetValues(SelectableLevel newLevel)
        {
            foreach (SpawnableMapObject obj in newLevel.spawnableMapObjects)
            {
                if (obj.prefabToSpawn.name == "Landmine") obj.numberToSpawn = new AnimationCurve(new Keyframe(0, 2.5f));
                if (obj.prefabToSpawn.name == "TurretContainer") obj.numberToSpawn = new AnimationCurve(new Keyframe(0, 2.5f));
            }
            Manager.BountyActive = false; Manager.DoorGlitchActive = false; Manager.ShipmentFees = false;

            // Reset Multipliers
            try
            {
                Manager.currentLevel.factorySizeMultiplier = Assets.factorySizeMultiplierList[newLevel.levelID];
            } catch
            {
                Manager.currentLevel.factorySizeMultiplier = 1f;
            }
            Manager.scrapAmountMultiplier = 1.0f;
            Manager.scrapValueMultiplier = 0.4f; // Default value is 0.4 not 1.0
            Manager.scrapMinAmount = newLevel.minScrap;
            Manager.scrapMaxAmount = newLevel.maxScrap;
            Manager.randomItemsToSpawnOutsideCount = 0;

            // Reset objectSpawnLists
            Manager.insideObjectsToSpawnOutside.Clear();
        }
    }
}
