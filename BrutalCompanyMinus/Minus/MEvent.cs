using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BrutalCompanyMinus.Minus
{
    public class MEvent
    {
        public string Description = "";
        public string ColorHex = "#FFFFFF";
        public int Weight = 1;
        public EventType Type = EventType.Neutral;
        public bool Enabled = true;

        public Dictionary<ScaleType, Scale> ScaleList = new Dictionary<ScaleType, Scale>();
        public List<string> EventsToRemove = new List<string>();
        public List<string> EventsToSpawnWith = new List<string>();

        public bool Executed = false;

        public enum EventType
        {
            VeryBad, Bad, Neutral, Good, VeryGood, Remove
        }

        public enum ScaleType
        {
            InsideEnemyRarity, OutsideEnemyRarity, DaytimeEnemyRarity, MinOutsideEnemy, MinInsideEnemy, MaxOutsideEnemy, MaxInsideEnemy,
            ScrapValue, ScrapAmount, FactorySize, MinDensity, MaxDensity, MinCash, MaxCash, MinItemAmount, MaxItemAmount, MinValue, MaxValue, Rarity, MinRarity, MaxRarity,
            MinCut, MaxCut, MinHp, MaxHp, SpawnMultiplier, MaxInsideEnemyCount, MaxOutsideEnemyCount, SpawnCapMultiplier, MinPercentageCut, MaxPercentageCut
        }

        public static Dictionary<ScaleType, string> ScaleInfoList = new Dictionary<ScaleType, string>() {
            { ScaleType.InsideEnemyRarity, "Enemy is added to Inside enemy list with rarity." }, { ScaleType.OutsideEnemyRarity, "Enemy is added to Outside enemy list with rarity." }, { ScaleType.DaytimeEnemyRarity, "Enemy is added to Daytime enemy list with rarity." },
            { ScaleType.MinOutsideEnemy, "Minimum amount of enemies garunteed to spawn outside." }, { ScaleType.MaxOutsideEnemy, "Maximum amount of enemies garunteed to spawn outside." }, { ScaleType.MinInsideEnemy, "Minimum amount of enemies garunteed to spawn inside." }, { ScaleType.MaxInsideEnemy, "Maximum amount of enemies garunteed to spawn inside." },
            { ScaleType.ScrapValue, "The amount that scrap value is multiplied by." }, { ScaleType.ScrapAmount, "The amount that scrap amount is multiplied by." }, { ScaleType.FactorySize, "The amount that factory size is multiplied by." },
            { ScaleType.MinDensity, "Minimum density value chosen." }, { ScaleType.MaxDensity, "Maximum density value chosen." }, { ScaleType.MinCash, "Minumum amount of cash given." }, { ScaleType.MaxCash, "Maximum amount of cash given." },
            { ScaleType.MinItemAmount, "Minimum amount of items to spawn." }, { ScaleType.MaxItemAmount, "Maximum amount of items to spawn." }, { ScaleType.MinValue, "The minimum value of something." }, { ScaleType.MaxValue, "The maximum value of something." },
            { ScaleType.Rarity, "The general chance of something." }, { ScaleType.MinRarity, "Minimum chance of something." }, { ScaleType.MaxRarity, "Maximum chance of something." }, { ScaleType.MinCut, "Minimum cut taken." }, { ScaleType.MaxCut, "Maximum cut taken." },
            { ScaleType.MinHp, "Minimum possible to be chosen." }, { ScaleType.MaxHp, "Maxmimum possible hp to be chosen." }, { ScaleType.SpawnMultiplier, "Will multiply the spawn chance." }, { ScaleType.SpawnCapMultiplier, "Will multiply the spawn cap." },
            { ScaleType.MaxInsideEnemyCount, "Changes max amount of inside enemies spawnable. " }, { ScaleType.MaxOutsideEnemyCount, "Changes max amount of outside enemies spawnable. " }, { ScaleType.MinPercentageCut, "Minimum possible percentage cut." }, { ScaleType.MaxPercentageCut, "Maximum possible percentage cut." }
        };


        public struct Scale
        {
            public float Base, Increment, MinCap, MaxCap;

            public Scale(float Base, float Increment, float MinCap, float MaxCap)
            {
                this.Base = Base;
                this.Increment = Increment;
                this.MinCap = MinCap;
                this.MaxCap = MaxCap;
            }

            public static float Compute(Scale scale, EventType Type = EventType.Neutral)
            {
                float increment = scale.Increment;

                if (Type == MEvent.EventType.VeryBad || Type == MEvent.EventType.Bad) increment = scale.Increment * Configuration.badEventIncrementMultiplier.Value;
                if (Type == MEvent.EventType.VeryGood || Type == MEvent.EventType.Good) increment = scale.Increment * Configuration.goodEventIncrementMultiplier.Value;

                float computedValue = scale.Base + (increment * Manager.daysPassed);

                if (!Configuration.ignoreScaleCap.Value) computedValue = Mathf.Clamp(computedValue, scale.MinCap, scale.MaxCap);
                return computedValue;
            }
        }

        public virtual string Name() => "";
        public virtual void Initalize() { }
        public virtual bool AddEventIfOnly() { return true; }
        public virtual void Execute() { }
        public virtual void OnShipLeave() { }
        public virtual void OnGameStart() { }
        public virtual float Getf(ScaleType EventType)
        {
            try
            {
                return Scale.Compute(ScaleList[EventType], Type);
            } catch
            {
                Log.LogError(string.Format("Scalar '{0}' for '{1}' not found, returning 0.", EventType.ToString(), Name()));
            }
            return 0.0f;
        }

        public int Get(ScaleType EventType)
        {
            return (int)Getf(EventType);
        }

        internal static MEvent GetEvent(string name)
        {
            int index = EventManager.events.FindIndex(x => x.Name() == name);
            if (index != -1) return EventManager.events[index];

            Log.LogError(string.Format("Event '{0}' dosen't exist, returning nothing event", name));
            return new Events.Nothing();
        }
    }
}
