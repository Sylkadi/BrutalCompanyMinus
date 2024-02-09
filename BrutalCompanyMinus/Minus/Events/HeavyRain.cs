using BrutalCompanyMinus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class HeavyRain : MEvent
    {
        public override string Name() => nameof(HeavyRain);

        public override void Initalize()
        {
            Weight = 8;
            Description = "Id rather stay inside";
            ColorHex = "#FFFFFF";
            Type = EventType.Neutral;

            EventsToRemove = new List<string>() { nameof(Raining), nameof(Gloomy) };
        }

        public override void Execute() {
            Manager.SetAtmosphere(Assets.AtmosphereNameList[Assets.AtmosphereName.Rainy], true);
            Manager.SetAtmosphere(Assets.AtmosphereNameList[Assets.AtmosphereName.Flooded], true);
            Manager.SetAtmosphere(Assets.AtmosphereNameList[Assets.AtmosphereName.Stormy], true);
        }
    }
}
