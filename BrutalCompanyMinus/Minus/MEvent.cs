using System.Collections.Generic;
using TMPro;

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

        public enum EventType
        {
            VeryBad, Bad, Neutral, Good, VeryGood, Remove
        }

        public enum ScaleType
        {
            EnemyRarity, MinOutsideEnemy, MinInsideEnemy, MaxOutsideEnemy, MaxInsideEnemy, ScrapValue, ScrapAmount, FactorySize, MinDensity, MaxDensity, MinCash, MaxCash, MinItemAmount, MaxItemAmount, MinValue, MaxValue, Other
        }

        public struct Scale
        {
            public float Base, Increment;

            public Scale(float Base, float Increment)
            {
                this.Base = Base;
                this.Increment = Increment;
            }
        }

        public virtual string Name() => "";
        public virtual void Initalize() { }
        public virtual bool AddEventIfOnly() { return true; }
        public virtual void Execute() { }
        public virtual float Getf(ScaleType EventType)
        {
            try
            {
                Scale scale = ScaleList[EventType];
                float increment = scale.Increment;

                if (Type == MEvent.EventType.VeryBad || Type == MEvent.EventType.Bad) increment = scale.Increment * Configuration.badEventIncrementMultiplier.Value;
                if (Type == MEvent.EventType.VeryGood || Type == MEvent.EventType.Good) increment = scale.Increment * Configuration.goodEventIncrementMultiplier.Value;

                return scale.Base + (increment * Manager.daysPassed);
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
