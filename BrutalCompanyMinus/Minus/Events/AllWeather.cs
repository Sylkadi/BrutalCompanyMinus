using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using HarmonyLib;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;

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
        }

        public override void Execute()
        {
            Net.Instance.SetAllWeatherActiveServerRpc(true);

            Net.Instance.SpawnAllWeatherServerRpc();

            Manager.SetAtmosphere(Assets.AtmosphereNameList[Assets.AtmosphereName.Exclipsed], true);
            Manager.SetAtmosphere(Assets.AtmosphereNameList[Assets.AtmosphereName.Flooded], true);
            Manager.SetAtmosphere(Assets.AtmosphereNameList[Assets.AtmosphereName.Rainy], true);
            Manager.SetAtmosphere(Assets.AtmosphereNameList[Assets.AtmosphereName.Stormy], true);
        }

        public override void OnShipLeave() => Net.Instance.SetAllWeatherActiveServerRpc(false);

        public override void OnGameStart() => Active = false;
    }
}
