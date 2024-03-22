using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Lizard : MEvent
    {
        public override string Name() => nameof(Lizard);

        public static Lizard Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Description = "They dont bite... i swear";
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(25.0f, 0.84f, 25.0f, 75.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(4.0f, 0.134f, 4.0f, 12.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.05f, 1.0f, 4.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(2.0f, 0.067f, 2.0f, 6.0f));
        }

        public override void Execute()
        {
            EnemyType Lizard = Assets.GetEnemy(Assets.EnemyName.SporeLizard);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Lizard, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, Lizard, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.InsideEnemies(Lizard, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
