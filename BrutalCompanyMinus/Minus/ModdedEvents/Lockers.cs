using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Lockers : MEvent
    {
        public override string Name() => nameof(Lockers);

        public static Lockers Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Lockers", "They look like iron maidens", "The chance of you dying has increased" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(20.0f, 0.67f, 20.0f, 60.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(3.0f, 0.084f, 3.0f, 9.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.05f, 1.0f, 4.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(2.0f, 0.05f, 2.0f, 5.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(0.0f, 0.05f, 0.0f, 1.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(0.0f, 0.084f, 0.0f, 2.0f));
        }

        public override bool AddEventIfOnly() => Compatibility.lockerPresent;

        public override void Execute()
        {
            EnemyType Locker = Assets.GetEnemy("LockerEnemy");

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Locker, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, Locker, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.InsideEnemies(Locker, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
            Manager.Spawn.OutsideEnemies(Locker, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
        }
    }
}
