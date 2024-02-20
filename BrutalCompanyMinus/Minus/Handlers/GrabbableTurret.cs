using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Handlers
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
            if (Events.GrabbableTurrets.Active && RoundManager.Instance.IsHost)
            {
                MEvent _event = MEvent.GetEvent(nameof(Events.GrabbableTurrets));

                float rarity = _event.Getf(MEvent.ScaleType.Rarity);

                if (UnityEngine.Random.Range(0.0f, 1.0f) - rarity <= 0.0f)
                {
                    GameObject grabbableTurret = Instantiate(Assets.grabbableTurret.spawnPrefab, __instance.transform.position, __instance.transform.rotation);
                    NetworkObject netObject = grabbableTurret.GetComponent<NetworkObject>();
                    netObject.Spawn();

                    __instance.StartCoroutine(destroySelf(__instance, netObject)); // destroy
                }
            }
        }

        private static IEnumerator destroySelf(Turret __instance, NetworkObjectReference netObject)
        {
            yield return new WaitForSeconds(7.0f);

            Net.Instance.SyncScrapValueServerRpc(netObject, (int)(UnityEngine.Random.Range(Assets.grabbableTurret.minValue, Assets.grabbableTurret.maxValue + 1) * RoundManager.Instance.scrapValueMultiplier));
            try
            {
                __instance.transform.parent.gameObject.GetComponent<NetworkObject>().Despawn(destroy: true);
            } catch
            {

            }
        }
        
        public override void Start()
        {
            base.Start();
            StartCoroutine(LateUpdateTransform());
        }


        public IEnumerator LateUpdateTransform()
        {
            yield return new WaitForSeconds(12.0f);
            if (RoundManager.Instance.IsHost) syncRotationServerRpc(new Vector3(0, UnityEngine.Random.Range(0, 360), 0));
        }

        [ServerRpc(RequireOwnership = false)]
        private void syncRotationServerRpc(Vector3 eulerAngle)
        {
            syncRotationClientRpc(eulerAngle);
        }

        [ClientRpc]
        private void syncRotationClientRpc(Vector3 eulerAngle)
        {
            turretTransform.eulerAngles += eulerAngle;
        }
    }
}
