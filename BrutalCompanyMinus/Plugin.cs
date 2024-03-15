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
using Discord;
using System.Diagnostics;
using BepInEx.Configuration;
using System.Globalization;

namespace BrutalCompanyMinus
{
    [HarmonyPatch]
    [BepInPlugin(GUID, NAME, VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        private const string GUID = "Drinkable.BrutalCompanyMinus";
        private const string NAME = "BrutalCompanyMinus";
        private const string VERSION = "0.10.3";
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
            harmony.PatchAll(typeof(GrabObjectTranspiler));

            Log.LogInfo(NAME + " " + VERSION + " " + "is done patching.");
        }

        private static bool Initalized = false;
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Terminal), "Awake")]
        private static void OnTerminalAwake()
        {
            if (Initalized) return;

            Configuration.uiConfig = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus\\UI_Settings.cfg", true);
            Configuration.difficultyConfig = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus\\Difficulty_Settings.cfg", true);
            Configuration.eventConfig = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus\\Events.cfg", true);
            Configuration.weatherConfig = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus\\Weather_Settings.cfg", true);
            Configuration.customAssetsConfig = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus\\Enemy_Scrap_Weights_Settings.cfg", true);

            // Custom enemy events
            int customEventCount = Configuration.eventConfig.Bind("_Temp Custom Monster Event Count", "How many events to generate in config?", 1, "This is temporary for the time being.").Value;
            for (int i = 0; i < customEventCount; i++)
            {
                MEvent e = new Minus.Events.CustomMonsterEvent();
                e.Enabled = false;
                EventManager.AddEvents(e);
            }

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

            EventManager.UpdateEventTypeCounts();
            if (!Configuration.useCustomWeights.Value) EventManager.UpdateAllEventWeights();

            Log.LogInfo(
                $"\n\nTotal Events:{EventManager.events.Count},   Disabled Events:{EventManager.disabledEvents.Count},   Total Events - Remove Count:{EventManager.events.Count - EventManager.eventTypeCount[5]}\n" +
                $"Very Bad:{EventManager.eventTypeCount[0]}\n" +
                $"Bad:{EventManager.eventTypeCount[1]}\n" +
                $"Neutral:{EventManager.eventTypeCount[2]}\n" +
                $"Good:{EventManager.eventTypeCount[3]}\n" +
                $"Very Good:{EventManager.eventTypeCount[4]}\n" +
                $"Remove:{EventManager.eventTypeCount[5]}\n");

            Initalized = true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.LoadNewLevel))]
        private static void ModifyLevel(ref SelectableLevel newLevel)
        {
            Manager.currentLevel = newLevel;
            Manager.currentTerminal = FindObjectOfType<Terminal>();
            Manager.daysPassed = StartOfRound.Instance.gameStats.daysSpent;

            Configuration.GenerateLevelConfigurations(StartOfRound.Instance); // Bind 
            LevelModifications.ModifyEnemyScrapSpawns(StartOfRound.Instance); // Set
            
            Assets.generateLevelScrapLists();
            Net.Instance.ClearGameObjectsClientRpc(); // Clear all previously placed objects on all clients
            if (!RoundManager.Instance.IsHost || newLevel.levelID == 3) return;

            LevelModifications.ResetValues(StartOfRound.Instance);

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

            // Difficulty modifications
            Manager.AddEnemyHp((int)MEvent.Scale.Compute(Configuration.enemyBonusHpScaling));
            Manager.MultiplySpawnChance(newLevel, MEvent.Scale.Compute(Configuration.spawnChanceMultiplierScaling));
            Manager.MultiplySpawnCap(MEvent.Scale.Compute(Configuration.spawnCapMultiplier));
            Manager.AddInsidePower((int)MEvent.Scale.Compute(Configuration.insideEnemyMaxPowerCountScaling));
            Manager.AddOutsidePower((int)MEvent.Scale.Compute(Configuration.outsideEnemyPowerCountScaling));

            // Choose any apply events
            if (!Configuration.useCustomWeights.Value) EventManager.UpdateAllEventWeights();

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

            // Apply maxPower counts
            RoundManager.Instance.currentLevel.maxEnemyPowerCount = (int)((RoundManager.Instance.currentLevel.maxEnemyPowerCount + Manager.bonusMaxInsidePowerCount) * Manager.spawncapMultipler);
            RoundManager.Instance.currentLevel.maxOutsideEnemyPowerCount = (int)((RoundManager.Instance.currentLevel.maxOutsideEnemyPowerCount + Manager.bonusMaxOutsidePowerCount) * Manager.spawncapMultipler);

            // Sync values to all clients
            Net.Instance.SyncValuesClientRpc(Manager.currentLevel.factorySizeMultiplier, Manager.scrapValueMultiplier, Manager.scrapAmountMultiplier, Manager.bonusEnemyHp);
            
            // Apply UI
            UI.GenerateText(currentEvents);

            // Logging
            Log.LogInfo("MapMultipliers = [scrapValueMultiplier: " + Manager.scrapValueMultiplier + ",     scrapAmountMultiplier: " + Manager.scrapAmountMultiplier + ",     currentLevel.factorySizeMultiplier:" + Manager.currentLevel.factorySizeMultiplier + "]");
        }
    }
}
