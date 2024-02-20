using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class GypsyColony : MEvent
    {
        public override string Name() => nameof(GypsyColony);

        public override void Initalize()
        {
            Weight = 1;
            Description = "Theres too many of them";
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(HoardingBugs) };
            EventsToSpawnWith = new List<string> { nameof(ScarceOutsideScrap) };

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(50.0f, 0.84f, 50.0f, 100.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(15.0f, 0.5f, 50.0f, 45.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(10.0f, 0.167f, 10.0f, 20.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(15.0f, 0.167f, 10.0f, 25.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(4.0f, 0.067f, 4.0f, 8.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(6.0f, 0.1f, 6.0f, 12.0f));
        }

        public override void Execute()
        {
            EnemyType HoardingBug = Assets.GetEnemy(Assets.EnemyName.HoardingBug);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, HoardingBug, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, HoardingBug, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.OutsideEnemies(HoardingBug, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
            Manager.Spawn.InsideEnemies(HoardingBug, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
