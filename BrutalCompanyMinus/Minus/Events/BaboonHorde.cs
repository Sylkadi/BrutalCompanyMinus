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

        public static BaboonHorde Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "You feel outnumbered", "Keep the door's closed.", "Hear their calls, see their shadows cover the land.", "Why are they also inside??" };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(50.0f, 0.84f, 50.0f, 100.0f));
            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(10.0f, 0.34f, 10.0f, 30.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(2.0f, 0.034f, 2.0f, 4.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(3.0f, 0.05f, 3.0f, 6.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(5.0f, 0.084f, 5.0f, 10.0f));
        }

        public override void Execute()
        {
            EnemyType BaboonHawk = Assets.GetEnemy(Assets.EnemyName.BaboonHawk);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, BaboonHawk, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, BaboonHawk, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.OutsideEnemies(BaboonHawk, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
            Manager.Spawn.InsideEnemies(BaboonHawk, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
