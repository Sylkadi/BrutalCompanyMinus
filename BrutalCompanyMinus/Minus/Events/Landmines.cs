using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Landmines : MEvent
    {
        public override string Name() => nameof(Landmines);

        public static Landmines Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Watch your step", "This facility is rigged", "Landmines, yes" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(12.0f, 0.3f, 12.0f, 30.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(16.0f, 0.44f, 16.0f, 42.0f));
        }

        public override bool AddEventIfOnly() => RoundManager.Instance.currentLevel.spawnableMapObjects.ToList().Exists(x => x.prefabToSpawn.name == Assets.ObjectNameList[Assets.ObjectName.Landmine]);

        public override void Execute()
        {
            AnimationCurve curve = new AnimationCurve(new Keyframe(0f, Get(ScaleType.MaxInsideEnemy)), new Keyframe(1f, Get(ScaleType.MinInsideEnemy)));

            foreach(SpawnableMapObject obj in RoundManager.Instance.currentLevel.spawnableMapObjects)
            {
                if(obj.prefabToSpawn.name == Assets.GetObject(Assets.ObjectName.Landmine).name)
                {
                    obj.numberToSpawn = curve;
                }
            }

        }
    }
}
