using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class BaboonHorde : MEvent
    {
        public override string Name() => nameof(BaboonHorde);

        public override void Initalize()
        {
            Weight = 1;
            Description = "You feel outnumbered";
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            ScaleList.Add(ScaleType.EnemyRarity, new Scale(35.0f, 1.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(5.0f, 0.08f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(8.0f, 0.15f));
        }

        public override void Execute()
        {
            EnemyType BaboonHawk = Assets.GetEnemy(Assets.EnemyName.BaboonHawk);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, BaboonHawk, Get(ScaleType.EnemyRarity));
            Manager.Spawn.OutsideEnemies(BaboonHawk, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
        }
    }
}
