using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Spiders : MEvent
    {
        public override string Name() => nameof(Spiders);

        public override void Initalize()
        {
            Weight = 3;
            Description = "Your skin crawls...";
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            EventsToRemove = new List<string>() { nameof(Trees), nameof(LeaflessTrees) };
            EventsToSpawnWith = new List<string>() { nameof(LeaflessBrownTrees) };

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(25.0f, 0.417f, 25.0f, 50.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(2.0f, 0.084f, 2.0f, 4.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(2.0f, 0.1f, 2.0f, 6.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(2.0f, 0.034f, 2.0f, 4.0f));
        }

        public override void Execute()
        {
            EnemyType BunkerSpider = Assets.GetEnemy(Assets.EnemyName.BunkerSpider);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, BunkerSpider, Get(ScaleType.InsideEnemyRarity));

            Manager.Spawn.OutsideEnemies(BunkerSpider, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
            Manager.Spawn.InsideEnemies(BunkerSpider, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
