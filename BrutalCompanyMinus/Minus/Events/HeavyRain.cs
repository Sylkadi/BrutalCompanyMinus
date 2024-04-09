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

        public static HeavyRain Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 8;
            Descriptions = new List<string>() { "Id rather stay inside", "Even an umbrella wont stop this", "Monsoon!" };
            ColorHex = "#FFFFFF";
            Type = EventType.Neutral;

            EventsToRemove = new List<string>() { nameof(Raining), nameof(Gloomy) };
        }

        public override void Execute() {
            Manager.SetAtmosphere(Assets.AtmosphereName.Rainy, true);
            Manager.SetAtmosphere(Assets.AtmosphereName.Flooded, true);
            Manager.SetAtmosphere(Assets.AtmosphereName.Stormy, true);
        }
    }
}
