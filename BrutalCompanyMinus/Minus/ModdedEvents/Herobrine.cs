using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Herobrine : MEvent
    {
        public override string Name() => nameof(Herobrine);

        public static Herobrine Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Herobrine has been removed from the game.", "What is that...", "The apparition is here to take your soul." };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(8.0f, 0.4f, 8.0f, 32.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(0.0f, 0.022f, 0.0f, 1.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(0.0f, 0.034f, 0.0f, 2.0f));
        }

        public override bool AddEventIfOnly() => Compatibility.herobrinePresent;

        public override void Execute()
        {
            EnemyType Herobrine = Assets.GetEnemy("Herobrine");

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Herobrine, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, Herobrine, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.InsideEnemies(Herobrine, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
            Manager.Spawn.OutsideEnemies(Herobrine, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
        }
    }
}
