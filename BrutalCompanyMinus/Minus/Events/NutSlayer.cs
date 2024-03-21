using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NutSlayer : MEvent
    {
        public override string Name() => nameof(NutSlayer);

        public static NutSlayer Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Description = "The nut slayer is inside the facility...";
            ColorHex = "#280000";
            Type = EventType.VeryBad;

            EventsToSpawnWith = new List<string>() { nameof(Gloomy), nameof(Thumpers), nameof(Spiders), nameof(Masked) };
            EventsToRemove = new List<string>() { nameof(HeavyRain), nameof(Raining) };

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.0f, 1.0f, 1.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(1.0f, 0.0f, 1.0f, 1.0f));
            ScaleList.Add(ScaleType.SpawnMultiplier, new Scale(1.35f, 0.01084f, 1.35f, 2.0f));
            ScaleList.Add(ScaleType.SpawnCapMultiplier, new Scale(1.5f, 0.0417f, 1.5f, 4.0f));
        }
        public override void Execute() 
        {
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Assets.nutSlayer, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, Assets.nutSlayer, Get(ScaleType.OutsideEnemyRarity));

            Manager.MultiplySpawnChance(RoundManager.Instance.currentLevel, Getf(ScaleType.SpawnMultiplier));
            Manager.MultiplySpawnCap(Getf(ScaleType.SpawnCapMultiplier));

            Manager.Spawn.InsideEnemies(Assets.nutSlayer, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        } 
    }
}
