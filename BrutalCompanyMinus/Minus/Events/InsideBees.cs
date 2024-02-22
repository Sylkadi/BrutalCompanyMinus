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

            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(2.0f, 0.05f, 2.0f, 5.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(3.0f, 0.084f, 3.0f, 8.0f));
        }

        public override void Execute() => Manager.Spawn.InsideEnemies(Assets.GetEnemy(Assets.EnemyName.CircuitBee), UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1), 10.0f);
    }
}
