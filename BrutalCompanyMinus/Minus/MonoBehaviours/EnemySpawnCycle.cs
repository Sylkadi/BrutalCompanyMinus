using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using HarmonyLib;
using System.Linq;
using UnityEngine.Events;
using BepInEx.Configuration;

namespace BrutalCompanyMinus.Minus.MonoBehaviours
{
    [HarmonyPatch]
    public class EnemySpawnCycle : NetworkBehaviour
    {
        public static EnemySpawnCycle Instance;

        public static bool Active = false;

        public List<SpawnCycle> spawnCycles;

        public static SpawnCycle allEnemiesCycle = new SpawnCycle();

        public static SpawnCycle allAllEnemiesCycle = new SpawnCycle();

        private void Awake()
        {
            Instance = this;
            spawnCycles = new List<SpawnCycle>();
        }

        private void Update()
        {
            if (!Active) return;
            foreach (SpawnCycle cycle in spawnCycles)
            {
                if (cycle.spawnCycleDuration <= 0.0f || !cycle.Ellapse(Time.deltaTime)) continue;
                cycle.AttemptSpawnAll();
            }

            if (Configuration.enableAllEnemies.Value && allEnemiesCycle.EllapseOnlyInterval(Time.deltaTime)) allEnemiesCycle.AttemptSpawnAll();
            if (Configuration.enableAllAllEnemies.Value && allAllEnemiesCycle.EllapseOnlyInterval(Time.deltaTime)) allAllEnemiesCycle.AttemptSpawnAll();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartOfRound), "ShipLeave")]
        private static void OnShipLeave()
        {
            Active = false;
            Instance.spawnCycles.Clear();
            foreach (SpawnCycle cycle in Instance.spawnCycles)
            {
                cycle.Reset();
            }
            allEnemiesCycle.Reset();
            allAllEnemiesCycle.Reset();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.openingDoorsSequence))]
        private static void OnGameStart()
        {
            Active = true;
            Instance.spawnCycles.Clear();
            foreach (SpawnCycle cycle in Instance.spawnCycles)
            {
                cycle.Reset();
            }
            allEnemiesCycle.Reset();
            allAllEnemiesCycle.Reset();
        }

        public enum SpawnLocation
        {
            Inside, Outside
        }

        private float GetChanceMultiplier()
        {
            float chanceMultiplier = RoundManager.Instance.currentLevel.enemySpawnChanceThroughoutDay.Evaluate(RoundManager.Instance.timeScript.currentDayTime / RoundManager.Instance.timeScript.totalTime);
            if (chanceMultiplier <= 0.0f)
            {
                return Mathf.Clamp(Mathf.Exp(chanceMultiplier), 0.01f, 1.0f);
            }
            return Mathf.Clamp(Mathf.Log(chanceMultiplier + (float)Math.E), 1.0f, 5.0f);
        }

        public class SpawnCycle
        {
            // Enemy

            public EnemySpawnInfo[] enemies;

            public float spawnCycleDuration, spawnAttemptInterval;

            public float nothingWeight;

            private float currentSpawnAttemptIntervalTime;

            public bool Ellapse(float deltaTime)
            {
                spawnCycleDuration -= deltaTime;
                if (currentSpawnAttemptIntervalTime > 0.0f)
                {
                    currentSpawnAttemptIntervalTime -= deltaTime;
                }
                else
                {
                    currentSpawnAttemptIntervalTime = spawnAttemptInterval;
                    return true;
                }
                return false;
            }

            public bool EllapseOnlyInterval(float deltaTime)
            {
                if (currentSpawnAttemptIntervalTime > 0.0f)
                {
                    currentSpawnAttemptIntervalTime -= deltaTime;
                }
                else
                {
                    currentSpawnAttemptIntervalTime = spawnAttemptInterval;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                currentSpawnAttemptIntervalTime = spawnAttemptInterval * 0.33f;
                foreach (EnemySpawnInfo spawnInfo in enemies) spawnInfo.Reset();
            }

            public void AttemptSpawnAll()
            {
                float spawnChanceMultiplier = Instance.GetChanceMultiplier();
                Log.LogInfo($"--- Attempting to spawn with a nothingWeight of {nothingWeight} and chanceMultiplier of {spawnChanceMultiplier}");
                foreach (EnemySpawnInfo spawnInfo in enemies)
                {
                    spawnInfo.AttemptSpawn(allEnemiesCycle.nothingWeight, spawnChanceMultiplier);
                }
            }
        }

        public class EnemySpawnInfo
        {
            public GameObject enemy;

            public float enemyWeight;

            public int spawnCap;

            public SpawnLocation spawnLocation;

            private int currentSpawned;

            public void AttemptSpawn(float nothingWeight, float chanceMultiplier)
            {
                switch (spawnLocation)
                {
                    case SpawnLocation.Inside:
                        AttemptSpawnInside(nothingWeight, chanceMultiplier);
                        break;
                    case SpawnLocation.Outside:
                        AttemptSpawnOutside(nothingWeight, chanceMultiplier);
                        break;
                }
            }

            private void AttemptSpawnInside(float nothingWeight, float chanceMultiplier)
            {
                float weight = enemyWeight * chanceMultiplier;
                Log.LogInfo($"### Attempting to spawn {enemy.name} at {weight:F2} weight;");
                if (UnityEngine.Random.Range(0, weight + nothingWeight) > nothingWeight)
                {
                    if (currentSpawned >= spawnCap) return;
                    currentSpawned++;

                    Manager.Spawn.InsideEnemies(enemy, 1);
                    Manager.Spawn.DoSpawnInsideEnemies();
                    Log.LogInfo($"!!! Succeeded spawning {enemy.name} inside.");
                    return;
                }
                Log.LogInfo($"Failed to spawn {enemy.name} inside.");
            }

            private void AttemptSpawnOutside(float nothingWeight, float chanceMultiplier)
            {
                float weight = enemyWeight * chanceMultiplier;
                Log.LogInfo($"### Attempting to spawn {enemy.name} at {weight:F2} weight;");
                if (UnityEngine.Random.Range(0, weight + nothingWeight) > nothingWeight)
                {
                    if (currentSpawned >= spawnCap) return;
                    currentSpawned++;

                    Manager.Spawn.OutsideEnemies(enemy, 1);
                    Manager.Spawn.DoSpawnOutsideEnemies();
                    Log.LogInfo($"!!! Succeeded spawning {enemy.name} outside.");
                    return;
                }
                Log.LogInfo($"Failed to spawn {enemy.name} outside.");
            }

            public void Reset() => currentSpawned = 0;
        }
    }
}
