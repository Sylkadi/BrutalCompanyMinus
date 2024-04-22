using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class GrabbableTurrets : MEvent
    {
        public static bool Active = false;
        public override string Name() => nameof(GrabbableTurrets);

        public static GrabbableTurrets Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Some turrets have turned into scrap...", "You can now offically sell some of the turrets, enjoy", "You can bring these home for an automated defense system for intruders." };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.Rarity, new Scale(0.33f, 0.0066f, 0.33f, 1.0f));
            ScaleList.Add(ScaleType.MinAmount, new Scale(2.0f, 0.06f, 2.0f, 8.0f));
            ScaleList.Add(ScaleType.MaxAmount, new Scale(3.0f, 0.09f, 3.0f, 12.0f));
        }

        public override bool AddEventIfOnly() => RoundManager.Instance.currentLevel.spawnableMapObjects.ToList().Exists(x => x.prefabToSpawn.name == Assets.ObjectNameList[Assets.ObjectName.Turret]);

        public override void Execute()
        {
            Active = true;
            RoundManager.Instance.currentLevel.spawnableMapObjects = RoundManager.Instance.currentLevel.spawnableMapObjects.Add(new SpawnableMapObject()
            {
                prefabToSpawn = Assets.GetObject(Assets.ObjectName.Turret),
                numberToSpawn = new AnimationCurve(new Keyframe(0f, Get(ScaleType.MinAmount)), new Keyframe(1f, Get(ScaleType.MaxAmount))),
                spawnFacingAwayFromWall = true,
                spawnFacingWall = false,
                spawnWithBackToWall = false,
                spawnWithBackFlushAgainstWall = false,
                requireDistanceBetweenSpawns = false,
                disallowSpawningNearEntrances = false
            });
        }

        public override void OnShipLeave() => Active = false;

        public override void OnGameStart() => Active = false;
    }
}
