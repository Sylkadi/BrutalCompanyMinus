using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Nutcracker : MEvent
    {
        public override string Name() => nameof(Nutcracker);

        public override void Initalize()
        {
            Weight = 1;
            Description = "Average american school experience";
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToSpawnWith = new List<string>() { nameof(Turrets) };

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(20.0f, 1.0f, 20.0f, 80.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(10.0f, 0.34f, 10.0f, 30.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(2.0f, 0.034f, 2.0f, 4.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(2.0f, 0.067f, 2.0f, 6.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(0.0f, 0.034f, 0.0f, 2.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(1.0f, 0.034f, 0.0f, 3.0f));
        }

        public override void Execute()
        {
            EnemyType Nutcracker = Assets.GetEnemy(Assets.EnemyName.NutCracker);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Nutcracker, Get(ScaleType.InsideEnemyRarity));

            Manager.Spawn.InsideEnemies(Nutcracker, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
            Manager.Spawn.OutsideEnemies(Nutcracker, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
        }
    }
}
