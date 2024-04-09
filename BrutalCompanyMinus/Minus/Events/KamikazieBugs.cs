using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class KamikazieBugs : MEvent
    {
        public override string Name() => nameof(KamikazieBugs);

        public static KamikazieBugs Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "I rather not anger them if I were you.", "Did you know that hoarding bugs have organs? Well these ones have bombs...", "I hope you meet an army of them.", "An invasion of self-destructing pests insdie the facility." };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(25.0f, 0.417f, 25.0f, 50.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(4.0f, 0.134f, 4.0f, 12.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(3.0f, 0.067f, 3.0f, 7.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(4.0f, 0.084f, 4.0f, 9.0f));
        }

        public override void Execute()
        {
            EnemyType KamikazieBug = Assets.kamikazieBug;

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, KamikazieBug, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, KamikazieBug, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.InsideEnemies(KamikazieBug, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
