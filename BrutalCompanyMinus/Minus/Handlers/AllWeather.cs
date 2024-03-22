using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;
using UnityEngine.AI;
using UnityEngine;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    internal class AllWeather
    {
        public static FloodWeather spawnedFloodedWeather = null;
        public static FieldInfo floodLevelOffset = typeof(FloodWeather).GetField("floodLevelOffset", BindingFlags.Instance | BindingFlags.NonPublic);
        public static float floodYLevelOffset = -5.0f;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoundManager), "SpawnOutsideHazards")]
        private static void OnSpawnOutsideHazards()
        {
            if (!Events.AllWeather.Active || RoundManager.Instance.currentLevel.currentWeather == LevelWeatherType.Rainy) return;
            if(RoundManager.Instance.IsHost) Net.Instance.SpawnMudPilesOutsideServerRpc(UnityEngine.Random.Range(8, 20));
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(FloodWeather), "OnGlobalTimeSync")]
        private static void OnSetToCurrentLevelWeather()
        {
            if (!Events.AllWeather.Active || RoundManager.Instance.currentLevel.currentWeather == LevelWeatherType.Flooded || spawnedFloodedWeather == null) return;
            floodLevelOffset.SetValue(spawnedFloodedWeather, (float)floodLevelOffset.GetValue(spawnedFloodedWeather) + floodYLevelOffset);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FloodWeather), "OnEnable")]
        private static void OnOnEnable()
        {
            if (!Events.AllWeather.Active || RoundManager.Instance.currentLevel.currentWeather == LevelWeatherType.Flooded || spawnedFloodedWeather == null) return;
            floodLevelOffset.SetValue(spawnedFloodedWeather, floodYLevelOffset);
        }
    }
}
