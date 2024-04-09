using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    internal class SafeOutside
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(RoundManager), "RefreshEnemiesList")]
        private static void OnRefreshEnemiesList(ref RoundManager __instance) => __instance.currentOutsideEnemyPower = __instance.currentMaxOutsidePower;
    }
}
