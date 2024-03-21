using BrutalCompanyMinus.Minus.Events;
using Discord;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Handlers
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

        public void Start()
        {
            if(instance != null) DestroyInstance();
            instance = this;

            currentTime = 15.0f;

            sirensClose.volume = volume;
            sirensFar.volume = volume;
        }

        public void Update()
        {
            if (!Events.Warzone.Active || !RoundManager.Instance.IsHost) return;
            if(currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            } else
            {
                float fireAmountMultiplier = Mathf.Clamp(Manager.terrainArea / 9700.0f, 1.0f, 3.0f);

                currentTime = bombardmentInterval;
                bombardmentCurrentTime = bombardmentTime;

                displayedBombardmentWarning = false;
            }

            if(currentTime <= 15 && !displayedBombardmentWarning)
            {
                DDay.instance.PlayServerRpc();
                if(displayWarning) Net.Instance.DisplayTipServerRpc("BOMBARDMENT IN 15 SECONDS", "TAKE COVER!!!", true);
                displayedBombardmentWarning = true;
            }

            if(bombardmentCurrentTime > 0)
            {
                bombardmentCurrentTime -= Time.deltaTime;
                fireCurrentTime -= Time.deltaTime;
            }

            if(fireCurrentTime < 0)
            {
                fireCurrentTime = fireInterval;

                seed++;
                System.Random rng = new System.Random(seed);

                // Fire
                for (int i = 0; i < fireAmount * fireAmountMultiplier; i++)
                {
                    Vector3 at = Manager.outsideObjectSpawnNodes[rng.Next(Manager.outsideObjectSpawnNodes.Count)];
                    at += new Vector3(rng.Next(-75, 75), 0, rng.Next(-75, 75));

                    Vector3 from = at + new Vector3(rng.Next(-100, 100), rng.Next(500, 800), rng.Next(-100, 100));

                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(new Ray(from, (at - from).normalized), out hit)) at = hit.point;

                    ArtilleryShell.FireAt(at, from);
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

            GameObject sirens = GameObject.Instantiate(Assets.artillerySirens, position, Quaternion.identity);
            sirens.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
        }

        public static void DestroyInstance()
        {
            if (instance == null) return;

            NetworkObject netObject = instance.transform.GetComponent<NetworkObject>();

            if (netObject == null) return;

            netObject.Despawn(true);
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
