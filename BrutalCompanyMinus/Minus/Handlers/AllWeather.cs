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
    internal class AllWeather // I hate this code
    {
        public static FloodWeather spawnedFloodedWeather = null;
        public static FieldInfo floodLevelOffset = typeof(FloodWeather).GetField("floodLevelOffset", BindingFlags.Instance | BindingFlags.NonPublic);

        public static float floodVariable1 = 1.0f, floodVariable2 = 1.0f, lightningVariable1 = 1.0f, LightningVariable2 = 1.0f;

        public static bool raining = false;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoundManager), "SpawnOutsideHazards")]
        private static void OnSpawnOutsideHazards()
        {
            if (!Events.AllWeather.Active || RoundManager.Instance.currentLevel.currentWeather == LevelWeatherType.Rainy || !raining) return;
            if(RoundManager.Instance.IsHost) Net.Instance.SpawnMudPilesOutsideServerRpc(UnityEngine.Random.Range(8, 16));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FloodWeather), "OnGlobalTimeSync")]
        private static void OnGlobalTimeSync(ref FloodWeather __instance)
        {
            if (!Events.AllWeather.Active || RoundManager.Instance.currentLevel.currentWeather == LevelWeatherType.Flooded || spawnedFloodedWeather == null) return;
            floodLevelOffset.SetValue(spawnedFloodedWeather, (Mathf.Clamp(TimeOfDay.Instance.globalTime / 1080f, 0f, 100f) * floodVariable2) + floodVariable1 - 1.0f);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FloodWeather), "OnEnable")]
        private static void OnOnEnable(ref FloodWeather __instance)
        {
            if (!Events.AllWeather.Active || RoundManager.Instance.currentLevel.currentWeather == LevelWeatherType.Flooded || spawnedFloodedWeather == null) return;
            __instance.transform.position = new Vector3(0.0f, floodVariable1, 0.0f);
            floodLevelOffset.SetValue(spawnedFloodedWeather, (Mathf.Clamp(TimeOfDay.Instance.globalTime / 1080f, 0f, 100f) * floodVariable2) + floodVariable1 - 1.0f);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RoundManager), "SetToCurrentLevelWeather")]
        private static void OnSetToCurrentLevelWeather()
        {
            if (!Events.AllWeather.Active || RoundManager.Instance.currentLevel.currentWeather == LevelWeatherType.Stormy) return;
            TimeOfDay.Instance.currentWeatherVariable = lightningVariable1;
            TimeOfDay.Instance.currentWeatherVariable = LightningVariable2;
        }
    }
}
