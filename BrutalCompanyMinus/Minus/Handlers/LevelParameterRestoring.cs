using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    internal class LevelParameterRestoring
    {
        internal static List<SpawnableItemWithRarity> levelScrap = new List<SpawnableItemWithRarity>();
        internal static int MinScrap = 0;
        internal static int MaxScrap = 0;

        internal static List<SpawnableEnemyWithRarity> insideEnemies = new List<SpawnableEnemyWithRarity>();
        internal static List<SpawnableEnemyWithRarity> outsideEnemies = new List<SpawnableEnemyWithRarity>();
        internal static List<SpawnableEnemyWithRarity> daytimeEnemies = new List<SpawnableEnemyWithRarity>();

        public static void StoreUnmodifiedParamaters(SelectableLevel currentLevel)
        {
            Log.LogInfo(string.Format("Storing un-modified level paramaters on level:{0}", currentLevel.name));
            // Store parameters before any changes made
            levelScrap.Clear(); levelScrap.AddRange(currentLevel.spawnableScrap);
            MinScrap = currentLevel.minScrap;
            MaxScrap = currentLevel.maxScrap;

            insideEnemies.Clear(); insideEnemies.AddRange(currentLevel.Enemies);
            outsideEnemies.Clear(); outsideEnemies.AddRange(currentLevel.OutsideEnemies);
            daytimeEnemies.Clear(); daytimeEnemies.AddRange(currentLevel.DaytimeEnemies);
        }
        

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RoundManager), "waitForScrapToSpawnToSync")]
        private static void OnwaitForScrapToSpawnToSync(ref RoundManager __instance)
        {
            Log.LogInfo(string.Format("Restoring un-modified level paramaters on level:{0}", __instance.currentLevel.name));

            // Restore paramaters
            __instance.currentLevel.Enemies.Clear(); __instance.currentLevel.Enemies.AddRange(insideEnemies);
            __instance.currentLevel.OutsideEnemies.Clear(); __instance.currentLevel.OutsideEnemies.AddRange(outsideEnemies);
            __instance.currentLevel.DaytimeEnemies.Clear(); __instance.currentLevel.DaytimeEnemies.AddRange(daytimeEnemies);

            __instance.currentLevel.spawnableScrap.Clear(); __instance.currentLevel.spawnableScrap.AddRange(levelScrap);
            __instance.currentLevel.minScrap = MinScrap;
            __instance.currentLevel.maxScrap = MaxScrap;
        }
    }
}
