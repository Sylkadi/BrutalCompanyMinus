using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Thumpers : MEvent
    {
        public override string Name() => nameof(Thumpers);

        public override void Initalize()
        {
            Weight = 3;
            Description = "You need to run";
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.EnemyRarity, new Scale(40.0f, 0.5f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.07f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(1.0f, 0.1f));
        }

        public override void Execute()
        {
            EnemyType Thumper = Assets.GetEnemy(Assets.EnemyName.Thumper);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Thumper, Get(ScaleType.EnemyRarity));
            Manager.Spawn.InsideEnemies(Thumper, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
