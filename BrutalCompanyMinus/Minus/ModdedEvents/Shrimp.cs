using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Shrimp : MEvent
    {
        public override string Name() => nameof(Shrimp);

        public static Shrimp Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Shrimp", "Actual doggo", "You have to feed it..." };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(25.0f, 0.417f, 25.0f, 50.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(4.0f, 0.134f, 4.0f, 12.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(1.0f, 0.05f, 2.0f, 4.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(0.0f, 0.05f, 0.0f, 1.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(0.0f, 0.084f, 0.0f, 2.0f));
        }

        public override bool AddEventIfOnly() => Compatibility.lcOfficePresent;

        public override void Execute()
        {
            EnemyType Shrimp = Assets.GetEnemy("ShrimpEnemy");

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Shrimp, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, Shrimp, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.InsideEnemies(Shrimp, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
            Manager.Spawn.OutsideEnemies(Shrimp, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
        }
    }
}
