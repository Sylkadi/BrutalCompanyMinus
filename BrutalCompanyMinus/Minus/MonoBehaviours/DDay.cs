using Discord;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.MonoBehaviours
{
    [HarmonyPatch]
    internal class DDay : NetworkBehaviour
    {
        public static float currentTime = 0;
        public static float bombardmentCurrentTime = 0;
        public static float fireCurrentTime = 0;

        public static float bombardmentInterval = 100;
        public static float bombardmentTime = 15;

        public static float fireInterval = 1;
        public static int fireAmount = 8;

        public static bool displayedBombardmentWarning = false;
        public static bool displayWarning = true;

        private static float fireAmountMultiplier = 1.0f;

        private static int seed = 2352;

        public static DDay instance;

        public AudioSource sirensClose;

        public AudioSource sirensFar;

        public Transform transform;

        public static float volume = 0.3f;

        private List<Vector3> spawnDenialNodes = new List<Vector3>();

        public void Start()
        {
            if (instance != null) DestroyInstance();
            instance = this;
            spawnDenialNodes = Helper.GetSpawnDenialNodes();

            currentTime = 15.0f;

            sirensClose.volume = volume;
            sirensFar.volume = volume;
        }

        public void Update() // I honestly dont even know how it's possible for this to persist... It dosen't make any sense to me, how is this even possible.....
        {
            if (!Events.Warzone.Active || !RoundManager.Instance.IsHost) return;
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                float fireAmountMultiplier = Mathf.Clamp(Manager.terrainArea / 9700.0f, 1.0f, 3.0f);

                currentTime = bombardmentInterval;
                bombardmentCurrentTime = bombardmentTime;

                displayedBombardmentWarning = false;
            }

            if (currentTime <= 15 && !displayedBombardmentWarning)
            {
                instance.PlayServerRpc();
                if (displayWarning) Net.Instance.DisplayTipServerRpc("BOMBARDMENT IN 15 SECONDS", "TAKE COVER!!!", true);
                displayedBombardmentWarning = true;
            }

            if (bombardmentCurrentTime > 0)
            {
                bombardmentCurrentTime -= Time.deltaTime;
                fireCurrentTime -= Time.deltaTime;
            }

            if (fireCurrentTime < 0)
            {
                fireCurrentTime = fireInterval;

                seed++;
                System.Random rng = new System.Random(seed);

                // Fire
                for (int i = 0; i < fireAmount * fireAmountMultiplier; i++)
                {
                    for (int j = 0; j < 3; j++) // 3 Attempts at safe position
                    {
                        rng = new System.Random(seed++);

                        Vector3 at = Manager.outsideObjectSpawnNodes[rng.Next(Manager.outsideObjectSpawnNodes.Count)];
                        at += new Vector3(rng.Next(-75, 75), 0, rng.Next(-75, 75));

                        Vector3 from = at + new Vector3(rng.Next(-100, 100), rng.Next(500, 800), rng.Next(-100, 100));

                        RaycastHit hit = new RaycastHit();
                        if (Physics.Raycast(new Ray(from, (at - from).normalized), out hit)) at = hit.point;

                        bool isSafe = true;
                        foreach (Vector3 denialNode in spawnDenialNodes)
                        {
                            if (Vector3.Distance(denialNode, at) < 15.0f)
                            {
                                isSafe = false;
                                break;
                            }
                        }

                        if (isSafe) ArtilleryShell.FireAt(at, from);
                        break;
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void PlayServerRpc() => PlayClientRpc();

        [ClientRpc]
        public void PlayClientRpc()
        {
            sirensClose.Play();
            sirensFar.Play();
        }

        [ServerRpc(RequireOwnership = false)]
        public void StopServerRpc() => StopClientRpc();

        [ClientRpc]
        public void StopClientRpc()
        {
            sirensClose.Stop();
            sirensFar.Stop();
        }

        public static void SpawnInstance()
        {
            Vector3 position = Vector3.zero;
            GameObject ship = GameObject.Find("HangarShip");

            if (ship != null) position = ship.transform.position; // This dosen't get corret position i want but im too lazy anyways...

            GameObject sirens = Instantiate(Assets.artillerySirens, position, Quaternion.identity);
            sirens.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);

            Manager.objectsToClear.Add(sirens);
        }

        public static void DestroyInstance()
        {
            Events.Warzone.Active = false;
            try
            {
                NetworkObject netObject = instance.transform.GetComponent<NetworkObject>();
                netObject.Despawn(true);
            } catch
            {

            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.EndGameServerRpc))]
        public static void DestroyForGodsSake()
        {
            Events.Warzone.Active = false;
            DestroyInstance();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartOfRound), "OnShipLandedMiscEvents")]
        public static void OnShipLanded()
        {
            if (!RoundManager.Instance.IsHost || !Events.Warzone.Active) return;
            SpawnInstance();
        }
    }
}
