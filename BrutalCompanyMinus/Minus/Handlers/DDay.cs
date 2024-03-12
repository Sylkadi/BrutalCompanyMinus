using Discord;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    internal class DDay
    {
        static float currentTime = 0;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoundManager), "Update")]
        public static void OnUpdate(ref RoundManager __instance)
        {
            if (!Events.DDay.Active) return;
            if(currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            } else
            {
                currentTime = 30;
                List<Vector3> x = Functions.GetOutsideNodes();
                foreach (Vector3 node in x)
                {
                    ArtilleryShell.FireAt(node);
                    ArtilleryShell.FireAt(node);
                    ArtilleryShell.FireAt(node);
                }
            }
        }

    }
}
