using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Arachnophobia : MEvent
    {
        public override string Name() => nameof(Arachnophobia);

        public override void Initalize()
        {
            Weight = 1;
            Description = "Nightmare Facility";
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(Trees), nameof(LeaflessBrownTrees) };
            EventsToSpawnWith = new List<string>() { nameof(LeaflessTrees) };

            ScaleList.Add(ScaleType.EnemyRarity, new Scale(80.0f, 1.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(7.0f, 0.1f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(11.0f, 0.2f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(3.0f, 0.05f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(5.0f, 0.1f));
        }

        public override void Execute()
        {
            EnemyType BunkerSpider = Assets.GetEnemy(Assets.EnemyName.BunkerSpider);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, BunkerSpider, Get(ScaleType.EnemyRarity));

            Manager.Spawn.OutsideEnemies(BunkerSpider, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
            Manager.Spawn.InsideEnemies(BunkerSpider, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
