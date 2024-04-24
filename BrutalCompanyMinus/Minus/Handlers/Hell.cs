using BrutalCompanyMinus.Minus.MonoBehaviours;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    internal class Hell
    {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoundManager), "AssignRandomEnemyToVent")]
        private static void OnAssignRandomEnemyToVent(ref RoundManager __instance)
        {
            if (!Events.Hell.Active) return;
            __instance.currentMaxInsidePower = 0;
            __instance.currentMaxOutsidePower = 0;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RoundManager), "RefreshEnemiesList")]
        private static void OnRefreshEnemiesList()
        {
            if (!Events.Hell.Active || !Events.Hell.SpawnCycle) return;

            EnemySpawnCycle.Instance.spawnCycles.Add(Events.Hell.insideHellSpawnCycle);
            RoundManager.Instance.StartCoroutine(AddOutsideSpawnCycleAfterDelay());

            Events.Hell.SpawnCycle = false;
        }

        private static IEnumerator AddOutsideSpawnCycleAfterDelay()
        {
            yield return new WaitForSeconds(75.0f);
            EnemySpawnCycle.Instance.spawnCycles.Add(Events.Hell.outsideHellSpawnCycle);
        }
    }
}
