using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Butlers : MEvent
    {
        public override string Name() => nameof(Butlers);

        public static Butlers Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Nicely fashioned gentlemen", "A touch of class descends upon this planet", "Pop!", "Knives" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(25.0f, 0.417f, 25.0f, 50.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(5.0f, 0.17f, 5.0f, 15.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.017f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(2.0f, 0.034f, 2.0f, 6.0f));
        }

        public override bool AddEventIfOnly() => Compatibility.IsVersion50;

        public override void Execute()
        {
            EnemyType Butler = Assets.GetEnemy(Assets.EnemyName.Butler);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Butler, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, Butler, Get(ScaleType.OutsideEnemyRarity));
            Manager.Spawn.InsideEnemies(Butler, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
