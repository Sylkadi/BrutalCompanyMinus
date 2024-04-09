using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Turrets : MEvent
    {
        public override string Name() => nameof(Turrets);

        public static Turrets Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Turrets!!", "Home defense systems", "Panic and scream" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(9.0f, 0.15f, 9.0f, 18.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(12.0f, 0.25f, 12.0f, 27.0f));
        }

        public override bool AddEventIfOnly() => RoundManager.Instance.currentLevel.spawnableMapObjects.ToList().Exists(x => x.prefabToSpawn.name == Assets.ObjectNameList[Assets.ObjectName.Turret]);

        public override void Execute()
        {
            AnimationCurve curve = new AnimationCurve(new Keyframe(0f, Get(ScaleType.MaxInsideEnemy)), new Keyframe(1f, Get(ScaleType.MinInsideEnemy)));

            foreach(SpawnableMapObject obj in RoundManager.Instance.currentLevel.spawnableMapObjects)
            {
                if(obj.prefabToSpawn.name == Assets.GetObject(Assets.ObjectName.Turret).name)
                {
                    obj.numberToSpawn = curve;
                }
            }
        }
    }
}
