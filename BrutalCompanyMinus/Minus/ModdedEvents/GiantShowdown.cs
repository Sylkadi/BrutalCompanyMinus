using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class GiantShowdown : MEvent
    {
        public override string Name() => nameof(GiantShowdown);

        public static GiantShowdown Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Attack on titan!!", "Battle of giants", "Take your bets..." };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(1.0f, 0.0f, 1.0f, 1.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(30.0f, 1.0f, 1.0f, 90.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(0.0f, 0.0f, 0.0f, 0.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(0.0f, 0.02f, 0.0f, 1.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(2.0f, 0.034f, 2.0f, 4.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(2.0f, 0.034f, 2.0f, 4.0f));
        }

        public override bool AddEventIfOnly() => Compatibility.theGiantSpecimensPresent;

        public override void Execute()
        {
            EnemyType RedWoodGiant = Assets.GetEnemy("PinkGiantObj");

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, RedWoodGiant, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, RedWoodGiant, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.InsideEnemies(RedWoodGiant, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
            Manager.Spawn.OutsideEnemies(RedWoodGiant, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));

            EnemyType forestGiant = Assets.GetEnemy(Assets.EnemyName.ForestKeeper);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, forestGiant, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, forestGiant, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.InsideEnemies(forestGiant, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
            Manager.Spawn.OutsideEnemies(forestGiant, UnityEngine.Random.Range((int)(Getf(ScaleType.MinOutsideEnemy) * 6.0f), (int)(Getf(ScaleType.MaxOutsideEnemy) * 6.0f) + 1));
        }
    }
}
