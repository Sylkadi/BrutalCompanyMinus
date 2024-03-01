using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class HoardingBugs : MEvent
    {
        public override string Name() => nameof(HoardingBugs);

        public static HoardingBugs Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Description = "They look cute.";
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            EventsToSpawnWith = new List<string>() { nameof(ScarceOutsideScrap) };

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(25.0f, 0.417f, 25.0f, 50.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(3.0f, 0.067f, 3.0f, 7.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(4.0f, 0.084f, 4.0f, 9.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(1.0f, 0.05f, 1.0f, 4.0f));
        }

        public override void Execute()
        {
            EnemyType HoardingBug = Assets.GetEnemy(Assets.EnemyName.HoardingBug);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, HoardingBug, Get(ScaleType.InsideEnemyRarity));

            Manager.Spawn.OutsideEnemies(HoardingBug, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
            Manager.Spawn.InsideEnemies(HoardingBug, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
