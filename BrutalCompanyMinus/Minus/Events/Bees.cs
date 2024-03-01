using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Bees : MEvent
    {
        public override string Name() => nameof(Bees);

        public static Bees Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Description = "BEES!!";
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.DaytimeEnemyRarity, new Scale(25.0f, 0.84f, 25.0f, 75.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(2.0f, 0.034f, 2.0f, 4.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(2.0f, 0.067f, 2.0f, 6.0f));
        }

        public override void Execute()
        {
            EnemyType CircuitBee = Assets.GetEnemy(Assets.EnemyName.CircuitBee);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.DaytimeEnemies, CircuitBee, Get(ScaleType.DaytimeEnemyRarity));
            Manager.Spawn.OutsideEnemies(CircuitBee, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
        }
    }
}
