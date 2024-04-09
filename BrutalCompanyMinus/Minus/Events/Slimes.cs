using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Slimes : MEvent
    {
        public override string Name() => nameof(Slimes);

        public static Slimes Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "The ground is sticky", "It's very slow unless you wack it", "Don't get lost in the sauce", "It's mostly water and pain", "blob" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(20.0f, 0.67f, 25.0f, 60.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(8.0f, 0.27f, 8.0f, 24.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(2.0f, 0.067f, 1.0f, 6.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(2.0f, 0.067f, 1.0f, 6.0f));
        }

        public override void Execute()
        {
            EnemyType Slime = Assets.GetEnemy(Assets.EnemyName.Hygrodere);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Slime, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, Slime, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.OutsideEnemies(Slime, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
            Manager.Spawn.InsideEnemies(Slime, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
