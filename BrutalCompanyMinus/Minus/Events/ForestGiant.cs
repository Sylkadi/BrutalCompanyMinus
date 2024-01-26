using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class ForestGiant : MEvent
    {
        public override string Name() => nameof(ForestGiant);

        public override void Initalize()
        {
            Weight = 1;
            Description = "Eddie hall in the facility?";
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.03f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(1.0f, 0.05f));
        }

        public override void Execute()
        {
            Manager.Spawn.InsideEnemies(Assets.GetEnemy(Assets.EnemyName.ForestKeeper), UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
