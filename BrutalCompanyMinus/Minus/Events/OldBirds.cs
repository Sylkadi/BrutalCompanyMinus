using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class OldBirds : MEvent
    {
        public override string Name() => nameof(OldBirds);

        public static OldBirds Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Who put that thing in there??", "Mentally deranged toddlers", "Does the lighnting kill them?" };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToSpawnWith = new List<string>() { nameof(Landmines), nameof(OutsideLandmines) };

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(1.0f, 0.84f, 1.0f, 6.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(30.0f, 1.0f, 30.0f, 90.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.017f, 1.0f, 2.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(1.0f, 0.067f, 1.0f, 5.0f));
        }

        public override bool AddEventIfOnly() => Compatibility.IsVersion50;

        public override void Execute()
        {
            EnemyType OldBird = Assets.GetEnemy(Assets.EnemyName.OldBird);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, OldBird, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, OldBird, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.OutsideEnemies(OldBird, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
            Manager.Spawn.InsideEnemies(OldBird, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
