using System;
using HarmonyLib;
using BepInEx;
using UnityEngine;
using BepInEx.Logging;
using HarmonyLib.Tools;
using System.Reflection;
using BrutalCompanyMinus.Minus;
using System.Collections.Generic;
using TMPro;
using Events = BrutalCompanyMinus.Minus.Events;
using System.Collections;
using BrutalCompanyMinus.Minus.Handlers;

namespace BrutalCompanyMinus
{
    [HarmonyPatch]
    [BepInPlugin(GUID, NAME, VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        private const string GUID = "Drinkable.BrutalCompanyMinus";
        private const string NAME = "BrutalCompanyMinus";
        private const string VERSION = "0.7.0";
        private readonly Harmony harmony = new Harmony(GUID);

        public static List<MEvent> events = new List<MEvent>() {
            // Very Good
            new Events.BigBonus(),
            new Events.ScrapGalore(),
            new Events.GoldenBars(),
            new Events.BigDelivery(),
            new Events.PlentyOutsideScrap(),
            // Good
            new Events.Bounty(),
            new Events.Bonus(),
            new Events.SmallerMap(),
            new Events.MoreScrap(),
            new Events.HigherScrapValue(),
            new Events.GoldenFacility(),
            new Events.Dentures(),
            new Events.Pickles(),
            new Events.Honk(),
            new Events.TransmuteScrapSmall(),
            new Events.SmallDeilvery(),
            new Events.ScarceOutsideScrap(),
            // Neutral
            new Events.Nothing(),
            new Events.Locusts(),
            new Events.Birds(),
            new Events.Trees(),
            new Events.LeaflessBrownTrees(),
            new Events.LeaflessTrees(),
            // Bad
            new Events.HoardingBugs(),
            new Events.Bees(),
            new Events.Landmines(),
            new Events.Lizard(),
            new Events.Slimes(),
            new Events.Thumpers(),
            new Events.Turrets(),
            new Events.Spiders(),
            new Events.SnareFleas(),
            new Events.DoorGlitch(),
            new Events.OutsideTurrets(),
            new Events.OutsideLandmines(),
            new Events.CursedGold(),
            // Very Bad
            new Events.Nutcracker(),
            new Events.Arachnophobia(),
            new Events.Bracken(),
            new Events.Coilhead(),
            new Events.BaboonHorde(),
            new Events.Dogs(),
            new Events.Jester(),
            new Events.LittleGirl(),
            new Events.AntiCoilhead(),
            new Events.ChineseProduce(),
            new Events.TransmuteScrapBig(),
            new Events.Warzone(),
            new Events.GypsyColony(),
            new Events.ForestGiant(),
            new Events.InsideBees(),
            // NoEnemy
            new Events.NoBaboons(),
            new Events.NoBracken(),
            new Events.NoCoilhead(),
            new Events.NoDogs(),
            new Events.NoGiants(),
            new Events.NoHoardingBugs(),
            new Events.NoJester(),
            new Events.NoLittleGirl(),
            new Events.NoLizards(),
            new Events.NoNutcracker(),
            new Events.NoSpiders(),
            new Events.NoThumpers(),
            new Events.NoSnareFleas(),
            new Events.NoWorm(),
            new Events.NoSlimes(),
            new Events.NoMasks(),
            new Events.NoTurrets(),
            new Events.NoLandmines()
        };
        public static List<MEvent> disabledEvents = new List<MEvent>();

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
                foreach (MEvent e in events) e.Initalize();

                // Config
                Configuration.Initalize();
                Manager.UpdateAllEventWeights();

                // Use config settings
                for (int i = 0; i != events.Count; i++)
                {
                    if (Configuration.useCustomWeights.Value) events[i].Weight = Configuration.eventWeights[i].Value;
                    events[i].Description = Configuration.eventDescriptions[i].Value;
                    events[i].ColorHex = Configuration.eventColorHexes[i].Value;
                    events[i].Type = Configuration.eventTypes[i].Value;
                    events[i].ScaleList = Configuration.eventScales[i];
                    events[i].Enabled = Configuration.eventEnables[i].Value;
                }

                // Create disabled events list and update
                List<MEvent> newEvents = new List<MEvent>();
                foreach (MEvent e in events)
                {
                    if (!e.Enabled)
                    {
                        disabledEvents.Add(e);
                    }
                    else
                    {
                        newEvents.Add(e);
                    }
                }
                events = newEvents;

                Initalized = true;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.LoadNewLevel))]
        private static void ModifyLevel(ref SelectableLevel newLevel)
        {
            Manager.daysPassed++;

            Net.Instance.ClearGameObjectsClientRpc(); // Clear all previously placed objects on all clients
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
                    Manager.factorySizeMultiplier *= e.factorySizeMultiplier;
                }
            }

            // Update Enemy max power counts
            RoundManager.Instance.currentMaxInsidePower += Math.Min(Manager.daysPassed, 10) + Manager.daysPassed;
            RoundManager.Instance.currentMaxOutsidePower += Manager.daysPassed / 2;

            // Apply events
            List<MEvent> additionalEvents = new List<MEvent>();
            List<MEvent> currentEvents = Manager.ChooseEvents(newLevel, events, out additionalEvents);
            Manager.ApplyEvents(currentEvents);
            Manager.ApplyEvents(additionalEvents);

            if(Configuration.showEventsInChat.Value)
            {
                HUDManager.Instance.AddTextToChatOnServer("<color=#FFFFFF>Events:</color>");
                foreach(MEvent e in currentEvents)
                {
                    HUDManager.Instance.AddTextToChatOnServer(string.Format("<color={0}>{1}</color>", e.ColorHex, e.Description));
                }
            }

            // Log Chosen events
            foreach (MEvent e in currentEvents) Log.LogInfo("Event chosen: " + e.Name());

            // Spawn outside scrap
            Manager.Spawn.DoSpawnScrapOutside(Manager.randomItemsToSpawnOutsideCount);

            // Sync values to all clients
            Net.Instance.SetMultipliersClientRpc(Manager.factorySizeMultiplier, Manager.scrapValueMultiplier, Manager.scrapAmountMultiplier);

            // Apply UI
            UI.GenerateText(currentEvents);

            // Logging
            Log.LogInfo("MapMultipliers = [scrapValueMultiplier: " + Manager.scrapValueMultiplier + ",     scrapAmountMultiplier: " + Manager.scrapAmountMultiplier + ",     factorySizeMultiplier:" + Manager.factorySizeMultiplier + "]");
            Log.LogInfo("IsAntiCoildHead = " + Net.Instance.isAntiCoilHead.Value);
        }

        private static void ResetValues(SelectableLevel newLevel)
        {
            foreach (SpawnableMapObject obj in newLevel.spawnableMapObjects)
            {
                if (obj.prefabToSpawn.name == "Landmine") obj.numberToSpawn = new AnimationCurve(new Keyframe(0, 2.5f));
                if (obj.prefabToSpawn.name == "TurretContainer") obj.numberToSpawn = new AnimationCurve(new Keyframe(0, 2.5f));
            }
            Manager.BountyActive = false; Manager.DoorGlitchActive = false; Net.Instance.isAntiCoilHead.Value = false;

            // Reset Multipliers
            try
            {
                Manager.factorySizeMultiplier = Assets.factorySizeMultiplierList[newLevel.levelID];
            } catch
            {
                Manager.factorySizeMultiplier = 1f;
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
