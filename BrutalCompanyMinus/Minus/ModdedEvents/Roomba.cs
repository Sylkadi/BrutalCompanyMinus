using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Roomba : MEvent
    {
        public override string Name() => nameof(Roomba);

        public static Roomba Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Moving Landmines!!", "Facility hoovers", "Weapons of war", "These things are against the Geneva convention" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(25.0f, 0.417f, 25.0f, 50.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(4.0f, 0.134f, 4.0f, 12.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(3.0f, 0.1f, 3.0f, 9.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(4.0f, 0.134f, 4.0f, 12.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(0.0f, 0.05f, 0.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(1.0f, 0.084f, 1.0f, 6.0f));
        }

        public override bool AddEventIfOnly() => Compatibility.lethalThingsPresent;

        public override void Execute()
        {
            EnemyType Roomba = Assets.GetEnemy("Boomba");

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Roomba, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, Roomba, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.InsideEnemies(Roomba, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
            Manager.Spawn.OutsideEnemies(Roomba, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
        }
    }
}
