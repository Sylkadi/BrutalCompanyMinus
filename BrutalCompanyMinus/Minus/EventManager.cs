using System;
using System.Collections.Generic;
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
            new Events.DoorGlitch(),
            new Events.OutsideTurrets(),
            new Events.OutsideLandmines(),
            new Events.ShipmentFees(),
            new Events.GrabbableLandmines(),
            new Events.GrabbableTurrets(),
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
            new Events.NutSlayer(),
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
        internal static List<MEvent> disabledEvents = new List<MEvent>();

        internal static List<MEvent> currentEvents = new List<MEvent>();

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

            // Remove incompatible events
            for (int i = 0; i < eventsToSpawnWith.Count; i++)
            {
                foreach (string eventToRemove in eventsToSpawnWith[i].EventsToRemove)
                {
                    eventsToSpawnWith.RemoveAll(x => x.Name() == eventToRemove);
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
            foreach (MEvent e in currentEvents) e.Execute();
        }


        internal static void UpdateAllEventWeights()
        {
            float fix(float value) // This is to avoid division by zero
            {
                if (value < 1) return 1;
                return value;
            }

            float eventTypeWeightSum = fix(Configuration.veryGoodWeight.Value + Configuration.goodWeight.Value + Configuration.neutralWeight.Value + Configuration.badWeight.Value + Configuration.veryBadWeight.Value + Configuration.removeEnemyWeight.Value);

            float veryGoodProbability = Configuration.veryGoodWeight.Value / eventTypeWeightSum;
            float goodProbablity = Configuration.goodWeight.Value / eventTypeWeightSum;
            float neutralProbability = Configuration.neutralWeight.Value / eventTypeWeightSum;
            float removeEnemyProbability = Configuration.removeEnemyWeight.Value / eventTypeWeightSum;
            float badProbability = Configuration.badWeight.Value / eventTypeWeightSum;
            float veryBadProbability = Configuration.veryBadWeight.Value / eventTypeWeightSum;


            // Update all weights on events
            float VeryGoodCount = 0, GoodCount = 0, NeutralCount = 0, RemoveCount = 0, BadCount = 0, VeryBadCount = 0, Sum = 0;
            foreach (MEvent e in events)
            {
                switch (e.Type)
                {
                    case MEvent.EventType.VeryGood:
                        VeryGoodCount++;
                        break;
                    case MEvent.EventType.Good:
                        GoodCount++;
                        break;
                    case MEvent.EventType.Neutral:
                        NeutralCount++;
                        break;
                    case MEvent.EventType.Remove:
                        RemoveCount++;
                        break;
                    case MEvent.EventType.Bad:
                        BadCount++;
                        break;
                    case MEvent.EventType.VeryBad:
                        VeryBadCount++;
                        break;
                }
            }
            Sum = VeryBadCount + GoodCount + NeutralCount + RemoveCount + BadCount + VeryBadCount;

            float VeryGoodWeight = (Sum / fix(VeryGoodCount)) * veryGoodProbability, GoodWeight = (Sum / fix(GoodCount)) * goodProbablity, NeutralWeight = (Sum / fix(NeutralCount)) * neutralProbability,
                  RemoveWeight = (Sum / fix(RemoveCount)) * removeEnemyProbability, BadWeight = (Sum / fix(BadCount)) * badProbability, VeryBadWeight = (Sum / fix(VeryBadCount)) * veryBadProbability;

            foreach (MEvent e in events)
            {
                switch (e.Type)
                {
                    case MEvent.EventType.VeryGood:
                        e.Weight = (int)(VeryGoodWeight * 1000f);
                        if (VeryGoodCount == 0) e.Weight = 0;
                        Log.LogInfo(string.Format("Set weight for {0} to {1}", e.Name(), e.Weight));
                        break;
                    case MEvent.EventType.Good:
                        e.Weight = (int)(GoodWeight * 1000f);
                        if (GoodCount == 0) e.Weight = 0;
                        Log.LogInfo(string.Format("Set weight for {0} to {1}", e.Name(), e.Weight));
                        break;
                    case MEvent.EventType.Neutral:
                        e.Weight = (int)(NeutralWeight * 1000f);
                        if (NeutralCount == 0) e.Weight = 0;
                        Log.LogInfo(string.Format("Set weight for {0} to {1}", e.Name(), e.Weight));
                        break;
                    case MEvent.EventType.Remove:
                        e.Weight = (int)(RemoveWeight * 1000f);
                        if (RemoveCount == 0) e.Weight = 0;
                        Log.LogInfo(string.Format("Set weight for {0} to {1}", e.Name(), e.Weight));
                        break;
                    case MEvent.EventType.Bad:
                        e.Weight = (int)(BadWeight * 1000f);
                        if (BadCount == 0) e.Weight = 0;
                        Log.LogInfo(string.Format("Set weight for {0} to {1}", e.Name(), e.Weight));
                        break;
                    case MEvent.EventType.VeryBad:
                        e.Weight = (int)(VeryBadWeight * 1000f);
                        if (VeryBadCount == 0) e.Weight = 0;
                        Log.LogInfo(string.Format("Set weight for {0} to {1}", e.Name(), e.Weight));
                        break;
                }
                switch (e.Name())
                {
                }
            }
        }

    }
}
