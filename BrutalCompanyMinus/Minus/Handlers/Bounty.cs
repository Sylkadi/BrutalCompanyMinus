using HarmonyLib;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(EnemyAI))]
    internal class Bounty
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(EnemyAI.KillEnemyServerRpc))]
        static void OnKillEnemyServerRpc()
        {
            if (!Manager.BountyActive) return;
            Manager.paycut += UnityEngine.Random.Range(5 + (Manager.daysPassed / 7), 20 + (Manager.daysPassed / 5));
        }
    }
}
