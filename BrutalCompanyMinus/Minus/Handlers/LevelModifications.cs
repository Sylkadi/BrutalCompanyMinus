using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
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

        private static bool modifiedEnemySpawns = false;
        public static void ModifyEnemyScrapSpawns(StartOfRound instance)
        {
            if (modifiedEnemySpawns || !Configuration.enableCustomWeights.Value) return;
            if (!Configuration.customEnemyWeights.Value && !Configuration.customScrapWeights.Value) return;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Log.LogInfo("Modifying enemy map pool and scrap pool from config.");

            // Multi-thread this shit
            Parallel.For(0, instance.levels.Length, i =>
            {
                Log.LogInfo(string.Format("Modifying enemy map pool and scrap pool for {0} using config settings.", instance.levels[i].name));

                if(Configuration.insideEnemyRarityList.TryGetValue(instance.levels[i].name, out _))
                {
                    if(Configuration.customEnemyWeights.Value) instance.levels[i].Enemies.Clear();
                    foreach (KeyValuePair<string, int> insideEnemy in Configuration.insideEnemyRarityList[instance.levels[i].name])
                    {
                        EnemyType enemy = Assets.GetEnemy(insideEnemy.Key);
                        int rarity = insideEnemy.Value;
                        if (Configuration.enableAllEnemies.Value && rarity == 0 && !enemy.isOutsideEnemy) rarity = Configuration.allEnemiesDefaultWeight.Value;
                        if (rarity == 0) continue; // Skip Entry
                        instance.levels[i].Enemies.Add(new SpawnableEnemyWithRarity() { enemyType = enemy, rarity = rarity });
                    }
                } else
                {
                    Log.LogError(string.Format("Level {0} dosen't exist in dictionaries for insideEnemyRarirtyList, skipping.", instance.levels[i].name));
                }

                if (Configuration.outsideEnemyRarityList.TryGetValue(instance.levels[i].name, out _))
                {
                    if (Configuration.customEnemyWeights.Value) instance.levels[i].OutsideEnemies.Clear();
                    foreach (KeyValuePair<string, int> outsideEnemy in Configuration.outsideEnemyRarityList[instance.levels[i].name])
                    {
                        EnemyType enemy = Assets.GetEnemy(outsideEnemy.Key);
                        int rarity = outsideEnemy.Value;
                        if (Configuration.enableAllEnemies.Value && rarity == 0 && enemy.isOutsideEnemy) rarity = Configuration.allEnemiesDefaultWeight.Value;
                        if (outsideEnemy.Value == 0) continue; // Skip Entry
                        instance.levels[i].OutsideEnemies.Add(new SpawnableEnemyWithRarity() { enemyType = enemy, rarity = rarity });
                    }
                } else
                {
                    Log.LogError(string.Format("Level {0} dosen't exist in dictionaries for outsideEnemyRarirtyList, skipping.", instance.levels[i].name));
                }

                if (Configuration.daytimeEnemyRarityList.TryGetValue(instance.levels[i].name, out _))
                {
                    if (Configuration.customEnemyWeights.Value) instance.levels[i].DaytimeEnemies.Clear();
                    foreach (KeyValuePair<string, int> daytimeEnemy in Configuration.daytimeEnemyRarityList[instance.levels[i].name])
                    {
                        EnemyType enemy = Assets.GetEnemy(daytimeEnemy.Key);
                        int rarity = daytimeEnemy.Value;
                        if (Configuration.enableAllEnemies.Value && rarity == 0 && enemy.isDaytimeEnemy) rarity = Configuration.allEnemiesDefaultWeight.Value;
                        if (daytimeEnemy.Value == 0) continue; // Skip Entry
                        instance.levels[i].DaytimeEnemies.Add(new SpawnableEnemyWithRarity() { enemyType = enemy, rarity = rarity });
                    }
                } else
                {
                    Log.LogError(string.Format("Level {0} dosen't exist in dictionaries for daytimeEnemyRarirtyList, skipping.", instance.levels[i].name));
                }

                if (Configuration.scrapRarityList.TryGetValue(instance.levels[i].name, out _))
                {
                    if (Configuration.customScrapWeights.Value) instance.levels[i].spawnableScrap.Clear();
                    foreach (KeyValuePair<string, int> scrap in Configuration.scrapRarityList[instance.levels[i].name])
                    {
                        Item item = Assets.GetItem(scrap.Key);
                        int rarity = scrap.Value;
                        if (Configuration.enableAllScrap.Value && rarity == 0) rarity = Configuration.allScrapDefaultWeight.Value;
                        if (scrap.Value == 0) continue; // Skip Entry
                        instance.levels[i].spawnableScrap.Add(new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(scrap.Key), rarity = scrap.Value });

                    }
                } else
                {
                    Log.LogError(string.Format("Level {0} dosen't exist in dictionaries for scrapRarityList, skipping.", instance.levels[i].name));
                }
            });

            stopWatch.Stop();
            Log.LogInfo(string.Format("Took {0}ms", stopWatch.ElapsedMilliseconds));

            modifiedEnemySpawns = true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartOfRound), "Start")]
        private static void onStartOfRoundStart()
        {
            Log.LogInfo("Executing OnGameStart() for all events");
            foreach(MEvent e in EventManager.events)
            {
                e.OnGameStart();
            }

            modifiedEnemySpawns = false;
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

            RoundManager.Instance.currentLevel.maxEnemyPowerCount = Assets.insideMaxPowerCounts[Manager.GetLevelIndex()];
            RoundManager.Instance.currentLevel.maxOutsideEnemyPowerCount = Assets.outsideMaxPowerCounts[Manager.GetLevelIndex()];
            RoundManager.Instance.currentLevel.maxDaytimeEnemyPowerCount = Assets.daytimeMaxPowerCounts[Manager.GetLevelIndex()];

            // Reset bonus hp
            Manager.bonusEnemyHp = 0;
            Manager.spawnChanceMultiplier = 1.0f;
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
            Manager.scrapValueMultiplier = 0.4f; // Default value is 0.4 not 1.0
            Manager.randomItemsToSpawnOutsideCount = 0;

            Manager.transmuteScrap = false;
            Manager.ScrapToTransmuteTo.Clear();

            // Reset objectSpawnLists
            Manager.insideObjectsToSpawnOutside.Clear();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoundManager), "waitForScrapToSpawnToSync")]
        public static void OnwaitForScrapToSpawnToSync(ref NetworkObjectReference[] spawnedScrap, ref int[] scrapValues) // Scrap transmutation
        {
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
                int scrapValue = (int)(UnityEngine.Random.Range(chosenItem.spawnableItem.minValue, chosenItem.spawnableItem.maxValue + 1) * RoundManager.Instance.scrapValueMultiplier);

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
