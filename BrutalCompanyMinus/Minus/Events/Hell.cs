using BrutalCompanyMinus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Hell : MEvent
    {
        public override string Name() => nameof(Hell);

        public static Hell Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Description = "Great reward, but at what cost...";
            ColorHex = "#280000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(Trees), nameof(LeaflessBrownTrees), nameof(Gloomy), nameof(Raining), nameof(HeavyRain) };
            EventsToSpawnWith = new List<string>() { nameof(LeaflessTrees), nameof(AntiCoilhead), nameof(InsideBees), nameof(KamikazieBugs), nameof(SnareFleas), nameof(Thumpers) };

            ScaleList.Add(ScaleType.ScrapValue, new Scale(1.75f, 0.0542f, 1.75f, 4.0f));
            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.75f, 0.0542f, 1.75f, 4.0f));
            ScaleList.Add(ScaleType.FactorySize, new Scale(1.25f, 0.0125f, 1.5f, 2.0f));
            ScaleList.Add(ScaleType.SpawnCapMultiplier, new Scale(3.0f, 0.2f, 3.0f, 15.0f));
            ScaleList.Add(ScaleType.SpawnMultiplier, new Scale(2.0f, 0.034f, 2.0f, 4.0f));
        }

        public override void Execute()
        {
            Manager.SetAtmosphere(Assets.AtmosphereNameList[Assets.AtmosphereName.Exclipsed], true);

            RoundManager.Instance.currentLevel.enemySpawnChanceThroughoutDay = new AnimationCurve(new Keyframe(0f, 1.0f), new Keyframe(1.0f, 8.0f));
            RoundManager.Instance.currentLevel.outsideEnemySpawnChanceThroughDay = new AnimationCurve(new Keyframe(0f, -10f), new Keyframe(0.15f, -10.0f), new Keyframe(0.16f, 1.0f), new Keyframe(1.0f, 8.0f));

            Manager.scrapValueMultiplier *= Getf(ScaleType.ScrapValue);
            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);
            Manager.currentLevel.factorySizeMultiplier *= Getf(ScaleType.FactorySize);
            Manager.MultiplySpawnChance(RoundManager.Instance.currentLevel, Getf(ScaleType.SpawnMultiplier));
            Manager.MultiplySpawnCap(Getf(ScaleType.SpawnCapMultiplier));

            Manager.Spawn.OutsideEnemies(Assets.GetEnemy(Assets.EnemyName.BunkerSpider), 3);
            Manager.Spawn.InsideEnemies(Assets.GetEnemy(Assets.EnemyName.BunkerSpider), 2);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, Assets.GetEnemy(Assets.EnemyName.ForestKeeper), 1);
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Assets.GetEnemy(Assets.EnemyName.Bracken), 15);
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Assets.GetEnemy(Assets.EnemyName.NutCracker), 5);
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Assets.nutSlayer, 1);
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Assets.GetEnemy(Assets.EnemyName.Bracken), 15);
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Assets.GetEnemy(Assets.EnemyName.EyelessDog), 3);
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Assets.GetEnemy(Assets.EnemyName.BaboonHawk), 5);
        }
    }
}
