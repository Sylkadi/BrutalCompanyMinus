using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class InsideBees : MEvent
    {
        public override string Name() => nameof(InsideBees);

        public override void Initalize()
        {
            Weight = 1;
            Description = "BEES!! wait...";
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToSpawnWith = new List<string>() { nameof(Bees) };

            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(2.0f, 0.05f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(3.0f, 0.08f));
        }

        public override void Execute() => Manager.Spawn.InsideEnemies(Assets.GetEnemy(Assets.EnemyName.CircuitBee), UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy)), 10.0f);
    }
}
