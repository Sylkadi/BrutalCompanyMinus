using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class AntiCoilhead : MEvent
    {
        public override string Name() => nameof(AntiCoilhead);

        public static AntiCoilhead Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Dont look at them...", "Glowy eyes", "My favourite", "I hope you don't have friends when dealing with these" };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(Coilhead), nameof(LeaflessBrownTrees), nameof(Trees), nameof(HeavyRain) };
            EventsToSpawnWith = new List<string>() { nameof(LeaflessTrees), nameof(Gloomy) };   

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(25.0f, 0.84f, 25.0f, 75.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(10.0f, 0.34f, 10.0f, 30.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(1.0f, 0.05f, 1.0f, 4.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(1.0f, 0.017f, 1.0f, 2.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
        }

        public override void Execute()
        {
            EnemyType AntiCoilHead = Assets.antiCoilHead;

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, AntiCoilHead, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, AntiCoilHead, Get(ScaleType.OutsideEnemyRarity));
            Manager.RemoveSpawn(Assets.EnemyName.CoilHead);
            Manager.Spawn.InsideEnemies(AntiCoilHead, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
            Manager.Spawn.OutsideEnemies(AntiCoilHead, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
        }
    }
}
