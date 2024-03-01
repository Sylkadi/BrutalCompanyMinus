using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Coilhead : MEvent
    {
        public override string Name() => nameof(Coilhead);

        public static Coilhead Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Description = "Coilheads detected in the facility!";
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(30.0f, 1.0f, 30.0f, 90.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(2.0f, 0.05f, 1.0f, 5.0f));
        }

        public override void Execute()
        {
            EnemyType Coilhead = Assets.GetEnemy(Assets.EnemyName.CoilHead);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Coilhead, Get(ScaleType.InsideEnemyRarity));
            Manager.Spawn.InsideEnemies(Coilhead, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
