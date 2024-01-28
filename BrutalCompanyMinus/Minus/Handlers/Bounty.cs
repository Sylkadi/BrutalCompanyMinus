using HarmonyLib;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(EnemyAI))]
    internal class Bounty
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(EnemyAI.KillEnemyOnOwnerClient))]
        static void PayOnkill()
        {
            if (!Manager.BountyActive) return;
            MEvent bountEvent = MEvent.GetEvent(nameof(Events.Bounty));
            Manager.PayCredits(UnityEngine.Random.Range(bountEvent.Get(MEvent.ScaleType.MinValue), bountEvent.Get(MEvent.ScaleType.MaxValue) + 1));
        }
    }
}
