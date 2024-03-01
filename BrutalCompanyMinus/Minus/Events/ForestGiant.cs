using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class ForestGiant : MEvent
    {
        public override string Name() => nameof(ForestGiant);

        public static ForestGiant Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Description = "Eddie hall in the facility?";
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(2.0f, 0.067f, 2.0f, 6.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(1.0f, 0.05f, 1.0f, 4.0f));
        }

        public override void Execute()
        {
            EnemyType ForestGiant = Assets.GetEnemy(Assets.EnemyName.ForestKeeper);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, ForestGiant, Get(ScaleType.InsideEnemyRarity));
            Manager.Spawn.InsideEnemies(ForestGiant, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
