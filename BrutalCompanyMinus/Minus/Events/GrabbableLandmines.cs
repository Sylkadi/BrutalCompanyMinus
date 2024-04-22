using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class GrabbableLandmines : MEvent
    {
        public static bool Active = false;
        public static bool LandmineDisabled = false;
        public override string Name() => nameof(GrabbableLandmines);

        public static GrabbableLandmines Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Some mines have turned into scrap...", "This was a wonderful idea", "Beep, Beep, Beep.", "You can now sell some of the landmines." };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.Rarity, new Scale(0.33f, 0.0066f, 0.33f, 1.0f));
            ScaleList.Add(ScaleType.MinAmount, new Scale(2.0f, 0.08f, 2.0f, 10.0f));
            ScaleList.Add(ScaleType.MaxAmount, new Scale(3.0f, 0.12f, 3.0f, 15.0f));
        }

        public override bool AddEventIfOnly() => RoundManager.Instance.currentLevel.spawnableMapObjects.ToList().Exists(x => x.prefabToSpawn.name == Assets.ObjectNameList[Assets.ObjectName.Landmine]);

        public override void Execute() {
            Active = true;
            LandmineDisabled = false;
            RoundManager.Instance.currentLevel.spawnableMapObjects = RoundManager.Instance.currentLevel.spawnableMapObjects.Add(new SpawnableMapObject()
            {
                prefabToSpawn = Assets.GetObject(Assets.ObjectName.Landmine),
                numberToSpawn = new AnimationCurve(new Keyframe(0f, Get(ScaleType.MinAmount)), new Keyframe(1f, Get(ScaleType.MaxAmount))),
                spawnFacingAwayFromWall = false,
                spawnFacingWall = false,
                spawnWithBackToWall = false,
                spawnWithBackFlushAgainstWall = false,
                requireDistanceBetweenSpawns = false,
                disallowSpawningNearEntrances = false
            });
        } 

        public override void OnShipLeave() {
            Active = false;
            LandmineDisabled = true;
        } 

        public override void OnGameStart()
        {
            Active = false;
            LandmineDisabled = false;
        }
    }
}
