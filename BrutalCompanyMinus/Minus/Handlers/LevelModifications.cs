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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartOfRound), "Start")]
        private static void onStartOfRoundStart()
        {
            Log.LogInfo("Executing OnGameStart() for all events");
            foreach(MEvent e in EventManager.events)
            {
                e.OnGameStart();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartOfRound), "ShipLeave")]
        public static void OnShipLeave()
        {
            Log.LogInfo("Executing OnShipLeave() for current events.");
            foreach (MEvent e in EventManager.currentEvents)
            {
                e.OnShipLeave();
            }
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

            Log.LogInfo(string.Format("Storing un-modified level paramaters on level:{0}", __instance.currentLevel.name));
            // Store parameters before any changes made
            insideEnemies.Clear(); insideEnemies.AddRange(__instance.currentLevel.Enemies);
            outsideEnemies.Clear(); outsideEnemies.AddRange(__instance.currentLevel.OutsideEnemies);
            daytimeEnemies.Clear(); daytimeEnemies.AddRange(__instance.currentLevel.DaytimeEnemies);

            Log.LogInfo("Resetting level values before changing.");

            foreach (SpawnableMapObject obj in __instance.currentLevel.spawnableMapObjects)
            {
                if (obj.prefabToSpawn.name == "Landmine") obj.numberToSpawn = new AnimationCurve(new Keyframe(0, 3.0f));
                if (obj.prefabToSpawn.name == "TurretContainer") obj.numberToSpawn = new AnimationCurve(new Keyframe(0, 3.0f));
            }

            // Reset spawn chances
            __instance.currentLevel.enemySpawnChanceThroughoutDay.ClearKeys();
            __instance.currentLevel.outsideEnemySpawnChanceThroughDay.ClearKeys();
            __instance.currentLevel.daytimeEnemySpawnChanceThroughDay.ClearKeys();
            foreach (Keyframe key in Assets.insideSpawnChanceCurves[Manager.GetLevelIndex()].keys) __instance.currentLevel.enemySpawnChanceThroughoutDay.AddKey(key);
            foreach (Keyframe key in Assets.outsideSpawnChanceCurves[Manager.GetLevelIndex()].keys) __instance.currentLevel.outsideEnemySpawnChanceThroughDay.AddKey(key);
            foreach (Keyframe key in Assets.daytimeSpawnChanceCurves[Manager.GetLevelIndex()].keys) __instance.currentLevel.daytimeEnemySpawnChanceThroughDay.AddKey(key);

            Events.GrabbableLandmines.LandmineDisabled = false;
            foreach (MEvent e in EventManager.events) e.Executed = false;

            RoundManager.Instance.currentLevel.maxEnemyPowerCount = Assets.insideMaxPowerCounts[Manager.GetLevelIndex()];
            RoundManager.Instance.currentLevel.maxOutsideEnemyPowerCount = Assets.outsideMaxPowerCounts[Manager.GetLevelIndex()];
            RoundManager.Instance.currentLevel.maxDaytimeEnemyPowerCount = Assets.daytimeMaxPowerCounts[Manager.GetLevelIndex()];

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

            int objectCount = spawnedScrap.Length;
            List<Vector3> oldScrapPositions = new List<Vector3>();

            foreach(NetworkObjectReference obj in spawnedScrap) // Despawn old objects
            {
                if(obj.TryGet(out NetworkObject netObj))
                {
                    oldScrapPositions.Add(netObj.transform.position);
                    netObj.Despawn(destroy: true);
                } else
                {
                    objectCount--;
                    Log.LogError("Item has no networkObject, not destroying.");
                }
            }

            // Create new
            List<NetworkObjectReference> newNetObjects = new List<NetworkObjectReference>();
            List<int> newScrapValues = new List<int>();

            List<int> weights = new List<int>();
            foreach(SpawnableItemWithRarity item in Manager.ScrapToTransmuteTo) weights.Add(item.rarity);

            for(int i = 0; i < objectCount; i++)
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
