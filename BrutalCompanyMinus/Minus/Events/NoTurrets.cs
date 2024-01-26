using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoTurrets : MEvent
    {
        public override string Name() => nameof(NoTurrets);

        public override void Initalize()
        {
            Weight = 1;
            Description = "No turrets";
            ColorHex = "#008000";
            Type = EventType.Remove;

            EventsToRemove = new List<string>() { nameof(Turrets), nameof(OutsideTurrets), nameof(Warzone) };
        }

        public override void Execute()
        {
            AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 0f));

            foreach (SpawnableMapObject obj in RoundManager.Instance.currentLevel.spawnableMapObjects)
            {
                if (obj.prefabToSpawn.name == Assets.GetObject(Assets.ObjectName.Turret).name)
                {
                    obj.numberToSpawn = curve;
                }
            }
        }

    }
}
