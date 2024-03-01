using GameNetcodeStuff;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(EnemyAI))]
    internal class _EnemyAI
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(EnemyAI.MeetsStandardPlayerCollisionConditions))]
        private static void OnMeetsStandardPlayerCollisionConditions(ref PlayerControllerB __result, ref Collider other, ref EnemyType ___enemyType, ref bool ___isEnemyDead, ref bool inKillAnimation, ref float ___stunNormalizedTimer) // This fix works, maybe theres a better way
        {
            // Why am i doing this again? just gona leave it here since nothing breaks.
            PlayerControllerB controller = other.gameObject.GetComponent<PlayerControllerB>();
            if (controller != null)
            {
                if (!___isEnemyDead && ___stunNormalizedTimer < 0.0f && !inKillAnimation && __result == null) // (This may have some unintended consequences)
                {
                    if (controller.actualClientId == GameNetworkManager.Instance.localPlayerController.actualClientId) __result = controller;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void OnStart(ref EnemyAI __instance) // Set isOutside and scale hp
        {
            __instance.StartCoroutine(UpdateHP(__instance));
            
            try
            {
                GameObject terrainMap = GameObject.FindGameObjectWithTag(Manager.terrainTag);
                GameObject[] objects = GameObject.FindGameObjectsWithTag(Manager.terrainTag);
                foreach (GameObject obj in objects)
                {
                    if (obj.name == Manager.terrainName)
                    {
                        terrainMap = obj;
                    }
                }

                float y = -100.0f;
                if (terrainMap != null) y = terrainMap.transform.position.y - 100.0f;

                if (__instance.transform.position.y > y)
                {
                    __instance.isOutside = true;
                    __instance.allAINodes = GameObject.FindGameObjectsWithTag("OutsideAINode"); // Otherwise AI would be fucked
                    if (GameNetworkManager.Instance.localPlayerController != null)
                    {
                        __instance.EnableEnemyMesh(!StartOfRound.Instance.hangarDoorsClosed || !GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom);
                    }
                    __instance.SyncPositionToClients();
                }
                else
                {
                    __instance.isOutside = false;
                    __instance.allAINodes = GameObject.FindGameObjectsWithTag("AINode");
                    __instance.SyncPositionToClients();
                }
            } catch
            {
                Log.LogError("Failed to set isOutside on EnemyAI.Start");
            }
        }

        private static IEnumerator UpdateHP(EnemyAI __instance)
        {
            yield return new WaitUntil(() => Net.Instance.receivedSyncedValues);
            __instance.enemyHP = (int)Functions.Range(__instance.enemyHP + Manager.bonusEnemyHp, 1.1f, 99999999.0f);
        }

    }
}
