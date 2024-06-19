using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace BrutalCompanyMinus.Minus.MonoBehaviours
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(Turret))]
    public class GrabbableTurret : GrabbableObject
    {
        public Transform turretTransform;

        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        private static void onTurretStart(ref Turret __instance)
        {
            if (!RoundManager.Instance.IsHost) return;

            if (Events.GrabbableTurrets.Active && __instance.tag != "PhysicsProp")
            {
                __instance.StartCoroutine(destroySelfAndReplace(__instance));
            }
            else
            {
                seed++;
                System.Random rng = new System.Random(seed);
                Net.Instance.GenerateAndSyncTerminalCodeServerRpc(__instance.NetworkObject, rng.Next(RoundManager.Instance.possibleCodesForBigDoors.Length));
            }
        }

        private static int seed = 0;
        private static IEnumerator destroySelfAndReplace(Turret __instance)
        {
            MEvent _event = Events.GrabbableTurrets.Instance;

            float rarity = _event.Getf(MEvent.ScaleType.Rarity);

            seed++;
            System.Random rng = new System.Random(StartOfRound.Instance.randomMapSeed + seed);
            if (rng.NextDouble() <= rarity)
            {
                GameObject grabbableTurret = Instantiate(Assets.grabbableTurret.spawnPrefab, RoundManager.Instance.GetRandomNavMeshPositionInBoxPredictable(__instance.transform.position, 30.0f, RoundManager.Instance.navHit, rng), __instance.transform.rotation);
                NetworkObject netObject = grabbableTurret.GetComponent<NetworkObject>();
                netObject.Spawn();

                Net.Instance.GenerateAndSyncTerminalCodeServerRpc(__instance.NetworkObject, rng.Next(RoundManager.Instance.possibleCodesForBigDoors.Length));

                Net.Instance.SyncScrapValueServerRpc(netObject, (int)(UnityEngine.Random.Range(Assets.grabbableTurret.minValue, Assets.grabbableTurret.maxValue + 1) * RoundManager.Instance.scrapValueMultiplier));

                yield return new WaitForSeconds(5.0f);

                try
                {
                    __instance.transform.parent.gameObject.GetComponent<NetworkObject>().Despawn(destroy: true);
                }
                catch
                {

                }
            }
        }

        public override void Start()
        {
            base.Start();
            StartCoroutine(UpdateTransform(11.0f, new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
        }

        public IEnumerator UpdateTransform(float time, Vector3 rotation)
        {
            yield return new WaitForSeconds(time);
            if (RoundManager.Instance.IsHost) syncRotationServerRpc(rotation);
        }

        [ServerRpc(RequireOwnership = false)]
        private void syncRotationServerRpc(Vector3 eulerAngle) => syncRotationClientRpc(eulerAngle);

        [ClientRpc]
        private void syncRotationClientRpc(Vector3 eulerAngle) => turretTransform.eulerAngles += eulerAngle;
    }
}
