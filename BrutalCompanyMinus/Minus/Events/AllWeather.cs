using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using HarmonyLib;

namespace BrutalCompanyMinus.Minus.Events
{
    [HarmonyPatch]
    internal class AllWeather : MEvent
    {
        public static bool Active = false;

        public override string Name() => nameof(AllWeather);

        public static AllWeather Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Description = "God hates you";
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(Gloomy), nameof(Raining), nameof(HeavyRain) };

            ScaleList.Add(ScaleType.ScrapValue, new Scale(1.60f, 0.0f, 1.65f, 1.60f));
            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.30f, 0.0f, 1.30f, 1.30f));
        }

        public override void Execute()
        {
            Net.Instance.SetAllWeatherActiveServerRpc(true);

            Net.Instance.SpawnAllWeatherServerRpc();

            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);
            Manager.scrapValueMultiplier *= Getf(ScaleType.ScrapValue);

            Manager.SetAtmosphere(Assets.AtmosphereName.Exclipsed, true);
            Manager.SetAtmosphere(Assets.AtmosphereName.Flooded, true);
            Manager.SetAtmosphere(Assets.AtmosphereName.Rainy, true);
            Manager.SetAtmosphere(Assets.AtmosphereName.Stormy, true);
        }

        public override void OnShipLeave() => Net.Instance.SetAllWeatherActiveServerRpc(false);

        public override void OnGameStart() => Active = false;
    }
}
