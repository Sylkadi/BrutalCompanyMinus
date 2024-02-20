using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Locusts : MEvent
    {
        public override string Name() => nameof(Locusts);

        public override void Initalize()
        {
            Weight = 8;
            Description = "Locust season is here";
            ColorHex = "#FFFFFF";
            Type = EventType.Neutral;

            ScaleList.Add(ScaleType.DaytimeEnemyRarity, new Scale(25.0f, 0.0f, 25.0f, 25.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(5.0f, 0.0f, 5.0f, 5.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(8.0f, 0.0f, 8.0f, 8.0f));
        }

        public override void Execute()
        {
            EnemyType Locust = Assets.GetEnemy(Assets.EnemyName.RoamingLocust);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.DaytimeEnemies, Locust, Get(ScaleType.DaytimeEnemyRarity));
            Manager.Spawn.OutsideEnemies(Locust, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
        }
    }
}
