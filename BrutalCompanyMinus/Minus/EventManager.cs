using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalCompanyMinus.Minus
{
    public class EventManager
    {

        internal static List<MEvent> events = new List<MEvent>() {
            // Very Good
            new Events.BigBonus(),
            new Events.ScrapGalore(),
            new Events.GoldenBars(),
            new Events.BigDelivery(),
            new Events.PlentyOutsideScrap(),
            new Events.BlackFriday(),
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
            new Events.DDay(),
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
            // NoEnemy
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
            new Events.NoLandmines()
        };
        internal static List<MEvent> disabledEvents = new List<MEvent>();

        internal static List<MEvent> currentEvents = new List<MEvent>();

        internal static float eventTypeSum = 0;
        internal static float[] eventTypeCount = new float[] { };

        public static void AddEvents(params MEvent[] _event) => events.AddRange(_event);

        internal static MEvent RandomWeightedEvent(List<MEvent> _events)
        {
            int WeightedSum = 0;
            foreach (MEvent e in _events) WeightedSum += e.Weight;

            foreach (MEvent e in _events)
            {
                if (UnityEngine.Random.Range(0, WeightedSum) < e.Weight)
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
            Random rng = new Random(StartOfRound.Instance.randomMapSeed + 32345);
            int eventsToSpawn = Configuration.eventsToSpawn.Value + RoundManager.Instance.GetRandomWeightedIndex(Configuration.weightsForExtraEvents.IntArray(), rng);
                
            // Spawn events
            for (int i = 0; i < eventsToSpawn; i++)
            {
                MEvent newEvent = RandomWeightedEvent(eventsToChooseForm);

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
    }
}
