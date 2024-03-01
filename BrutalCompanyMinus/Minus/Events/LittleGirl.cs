using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class LittleGirl : MEvent
    {
        public override string Name() => nameof(LittleGirl);

        public static LittleGirl Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Description = "They just want to touch you";
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(2.0f, 0.067f, 2.0f, 6.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(3.0f, 0.1f, 3.0f, 9.0f));
        }
        public override void Execute() => Manager.Spawn.InsideEnemies(Assets.GetEnemy(Assets.EnemyName.GhostGirl), UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
    }
}
