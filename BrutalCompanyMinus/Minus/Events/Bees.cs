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

        public override void Initalize()
        {
            Weight = 3;
            Description = "BEES!!";
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.EnemyRarity, new Scale(40.0f, 0.5f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(2.0f, 0.1f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(3.0f, 0.15f));
        }

        public override void Execute()
        {
            EnemyType CircuitBee = Assets.GetEnemy(Assets.EnemyName.CircuitBee);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.DaytimeEnemies, CircuitBee, Get(ScaleType.EnemyRarity));
            Manager.Spawn.OutsideEnemies(CircuitBee, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
        }
    }
}
