using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NutSlayer : MEvent
    {
        public override string Name() => nameof(NutSlayer);

        public override void Initalize()
        {
            Weight = 1;
            Description = "The nut slayer is inside the facility...";
            ColorHex = "#280000";
            Type = EventType.VeryBad;

            EventsToSpawnWith = new List<string>() { nameof(Gloomy) };
            EventsToRemove = new List<string>() { nameof(HeavyRain) };

            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(1.0f, 0.0f));
        }
        public override void Execute() => Manager.Spawn.InsideEnemies(Assets.nutSlayer, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
    }
}
