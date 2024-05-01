using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BrutalCompanyMinus.Minus
{
    public class MEvent
    {
        /// <summary>
        /// This is the text displayed in the UI.
        /// </summary>
        public List<string> Descriptions = new List<string>() { "" };

        /// <summary>
        /// The color the event will be displayed in the UI in hex.
        /// </summary>
        public string ColorHex = "#FFFFFF";

        /// <summary>
        /// This can be ignored, this value is only used when Use_Custom_Weights in the config is set to true.
        /// </summary>
        public int Weight = 1;

        /// <summary>
        /// Set in what your opinion the severity of this event.
        /// </summary>
        public EventType Type = EventType.Neutral;

        /// <summary>
        /// If true event will appear, if false event will not appear.
        /// </summary>
        public bool Enabled = true;

        /// <summary>
        /// Set scales in Initalize() and then use Getf(ScaleType) or Get(ScaleType) to compute scale in Execute(), this will also generate automatically generate in the config.
        /// </summary>
        public Dictionary<ScaleType, Scale> ScaleList = new Dictionary<ScaleType, Scale>();

        /// <summary>
        /// Set this in Initalize() to specify events to prevent from occuring.
        /// </summary>
        public List<string> EventsToRemove = new List<string>();

        /// <summary>
        /// Set this in Initalize() to specify events to spawn with, these wont be shown in the UI.
        /// </summary>
        public List<string> EventsToSpawnWith = new List<string>();

        internal bool Executed = false;

        /// <summary>
        /// Set this in Initalize() to make monster event(s).
        /// </summary>
        public List<MonsterEvent> monsterEvents = new List<MonsterEvent>();

        /// <summary>
        /// Set this in Initalize() to make a transmutation event.
        /// </summary>
        public ScrapTransmutationEvent scrapTransmutationEvent = new ScrapTransmutationEvent(new Scale(0.0f, 0.0f, 0.0f, 0.0f));

        /// <summary>
        /// This is the event type.
        /// </summary>
        public enum EventType
        {
            VeryBad, Bad, Neutral, Good, VeryGood, Remove
        }

        /// <summary>
        /// Use the right one to generate the right config name and description.
        /// </summary>
        public enum ScaleType
        {
            InsideEnemyRarity, OutsideEnemyRarity, DaytimeEnemyRarity, MinOutsideEnemy, MinInsideEnemy, MaxOutsideEnemy, MaxInsideEnemy,
            ScrapValue, ScrapAmount, FactorySize, MinDensity, MaxDensity, MinCash, MaxCash, MinItemAmount, MaxItemAmount, MinValue, MaxValue, Rarity, MinRarity, MaxRarity,
            MinCut, MaxCut, MinHp, MaxHp, SpawnMultiplier, MaxInsideEnemyCount, MaxOutsideEnemyCount, SpawnCapMultiplier, MinPercentageCut, MaxPercentageCut, MinAmount, MaxAmount, Percentage
        }

        internal static Dictionary<ScaleType, string> ScaleInfoList = new Dictionary<ScaleType, string>() {
            { ScaleType.InsideEnemyRarity, "Enemy is added to Inside enemy list with rarity." }, { ScaleType.OutsideEnemyRarity, "Enemy is added to Outside enemy list with rarity." }, { ScaleType.DaytimeEnemyRarity, "Enemy is added to Daytime enemy list with rarity." },
            { ScaleType.MinOutsideEnemy, "Minimum amount of enemies garunteed to spawn outside." }, { ScaleType.MaxOutsideEnemy, "Maximum amount of enemies garunteed to spawn outside." }, { ScaleType.MinInsideEnemy, "Minimum amount of enemies garunteed to spawn inside." }, { ScaleType.MaxInsideEnemy, "Maximum amount of enemies garunteed to spawn inside." },
            { ScaleType.ScrapValue, "The amount that scrap value is multiplied by." }, { ScaleType.ScrapAmount, "The amount that scrap amount is multiplied by." }, { ScaleType.FactorySize, "The amount that factory size is multiplied by." },
            { ScaleType.MinDensity, "Minimum density value chosen." }, { ScaleType.MaxDensity, "Maximum density value chosen." }, { ScaleType.MinCash, "Minumum amount of cash given." }, { ScaleType.MaxCash, "Maximum amount of cash given." },
            { ScaleType.MinItemAmount, "Minimum amount of items to spawn." }, { ScaleType.MaxItemAmount, "Maximum amount of items to spawn." }, { ScaleType.MinValue, "The minimum value of something." }, { ScaleType.MaxValue, "The maximum value of something." },
            { ScaleType.Rarity, "The general chance of something." }, { ScaleType.MinRarity, "Minimum chance of something." }, { ScaleType.MaxRarity, "Maximum chance of something." }, { ScaleType.MinCut, "Minimum cut taken." }, { ScaleType.MaxCut, "Maximum cut taken." },
            { ScaleType.MinHp, "Minimum possible to be chosen." }, { ScaleType.MaxHp, "Maxmimum possible hp to be chosen." }, { ScaleType.SpawnMultiplier, "Will multiply the spawn chance." }, { ScaleType.SpawnCapMultiplier, "Will multiply the spawn cap." },
            { ScaleType.MaxInsideEnemyCount, "Changes max amount of inside enemies spawnable. " }, { ScaleType.MaxOutsideEnemyCount, "Changes max amount of outside enemies spawnable. " }, { ScaleType.MinPercentageCut, "Minimum possible percentage cut." }, { ScaleType.MaxPercentageCut, "Maximum possible percentage cut." },
            { ScaleType.MinAmount, "Minimum amount of something to be chosen." }, { ScaleType.MaxAmount, "Maximum amount of something to be chosen." }, { ScaleType.Percentage, "This value goes between 0.0 to 1.0." }
        };

        /// <summary>
        /// This is used to scale events by difficulty.
        /// </summary>
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

                if (Type == EventType.VeryBad || Type == EventType.Bad) increment = scale.Increment * Configuration.badEventIncrementMultiplier.Value;
                if (Type == EventType.VeryGood || Type == EventType.Good) increment = scale.Increment * Configuration.goodEventIncrementMultiplier.Value;

                return Mathf.Clamp(scale.Base + (increment * Manager.difficulty), scale.MinCap, Configuration.ignoreMaxCap.Value ? 99999999999.0f : scale.MaxCap);
            }

            public float Computef(EventType type) => Compute(this, type);

            public int Compute(EventType type) => (int)Compute(this, type);
        }

        /// <summary>
        /// This is used to identify said event, preferably use nameof(thisClass) for name.
        /// </summary>
        /// <returns>Returns the given name.</returns>
        public virtual string Name() => "";

        /// <summary>
        /// This is called just right before config is generated.
        /// </summary>
        public virtual void Initalize() { }

        /// <summary>
        /// Event algorithm will only pick this event if this returns true.
        /// </summary>
        /// <returns>Always true if not overriden.</returns>
        public virtual bool AddEventIfOnly() { return true; }

        /// <summary>
        /// This is called after lever is pulled.
        /// </summary>
        public virtual void Execute() { }

        /// <summary>
        /// This is called when ship leaves.
        /// </summary>
        public virtual void OnShipLeave() { }

        /// <summary>
        /// This will only be called once when the game starts.
        /// </summary>
        public virtual void OnGameStart() { }

        /// <summary>
        /// This is used to compute scales, can be overriden.
        /// </summary>
        /// <param name="scaleType">A scale from Scale List, if dosen't exist it will return 0.0f.</param>
        /// <returns>Returns computed float value of given ScaleType if found.</returns>
        public virtual float Getf(ScaleType scaleType)
        {
            try
            {
                return Scale.Compute(ScaleList[scaleType], Type);
            } catch
            {
                Log.LogError(string.Format("Scalar '{0}' for '{1}' not found, returning 0.", scaleType.ToString(), Name()));
            }
            return 0.0f;
        }

        /// <summary>
        /// This computes Getf() and then converts to int.
        /// </summary>
        /// <param name="scaleType">A scale from Scale List, if dosen't exist it will return 0.</param>
        /// <returns>Returns computed int value of given ScaleType if found.</returns>
        public int Get(ScaleType scaleType)
        {
            return (int)Getf(scaleType);
        }

        /// <summary>
        /// Will execute every monster event inside of monsterEvents.
        /// </summary>
        public void ExecuteAllMonsterEvents()
        {
            foreach(MonsterEvent monsterEvent in monsterEvents)
            {
                monsterEvent.Execute();
            }
        }

        /// <summary>
        /// Will return an event from name.
        /// </summary>
        /// <param name="name">Name of event to find.</param>
        /// <returns>Will return said event if found, otherwise it will return the Nothing event.</returns>
        public static MEvent GetEvent(string name)
        {
            int index = EventManager.events.FindIndex(x => x.Name() == name);
            if (index != -1) return EventManager.events[index];

            Log.LogError(string.Format("Event '{0}' dosen't exist, returning nothing event", name));
            return new Events.Nothing();
        }

        /// <summary>
        /// This is used to describe a basic monster event.
        /// </summary>
        public class MonsterEvent
        {
            public EnemyType enemy;

            public Scale insideSpawnRarity, outsideSpawnRarity, minInside, maxInside, minOutside, maxOutside;

            public EventType eventType;

            public MonsterEvent(EnemyType enemy, Scale insideSpawnRarity, Scale outsideSpawnRarity, Scale minInside, Scale maxInside, Scale minOutside, Scale maxOutside)
            {
                this.enemy = enemy;
                assignRarities(insideSpawnRarity, outsideSpawnRarity, minInside, maxInside, minOutside, maxOutside);
            }

            public MonsterEvent(Assets.EnemyName enemyName, Scale insideSpawnRarity, Scale outsideSpawnRarity, Scale minInside, Scale maxInside, Scale minOutside, Scale maxOutside)
            {
                this.enemy = Assets.GetEnemy(enemyName);
                assignRarities(insideSpawnRarity, outsideSpawnRarity, minInside, maxInside, minOutside, maxOutside);
            }

            public MonsterEvent(string enemyName, Scale insideSpawnRarity, Scale outsideSpawnRarity, Scale minInside, Scale maxInside, Scale minOutside, Scale maxOutside)
            {
                this.enemy = Assets.GetEnemy(enemyName);
                assignRarities(insideSpawnRarity, outsideSpawnRarity, minInside, maxInside, minOutside, maxOutside);
            }

            private void assignRarities(Scale insideSpawnRarity, Scale outsideSpawnRarity, Scale minInside, Scale maxInside, Scale minOutside, Scale maxOutside)
            {
                this.insideSpawnRarity = insideSpawnRarity;
                this.outsideSpawnRarity = outsideSpawnRarity;
                this.minInside = minInside;
                this.maxInside = maxInside;
                this.minOutside = minOutside;
                this.maxOutside = maxOutside;
            }

            public void Execute()
            {
                Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, enemy, insideSpawnRarity.Compute(eventType));
                Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, enemy, outsideSpawnRarity.Compute(eventType));
                Manager.Spawn.InsideEnemies(enemy, UnityEngine.Random.Range(minInside.Compute(eventType), maxInside.Compute(eventType) + 1)); 
                Manager.Spawn.OutsideEnemies(enemy, UnityEngine.Random.Range(minOutside.Compute(eventType), maxOutside.Compute(eventType) + 1));
            }
        }

        /// <summary>
        /// This is used to describe a scrap transmutation event.
        /// </summary>
        public class ScrapTransmutationEvent
        {
            public Scale amount; // Between 0.0 to 1.0

            public SpawnableItemWithRarity[] items;

            public ScrapTransmutationEvent(Scale amount, params SpawnableItemWithRarity[] items)
            {
                this.items = items;
                this.amount = amount;
            }

            public void Execute() => Manager.TransmuteScrap(amount.Computef(EventType.Neutral), items);
        }
    }
}
