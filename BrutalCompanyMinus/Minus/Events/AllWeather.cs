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
            Descriptions = new List<string>() { "God hates you", "Chaos reigns as the skies unleash every weather pattern known to man.", "An umbrella might help here", "I hope you suffer here" };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(Gloomy), nameof(Raining), nameof(HeavyRain) };

            ScaleList.Add(ScaleType.ScrapValue, new Scale(1.60f, 0.0f, 1.60f, 1.60f));
            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.30f, 0.0f, 1.30f, 1.30f));
        }

        public override bool AddEventIfOnly() => RoundManager.Instance.currentLevel.randomWeathers != null && RoundManager.Instance.currentLevel.randomWeathers.Length >= 2;

        public override void Execute()
        {
            Net.Instance.SetAllWeatherActiveServerRpc(true);

            Net.Instance.SpawnAllWeatherServerRpc(Net.Instance._seed++);

            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);
            Manager.scrapValueMultiplier *= Getf(ScaleType.ScrapValue);

            if (RoundManager.Instance.currentLevel.randomWeathers == null) return;

            foreach(RandomWeatherWithVariables randomWeather in RoundManager.Instance.currentLevel.randomWeathers)
            {
                if (randomWeather.weatherType == RoundManager.Instance.currentLevel.currentWeather) continue;
                switch(randomWeather.weatherType)
                {
                    case LevelWeatherType.Rainy:
                        Manager.SetAtmosphere(Assets.AtmosphereName.Rainy, true);
                        break;
                    case LevelWeatherType.Foggy:
                        Manager.SetAtmosphere(Assets.AtmosphereName.Foggy, true);
                        break;
                    case LevelWeatherType.Stormy:
                        Manager.SetAtmosphere(Assets.AtmosphereName.Stormy, true);
                        break;
                    case LevelWeatherType.Flooded:
                        Manager.SetAtmosphere(Assets.AtmosphereName.Flooded, true);
                        break;
                    case LevelWeatherType.Eclipsed:
                        Manager.SetAtmosphere(Assets.AtmosphereName.Exclipsed, true);
                        break;
                }
            }
        }

        public override void OnShipLeave() => Net.Instance.SetAllWeatherActiveServerRpc(false);

        public override void OnGameStart() => Active = false;
    }
}
