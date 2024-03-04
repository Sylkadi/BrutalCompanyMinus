using HarmonyLib;
using System.Collections.Generic;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(EnemyAI))]
    internal class Bounty
    {
        public static List<int> enemyObjectIDs = new List<int>();

        [HarmonyPostfix]
        [HarmonyPatch(nameof(EnemyAI.KillEnemyOnOwnerClient))]
        static void PayOnkill(ref EnemyAI __instance)
        {
            if (!Events.Bounty.Active || __instance.isEnemyDead) return;
            foreach(int id in enemyObjectIDs)
            {
                if (__instance.gameObject.GetInstanceID() == id) return;
            }

            MEvent bountEvent = Events.Bounty.Instance;
            Manager.PayCredits(UnityEngine.Random.Range(bountEvent.Get(MEvent.ScaleType.MinValue), bountEvent.Get(MEvent.ScaleType.MaxValue) + 1));

            enemyObjectIDs.Add(__instance.gameObject.GetInstanceID());
        }
    }
}
