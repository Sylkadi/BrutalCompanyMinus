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

        public override void Initalize()
        {
            Weight = 3;
            Description = "The ground is sticky";
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.EnemyRarity, new Scale(30.0f, 0.5f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.05f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(2.0f, 0.1f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(1.0f, 0.04f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(2.0f, 0.08f));
        }

        public override void Execute()
        {
            EnemyType Slime = Assets.GetEnemy(Assets.EnemyName.Hygrodere);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Slime, Get(ScaleType.EnemyRarity));

            Manager.Spawn.OutsideEnemies(Slime, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
            Manager.Spawn.InsideEnemies(Slime, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
