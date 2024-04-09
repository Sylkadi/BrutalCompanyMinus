using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class ImmortalSnail : MEvent
    {
        public override string Name() => nameof(ImmortalSnail);

        public static ImmortalSnail Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "It's very slow..", "Looks pretty innocent", "A moving thermonuclear bomb" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(10.0f, 0.5f, 10.0f, 40.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(5.0f, 0.167f, 5.0f, 15.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(1.0f, 0.05f, 1.0f, 4.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(0.0f, 0.034f, 0.0f, 2.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(0.0f, 0.034f, 0.0f, 2.0f));
        }

        public override bool AddEventIfOnly() => Compatibility.immortalSnailPresent;

        public override void Execute()
        {
            EnemyType ImmortalSnail = Assets.GetEnemy("ImmortalSnail.EnemyType");

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, ImmortalSnail, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, ImmortalSnail, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.InsideEnemies(ImmortalSnail, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
            Manager.Spawn.OutsideEnemies(ImmortalSnail, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
        }
    }
}
