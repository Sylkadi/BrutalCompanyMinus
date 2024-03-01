using HarmonyLib;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(EnemyAI))]
    internal class Bounty
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(EnemyAI.KillEnemyOnOwnerClient))]
        static void PayOnkill(ref EnemyAI __instance)
        {
            if (!Events.Bounty.Active || __instance.isEnemyDead) return;
            MEvent bountEvent = Events.Bounty.Instance;
            Manager.PayCredits(UnityEngine.Random.Range(bountEvent.Get(MEvent.ScaleType.MinValue), bountEvent.Get(MEvent.ScaleType.MaxValue) + 1));
        }
    }
}
