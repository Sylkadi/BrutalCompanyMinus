using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    internal class SafeOutside
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(RoundManager), "RefreshEnemiesList")]
        private static void OnRefreshEnemiesList(ref RoundManager __instance)
        {
            if (!Events.SafeOutside.Active) return;
            __instance.currentOutsideEnemyPower = __instance.currentMaxOutsidePower;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EnemyAI), "Start")]
        private static void OnEnemyAIStart(ref EnemyAI __instance)
        {
            if(!Events.SafeOutside.Active) return;

            GameObject terrainMap = Manager.terrainObject;

            float y = -100.0f;
            if (terrainMap != null) y = terrainMap.transform.position.y - 100.0f;

            if (__instance.transform.position.y > y) __instance.StartCoroutine(DestroyEnemyAI(__instance));
        }

        private static IEnumerator DestroyEnemyAI(EnemyAI ai)
        {
            yield return new WaitForSeconds(0.1f);
            NetworkObject netObj = ai.GetComponent<NetworkObject>();
            if(netObj != null)
            {
                netObj.Despawn(destroy: true);
            } else
            {
                Log.LogError("Failed to capture enemyAI networkobject while safeOutside is active");
            }
        }
    }
}
