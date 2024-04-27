using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    internal class LevelModifications
    {
        internal static List<SpawnableEnemyWithRarity> insideEnemies = new List<SpawnableEnemyWithRarity>();
        internal static List<SpawnableEnemyWithRarity> outsideEnemies = new List<SpawnableEnemyWithRarity>();
        internal static List<SpawnableEnemyWithRarity> daytimeEnemies = new List<SpawnableEnemyWithRarity>();
        internal static List<SpawnableMapObject> spawnableMapObjects = new List<SpawnableMapObject>();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartOfRound), "Start")]
        private static void onStartOfRoundStart()
        {
            if (!Configuration.Initalized) return;
            EventManager.forcedEvents.Clear();
            UI.canClearText = true;
            EventManager.ExecuteOnGameStart();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartOfRound), "ShipLeave")]
        public static void OnShipLeave()
        {
            EventManager.ExecuteOnShipLeave();
            EventManager.currentEvents.Clear();

            Net.Instance.ClearGameObjectsClientRpc(); // Clear all previously placed objects on all clients

            Log.LogInfo("Restoring un-modified level enemy spawns on current level.");
            // Restore parameters
            RoundManager.Instance.currentLevel.Enemies.Clear(); RoundManager.Instance.currentLevel.Enemies.AddRange(insideEnemies);
            RoundManager.Instance.currentLevel.OutsideEnemies.Clear(); RoundManager.Instance.currentLevel.OutsideEnemies.AddRange(outsideEnemies);
            RoundManager.Instance.currentLevel.DaytimeEnemies.Clear(); RoundManager.Instance.currentLevel.DaytimeEnemies.AddRange(daytimeEnemies);
        }

        public static void ResetValues(StartOfRound __instance) // Reset values
        {
            if (!RoundManager.Instance.IsHost || __instance.currentLevel.levelID == 3) return;

            Manager.currentLevel = __instance.currentLevel;
            int levelIndex = Manager.GetLevelIndex();

            Log.LogInfo(string.Format("Storing un-modified level paramaters on level:{0}", __instance.currentLevel.name));
            // Store parameters before any changes made
            insideEnemies.Clear(); insideEnemies.AddRange(__instance.currentLevel.Enemies);
            outsideEnemies.Clear(); outsideEnemies.AddRange(__instance.currentLevel.OutsideEnemies);
            daytimeEnemies.Clear(); daytimeEnemies.AddRange(__instance.currentLevel.DaytimeEnemies);

            Log.LogInfo("Resetting level values before changing.");

            __instance.currentLevel.spawnableMapObjects = Assets.spawnableMapObjects[levelIndex];

            // Reset spawn chances
            __instance.currentLevel.enemySpawnChanceThroughoutDay.ClearKeys();
            __instance.currentLevel.outsideEnemySpawnChanceThroughDay.ClearKeys();
            __instance.currentLevel.daytimeEnemySpawnChanceThroughDay.ClearKeys();
            foreach (Keyframe key in Assets.insideSpawnChanceCurves[levelIndex].keys) __instance.currentLevel.enemySpawnChanceThroughoutDay.AddKey(key);
            foreach (Keyframe key in Assets.outsideSpawnChanceCurves[levelIndex].keys) __instance.currentLevel.outsideEnemySpawnChanceThroughDay.AddKey(key);
            foreach (Keyframe key in Assets.daytimeSpawnChanceCurves[levelIndex].keys) __instance.currentLevel.daytimeEnemySpawnChanceThroughDay.AddKey(key);

            Events.GrabbableLandmines.LandmineDisabled = false;
            foreach (MEvent e in EventManager.events) e.Executed = false;

            RoundManager.Instance.currentLevel.maxEnemyPowerCount = Assets.insideMaxPowerCounts[levelIndex];
            RoundManager.Instance.currentLevel.maxOutsideEnemyPowerCount = Assets.outsideMaxPowerCounts[levelIndex];
            RoundManager.Instance.currentLevel.maxDaytimeEnemyPowerCount = Assets.daytimeMaxPowerCounts[levelIndex];

            // Reset bonus hp
            Manager.bonusEnemyHp = 0;
            Manager.spawnChanceMultiplier = 1.0f;
            Manager.spawncapMultipler = 1.0f;
            Manager.bonusMaxInsidePowerCount = 0;
            Manager.bonusMaxOutsidePowerCount = 0;

            // Reset multipliers
            try
            {
                Manager.currentLevel.factorySizeMultiplier = Assets.factorySizeMultiplierList[__instance.currentLevel.levelID];
            } catch
            {
                Manager.currentLevel.factorySizeMultiplier = 1f;
            }
            Manager.scrapAmountMultiplier = 1.0f;
            Manager.scrapValueMultiplier = 1.0f;
            Manager.randomItemsToSpawnOutsideCount = 0;
            Manager.transmuteScrap = false;
            Manager.ScrapToTransmuteTo.Clear();

            // Reset objectSpawnLists
            Manager.insideObjectsToSpawnOutside.Clear();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoundManager), "waitForScrapToSpawnToSync")]
        public static void OnwaitForScrapToSpawnToSync(ref NetworkObjectReference[] spawnedScrap, ref int[] scrapValues) // Scrap transmutation + Scrap multipliers
        {
            if (spawnedScrap.Length == 0) return;

            for(int i = 0; i < scrapValues.Length; i++)
            {
                scrapValues[i] = (int)(scrapValues[i] * Manager.scrapValueMultiplier);
            }

            int scrapDiffernce = (int)(spawnedScrap.Length * (Manager.scrapAmountMultiplier - 1));
            int amountRemoved = 0;
            Manager.ScrapSpawnInfo insideScrap = new Manager.ScrapSpawnInfo(new NetworkObjectReference[] { }, new int[] { });
            if(scrapDiffernce >= 0) // Add more scrap if multiplier >= x1.0
            {
                insideScrap = Manager.Spawn.DoSpawnScrapInside(scrapDiffernce);
            } else // Remove scrap if multiplier < x1.0
            {
                amountRemoved = (int)Mathf.Clamp(scrapDiffernce * -1, 0, scrapValues.Length - 1);
                Log.LogInfo($"Removing {amountRemoved} scrap.");
                for (int i = spawnedScrap.Length - 1; i >= spawnedScrap.Length - amountRemoved; i--)
                {
                    NetworkObject netObject = null;
                    spawnedScrap[i].TryGet(out netObject);
                    if(netObject != null)
                    {
                        netObject.Despawn(destroy: true);
                    }
                }
            }
            Manager.ScrapSpawnInfo outsideScrap = Manager.Spawn.DoSpawnScrapOutside(Manager.randomItemsToSpawnOutsideCount);

            List<NetworkObjectReference> newSpawnedScrapList = new List<NetworkObjectReference>();
            List<int> newScrapValuesList = new List<int>();

            for(int i = 0; i < spawnedScrap.Length - amountRemoved; i++)
            {
                newSpawnedScrapList.Add(spawnedScrap[i]);
                newScrapValuesList.Add(scrapValues[i]);
            }
            for (int i = 0; i < insideScrap.netObjects.Length; i++)
            {
                newSpawnedScrapList.Add(insideScrap.netObjects[i]);
                newScrapValuesList.Add(insideScrap.scrapPrices[i]);
            }
            for (int i = 0; i < outsideScrap.netObjects.Length; i++)
            {
                newSpawnedScrapList.Add(outsideScrap.netObjects[i]);
                newScrapValuesList.Add(outsideScrap.scrapPrices[i]);
            }
            spawnedScrap = newSpawnedScrapList.ToArray();
            scrapValues = newScrapValuesList.ToArray();



            if (!Manager.transmuteScrap) return;
            if(Manager.ScrapToTransmuteTo.Count == 0)
            {
                Log.LogError("ScrapToTransmuteTo Count is 0, returning.");
                return;
            }
            if(Manager.scrapTransmuteAmount.Count == 0)
            {
                Log.LogError("scrapTransmuteAmount Count is 0, returning.");
                return;
            }

            float amount = 0.0f;
            foreach(float scrapTransmuteAmount in Manager.scrapTransmuteAmount) amount += scrapTransmuteAmount;
            amount /= Manager.scrapTransmuteAmount.Count;
            Manager.scrapTransmuteAmount.Clear();
            int scrapToRemove = Mathf.Clamp((int)(spawnedScrap.Length * amount) + 1, 1, spawnedScrap.Length);

            Log.LogInfo($"Transmuting {scrapToRemove} scrap.");

            List<Vector3> oldScrapPositions = new List<Vector3>();
            for (int i = 0; i < scrapToRemove; i++)
            {
                if (!spawnedScrap[i].TryGet(out NetworkObject netObj)) continue;
                oldScrapPositions.Add(netObj.transform.position);
                netObj.Despawn(destroy: true);
            }

            // Create new
            List<NetworkObjectReference> newNetObjects = new List<NetworkObjectReference>();
            List<int> newScrapValues = new List<int>();
            for (int i = scrapToRemove; i < spawnedScrap.Length; i++)
            {
                newNetObjects.Add(spawnedScrap[i]);
                newScrapValues.Add(scrapValues[i]);
            }

            List<int> weights = new List<int>();
            foreach(SpawnableItemWithRarity item in Manager.ScrapToTransmuteTo) weights.Add(item.rarity);

            for(int i = 0; i < scrapToRemove; i++)
            {
                SpawnableItemWithRarity chosenItem = Manager.ScrapToTransmuteTo[RoundManager.Instance.GetRandomWeightedIndexList(weights)];

                GameObject obj = GameObject.Instantiate(chosenItem.spawnableItem.spawnPrefab, Vector3.zero, Quaternion.identity);

                GrabbableObject grabObj = obj.GetComponent<GrabbableObject>();
                NetworkObject netObj = obj.GetComponent<NetworkObject>();
                if (grabObj == null)
                {
                    Log.LogError("chosenItem grabbableObject is null, skipping entry.");
                    continue;
                }
                if (netObj == null)
                {
                    Log.LogError("chosenItem networkObject is null, skipping entry,");
                    continue;
                }

                // Transform
                grabObj.transform.position = oldScrapPositions[i];
                grabObj.transform.rotation = Quaternion.Euler(grabObj.itemProperties.restingRotation);
                grabObj.fallTime = 0.0f;

                // Generate scrap value
                int scrapValue = (int)(UnityEngine.Random.Range(chosenItem.spawnableItem.minValue, chosenItem.spawnableItem.maxValue + 1) * RoundManager.Instance.scrapValueMultiplier * Manager.scrapValueMultiplier);

                newScrapValues.Add(scrapValue);
                grabObj.scrapValue = scrapValue;

                // Spawn object and add to list
                netObj.Spawn();
                newNetObjects.Add(netObj);
            }

            // Replace spawnedScrap, scrapValues
            spawnedScrap = newNetObjects.ToArray();
            scrapValues = newScrapValues.ToArray();
        }
    }
}
