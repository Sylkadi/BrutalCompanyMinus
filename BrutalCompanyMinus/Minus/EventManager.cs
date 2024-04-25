using BrutalCompanyMinus.Minus.Handlers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace BrutalCompanyMinus.Minus
{
    [HarmonyPatch]
    public class EventManager
    {
        internal static List<MEvent> vanillaEvents = new List<MEvent>() {
            // Very Good
            new Events.BigBonus(),
            new Events.ScrapGalore(),
            new Events.GoldenBars(),
            new Events.BigDelivery(),
            new Events.PlentyOutsideScrap(),
            new Events.BlackFriday(),
            new Events.SafeOutside(),
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
            new Events.FragileEnemies(),
            new Events.FullAccess(),
            new Events.EarlyShip(),
            new Events.MoreExits(),
            // Neutral
            new Events.Nothing(),
            new Events.Locusts(),
            new Events.Birds(),
            new Events.Trees(),
            new Events.LeaflessBrownTrees(),
            new Events.LeaflessTrees(),
            new Events.Raining(),
            new Events.Gloomy(),
            new Events.HeavyRain(),
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
            new Events.FacilityGhost(),
            new Events.OutsideTurrets(),
            new Events.OutsideLandmines(),
            new Events.ShipmentFees(),
            new Events.GrabbableLandmines(),
            new Events.GrabbableTurrets(),
            new Events.StrongEnemies(),
            new Events.KamikazieBugs(),
            new Events.RealityShift(),
            new Events.Masked(),
            new Events.Butlers(),
            new Events.SpikeTraps(),
            new Events.FlowerSnake(),
            new Events.LateShip(),
            new Events.HolidaySeason(),
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
            new Events.BugHorde(),
            new Events.ForestGiant(),
            new Events.InsideBees(),
            new Events.NutSlayer(),
            new Events.Hell(),
            new Events.AllWeather(),
            new Events.Worms(),
            new Events.OldBirds(),
            // No Enemy
            new Events.NoBaboons(),
            new Events.NoBracken(),
            new Events.NoCoilhead(),
            new Events.NoDogs(),
            new Events.NoGiants(),
            new Events.NoHoardingBugs(),
            new Events.NoJester(),
            new Events.NoGhosts(),
            new Events.NoLizards(),
            new Events.NoNutcracker(),
            new Events.NoSpiders(),
            new Events.NoThumpers(),
            new Events.NoSnareFleas(),
            new Events.NoWorm(),
            new Events.NoSlimes(),
            new Events.NoMasks(),
            new Events.NoTurrets(),
            new Events.NoLandmines(),
            new Events.NoOldBird(),
            new Events.NoButlers(),
            new Events.NoSpikeTraps()
        };

        internal static List<MEvent> moddedEvents = new List<MEvent>();

        internal static List<MEvent> customEvents = new List<MEvent>();

        internal static List<MEvent> events = new List<MEvent>();

        internal static List<MEvent> disabledEvents = new List<MEvent>();

        internal static List<MEvent> currentEvents = new List<MEvent>();

        internal static List<MEvent> forcedEvents = new List<MEvent>();

        internal static List<MEvent> allVeryGood = new List<MEvent>(), allGood = new List<MEvent>(), allNeutral = new List<MEvent>(), allBad = new List<MEvent>(), allVeryBad = new List<MEvent>(), allRemove = new List<MEvent>();

        internal static List<string> currentEventDescriptions = new List<string>();

        internal static float eventTypeSum = 0;
        internal static float[] eventTypeCount = new float[] { };

        internal static float[] eventTypeRarities = new float[] { };

        public static void AddEvents(params MEvent[] _event) => events.AddRange(_event);

        internal static MEvent RandomWeightedEvent(List<MEvent> _events, System.Random rng)
        {
            if (_events.Count == 0) return new Events.Nothing();

            int WeightedSum = 0;
            foreach (MEvent e in _events) WeightedSum += e.Weight;

            foreach (MEvent e in _events)
            {
                if (rng.Next(0, WeightedSum) < e.Weight)
                {
                    return e;
                }
                WeightedSum -= e.Weight;
            }

            return _events[_events.Count - 1];
        }

        internal static List<MEvent> ChooseEvents(out List<MEvent> additionalEvents)
        {
            currentEvents.Clear();

            List<MEvent> chosenEvents = new List<MEvent>();
            List<MEvent> eventsToChooseForm = new List<MEvent>();
            foreach (MEvent e in events) eventsToChooseForm.Add(e);

            // Decide how many events to spawn
            System.Random rng = new System.Random(StartOfRound.Instance.randomMapSeed + 32345 + Environment.TickCount);
            int eventsToSpawn = (int)MEvent.Scale.Compute(Configuration.eventsToSpawn, MEvent.EventType.Neutral) + RoundManager.Instance.GetRandomWeightedIndex(Configuration.weightsForExtraEvents.IntArray(), rng);
                
            // Spawn events
            for (int i = 0; i < eventsToSpawn; i++)
            {
                MEvent newEvent = RandomWeightedEvent(eventsToChooseForm, rng);

                bool foundForcedEvent = false;
                foreach (MEvent forcedEvent in forcedEvents)
                {
                    if (forcedEvent.Name() == newEvent.Name())
                    {
                        eventsToChooseForm.RemoveAll(x => x.Name() == forcedEvent.Name());
                        foundForcedEvent = true;
                        break;
                    }
                }
                if(foundForcedEvent)
                {
                    i--;
                    continue;
                }

                if (!newEvent.AddEventIfOnly()) // If event condition is false, remove event from eventsToChoosefrom and iterate again
                {
                    i--;
                    eventsToChooseForm.RemoveAll(x => x.Name() == newEvent.Name());
                    continue;
                }

                chosenEvents.Add(newEvent);

                // Remove so no further accurrences
                eventsToChooseForm.RemoveAll(x => x.Name() == newEvent.Name());

                // Remove incompatible events from toChooseList
                int AmountRemoved = 0;

                foreach (string eventToRemove in newEvent.EventsToRemove)
                {
                    eventsToChooseForm.RemoveAll(x => x.Name() == eventToRemove);
                    AmountRemoved += chosenEvents.RemoveAll(x => x.Name() == eventToRemove);
                }

                foreach (string eventToSpawnWith in newEvent.EventsToSpawnWith)
                {
                    eventsToChooseForm.RemoveAll(x => x.Name() == eventToSpawnWith);
                    AmountRemoved += chosenEvents.RemoveAll(x => x.Name() == eventToSpawnWith);
                }

                i -= AmountRemoved; // Decrement each time an event is removed from chosenEvents list
            }

            // Generate eventsToSpawnWith list with no copies
            List<MEvent> eventsToSpawnWith = new List<MEvent>();
            for (int i = 0; i < chosenEvents.Count; i++)
            {
                foreach (string eventToSpawnWith in chosenEvents[i].EventsToSpawnWith)
                {
                    int index = eventsToSpawnWith.FindIndex(x => x.Name() == eventToSpawnWith);
                    if (index == -1) eventsToSpawnWith.Add(MEvent.GetEvent(eventToSpawnWith)); // If dosen't exist in list, add.
                }
            }

            // Remove disabledEvents from EventsToSpawnWith List
            foreach (MEvent e in disabledEvents)
            {
                int index = eventsToSpawnWith.FindIndex(x => x.Name() == e.Name());
                if (index != -1) eventsToSpawnWith.RemoveAt(index);
            }

            additionalEvents = eventsToSpawnWith;
            currentEvents = chosenEvents;
            return chosenEvents;
        }

        internal static void ApplyEvents(List<MEvent> currentEvents)
        {
            foreach (MEvent e in currentEvents)
            {
                if(!e.Executed)
                {
                    e.Executed = true;
                    e.Execute();
                }
            }
        }

        internal static void UpdateAllEventWeights()
        {
            float fix(float value) // This is to avoid division by zero
            {
                if (value < 1) return 1;
                return value;
            }

            int eventTypeAmount = Configuration.eventTypeScales.Length;

            float[] computedScales = new float[eventTypeAmount];
            for (int i = 0; i < eventTypeAmount; i++) computedScales[i] = MEvent.Scale.Compute(Configuration.eventTypeScales[i]);

            float eventTypeWeightSum = 0;
            for (int i = 0; i < eventTypeAmount; i++) eventTypeWeightSum += computedScales[i];
            eventTypeWeightSum = fix(eventTypeWeightSum);

            float[] eventTypeProbabilities = new float[eventTypeAmount];
            for(int i = 0; i < eventTypeAmount; i++) eventTypeProbabilities[i] = computedScales[i] / eventTypeWeightSum;
            eventTypeRarities = eventTypeProbabilities;

            int[] newEventWeights = new int[eventTypeAmount];
            for (int i = 0; i < eventTypeAmount; i++)
            {
                newEventWeights[i] = (int)((eventTypeSum / fix(eventTypeCount[i])) * eventTypeProbabilities[i] * 1000.0f);
                Log.LogInfo($"Set eventType weight for {((MEvent.EventType)Enum.ToObject(typeof(MEvent.EventType), i)).ToString()} to {newEventWeights[i]}");
            }


            foreach(MEvent e in events) e.Weight = newEventWeights[(int)e.Type];
        }

        internal static void UpdateEventTypeCounts()
        {
            int eventTypeAmount = Configuration.eventTypeScales.Length;

            eventTypeCount = new float[eventTypeAmount];
            for (int i = 0; i < eventTypeAmount; i++) eventTypeCount[i] = 0.0f;
            foreach (MEvent e in events) eventTypeCount[(int)e.Type]++;

            eventTypeSum = 0.0f;
            for (int i = 0; i < eventTypeAmount; i++) eventTypeSum += eventTypeCount[i];
        }

        internal static void UpdateEventDescriptions(List<MEvent> events)
        {
            currentEventDescriptions.Clear();
            foreach(MEvent e in events)
            {
                currentEventDescriptions.Add($"<color={e.ColorHex}>{e.Descriptions[UnityEngine.Random.Range(0, e.Descriptions.Count)]}</color>");
            }
        }

        public struct DifficultyTransition : IComparable<DifficultyTransition>
        {
            public string name;
            public string description;
            public float above;

            public DifficultyTransition(string name, string description, float above)
            {
                this.name = name;
                this.description = description;
                this.above = above;
            }

            public int CompareTo(DifficultyTransition other)
            {
                return above.CompareTo(other.above);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.LoadNewLevel))]
        private static void ModifyLevel(ref SelectableLevel newLevel)
        {
            Log.LogInfo("Executing OnShipLeave() for current events.");
            foreach (MEvent e in EventManager.currentEvents)
            {
                e.OnShipLeave();
            }

            UI.canClearText = false;
            Manager.ComputeDifficultyValues();

            Manager.currentLevel = newLevel;
            Manager.currentTerminal = GameObject.FindObjectOfType<Terminal>();

            Assets.generateOriginalValuesLists();
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
                }
            }

            // Difficulty modifications
            Manager.AddEnemyHp((int)MEvent.Scale.Compute(Configuration.enemyBonusHpScaling));
            Manager.AddInsideSpawnChance(newLevel, MEvent.Scale.Compute(Configuration.insideSpawnChanceAdditive));
            Manager.AddOutsideSpawnChance(newLevel, MEvent.Scale.Compute(Configuration.outsideSpawnChanceAdditive));
            Manager.MultiplySpawnChance(newLevel, MEvent.Scale.Compute(Configuration.spawnChanceMultiplierScaling));
            Manager.MultiplySpawnCap(MEvent.Scale.Compute(Configuration.spawnCapMultiplier));
            Manager.AddInsidePower((int)MEvent.Scale.Compute(Configuration.insideEnemyMaxPowerCountScaling));
            Manager.AddOutsidePower((int)MEvent.Scale.Compute(Configuration.outsideEnemyPowerCountScaling));
            Manager.scrapValueMultiplier *= MEvent.Scale.Compute(Configuration.scrapValueMultiplier);
            Manager.scrapAmountMultiplier *= MEvent.Scale.Compute(Configuration.scrapAmountMultiplier);

            // Choose any apply events
            if (!Configuration.useCustomWeights.Value) UpdateAllEventWeights();

            List<MEvent> additionalEvents = new List<MEvent>();
            List<MEvent> currentEvents = ChooseEvents(out additionalEvents);

            foreach (MEvent e in currentEvents) Log.LogInfo("Event chosen: " + e.Name()); // Log Chosen events
            foreach (MEvent e in additionalEvents) Log.LogInfo("Additional events: " + e.Name());

            ApplyEvents(currentEvents);
            ApplyEvents(additionalEvents);

            foreach(MEvent forcedEvent in forcedEvents)
            {
                forcedEvent.Execute();

                foreach(string additionalEvent in forcedEvent.EventsToSpawnWith)
                {
                    MEvent.GetEvent(additionalEvent).Execute();
                }
            }

            currentEvents.AddRange(forcedEvents);
            forcedEvents.Clear();

            UpdateEventDescriptions(currentEvents);

            if (Configuration.showEventsInChat.Value && !Configuration.DisplayUIAfterShipLeaves.Value)
            {
                HUDManager.Instance.AddTextToChatOnServer("<color=#FFFFFF>Events:</color>");
                foreach(string eventDescription in currentEventDescriptions)
                {
                    HUDManager.Instance.AddTextToChatOnServer(eventDescription);
                }
            }

            // Apply maxPower counts
            RoundManager.Instance.currentLevel.maxEnemyPowerCount = (int)((RoundManager.Instance.currentLevel.maxEnemyPowerCount + Manager.bonusMaxInsidePowerCount) * Manager.spawncapMultipler);
            RoundManager.Instance.currentLevel.maxOutsideEnemyPowerCount = (int)((RoundManager.Instance.currentLevel.maxOutsideEnemyPowerCount + Manager.bonusMaxOutsidePowerCount) * Manager.spawncapMultipler);

            // Sync values to all clients
            Net.Instance.SyncValuesClientRpc(Manager.currentLevel.factorySizeMultiplier, Manager.scrapValueMultiplier, Manager.scrapAmountMultiplier, Manager.bonusEnemyHp);

            // Apply UI
            if (!Configuration.DisplayUIAfterShipLeaves.Value)
            {
                UI.GenerateText(currentEvents);
            }
            else
            {
                UI.ClearText();
            }

            // Logging
            Log.LogInfo("MapMultipliers = [scrapValueMultiplier: " + Manager.scrapValueMultiplier + ",     scrapAmountMultiplier: " + Manager.scrapAmountMultiplier + ",     currentLevel.factorySizeMultiplier:" + Manager.currentLevel.factorySizeMultiplier + "]");
            Log.LogInfo("Inside Spawn Curve");
            foreach (Keyframe key in newLevel.enemySpawnChanceThroughoutDay.keys) Log.LogInfo($"Time:{key.time} + $Value:{key.value}");
            Log.LogInfo("Outside Spawn Curve");
            foreach (Keyframe key in newLevel.outsideEnemySpawnChanceThroughDay.keys) Log.LogInfo($"Time:{key.time} + $Value:{key.value}");
            Log.LogInfo("Daytime Spawn Curve");
            foreach (Keyframe key in newLevel.daytimeEnemySpawnChanceThroughDay.keys) Log.LogInfo($"Time:{key.time} + $Value:{key.value}");
        }
    }
}
