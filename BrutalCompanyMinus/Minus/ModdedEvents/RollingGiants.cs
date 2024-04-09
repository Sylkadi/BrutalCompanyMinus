using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class RollingGiants : MEvent
    {
        public override string Name() => nameof(RollingGiants);

        public static RollingGiants Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Rolling Giants!!", "It wants to touch...", "What even is this thing?????" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(10.0f, 0.34f, 10.0f, 30.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(3.0f, 0.1f, 4.0f, 9.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(0.0f, 0.034f, 0.0f, 2.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
        }

        public override bool AddEventIfOnly() => Compatibility.rollinggiantPresent;

        public override void Execute()
        {
            EnemyType RollingGiant = Assets.GetEnemy("RollingGiant_EnemyType");
            EnemyType RollingGiantOutside = Assets.GetEnemy("RollingGiant_EnemyType_Outside");

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, RollingGiant, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, RollingGiantOutside, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.InsideEnemies(RollingGiant, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
            Manager.Spawn.OutsideEnemies(RollingGiantOutside, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
        }
    }
}
