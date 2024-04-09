using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Worms : MEvent
    {
        public override string Name() => nameof(Worms);

        public static Worms Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Bug Breach Detected", "The ultimate fine dining experience", "Dont make out with those things" };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(SnareFleas) };

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(5.0f, 0.17f, 5.0f, 15.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(30.0f, 1.0f, 30.0f, 90.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.034f, 0.0f, 2.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(1.0f, 0.05f, 0.0f, 3.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
        }
        
        public override void Execute()
        {
            EnemyType Worm = Assets.GetEnemy(Assets.EnemyName.EarthLeviathan);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Worm, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, Worm, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.OutsideEnemies(Worm, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
            Manager.Spawn.InsideEnemies(Worm, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));

            if (SnareFleas.Instance == null || !SnareFleas.Instance.Enabled) return;

            EnemyType SnareFlea = Assets.GetEnemy(Assets.EnemyName.SnareFlea);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, SnareFlea, (int)(Scale.Compute(SnareFleas.Instance.ScaleList[ScaleType.OutsideEnemyRarity], EventType.Bad) * 3.0f));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, SnareFlea, (int)(Scale.Compute(SnareFleas.Instance.ScaleList[ScaleType.InsideEnemyRarity], EventType.Bad) * 1.333f));
            Manager.Spawn.OutsideEnemies(SnareFlea, UnityEngine.Random.Range((int)Scale.Compute(SnareFleas.Instance.ScaleList[ScaleType.MinInsideEnemy]), (int)Scale.Compute(SnareFleas.Instance.ScaleList[ScaleType.MaxInsideEnemy]) + 1));
            Manager.Spawn.InsideEnemies(SnareFlea, UnityEngine.Random.Range((int)(Scale.Compute(SnareFleas.Instance.ScaleList[ScaleType.MinInsideEnemy]) * 2.5f), (int)(Scale.Compute(SnareFleas.Instance.ScaleList[ScaleType.MaxInsideEnemy]) * 2.5f) + 1));
        }
    }
}
