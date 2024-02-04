using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class AntiCoilhead : MEvent
    {
        public override string Name() => nameof(AntiCoilhead);

        public override void Initalize()
        {
            Weight = 1;
            Description = "Dont look at them...";
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(Coilhead), nameof(LeaflessBrownTrees) };
            EventsToSpawnWith = new List<string>() { nameof(LeaflessTrees) };

            ScaleList.Add(ScaleType.EnemyRarity, new Scale(30.0f, 0.5f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.08f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(1.0f, 0.1f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(1.0f, 0.08f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(1.0f, 0.1f));
        }

        public override void Execute()
        {
            EnemyType AntiCoilHead = Assets.antiCoilHead;

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, AntiCoilHead, Get(ScaleType.EnemyRarity));
            Manager.RemoveSpawn(Assets.EnemyNameList[Assets.EnemyName.CoilHead]);
            Manager.Spawn.InsideEnemies(AntiCoilHead, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
            Manager.Spawn.OutsideEnemies(AntiCoilHead, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
        }
    }
}
