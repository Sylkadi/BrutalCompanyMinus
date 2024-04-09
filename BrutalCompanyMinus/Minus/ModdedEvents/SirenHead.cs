using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class SirenHead : MEvent
    {
        public override string Name() => nameof(SirenHead);

        public static SirenHead Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "God would like to speak with you...", "NINE. EIGHTEEN. ONE. CHILD. SEVENTEEN. REMOVE. VILE." };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(30.0f, 1.0f, 1.0f, 90.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(0.0f, 0.022f, 0.0f, 1.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(0.0f, 0.0167f, 0.0f, 2.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
        }

        public override bool AddEventIfOnly() => Compatibility.sirenheadPresent;

        public override void Execute()
        {
            EnemyType SirenHead = Assets.GetEnemy("SirenHead");

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, SirenHead, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, SirenHead, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.InsideEnemies(SirenHead, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
            Manager.Spawn.OutsideEnemies(SirenHead, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
        }
    }
}
