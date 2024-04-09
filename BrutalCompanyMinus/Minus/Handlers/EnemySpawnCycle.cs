using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using HarmonyLib;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    public class EnemySpawnCycle : NetworkBehaviour
    {
        public static EnemySpawnCycle Instance;

        public static bool Active = false;

        private List<SpawnCycle> spawnCycles;

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

                foreach(EnemySpawnInfo spawnInfo in cycle.enemies)
                {
                    spawnInfo.AttemptSpawn();
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartOfRound), "ShipLeave")]
        private static void OnShipLeave()
        {
            Active = false;
            Instance.spawnCycles.Clear();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.openingDoorsSequence))]
        private static void OnGameStart()
        {
            Active = true;
            Instance.spawnCycles.Clear();
        }

        public enum SpawnLocation
        {
            Inside, Outside
        }

        public class SpawnCycle
        {
            // Enemy

            public EnemySpawnInfo[] enemies;

            public int nothingWeight;

            public float spawnCycleDuration, spawnAttemptInterval;

            private float currentSpawnAttemptIntervalTime;

            public int seed;
            
            private System.Random rng;

            public bool Ellapse(float deltaTime)
            {
                spawnCycleDuration -= deltaTime;
                if(currentSpawnAttemptIntervalTime > 0.0f)
                {
                    currentSpawnAttemptIntervalTime -= deltaTime;
                } else
                {
                    currentSpawnAttemptIntervalTime = spawnAttemptInterval;
                    return true;
                }
                return false;
            }
        }

        public class EnemySpawnInfo
        {
            public GameObject enemy;

            public int enemyWeight, nothingWeight;

            public int spawnCap;

            public SpawnLocation spawnLocation;

            private int currentSpawned;

            public void AttemptSpawn()
            {
                if (UnityEngine.Random.Range(0, enemyWeight + nothingWeight) > nothingWeight)
                {
                    if (currentSpawned >= spawnCap) return;
                    currentSpawned++;

                    switch (spawnLocation)
                    {
                        case SpawnLocation.Inside:
                            Manager.Spawn.InsideEnemies(enemy, 1);
                            Manager.Spawn.DoSpawnInsideEnemies();
                            break;
                        case SpawnLocation.Outside:
                            Manager.Spawn.OutsideEnemies(enemy, 1);
                            Manager.Spawn.DoSpawnOutsideEnemies();
                            break;
                    }
                }
            }
        }
    }
}
