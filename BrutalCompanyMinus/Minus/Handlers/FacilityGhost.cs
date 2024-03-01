using HarmonyLib;
using System;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    internal class FacilityGhost
    {
        public static float actionCurrentTime = 0.0f, actionTimeCooldown = 15.0f; // Normal

        public static float ghostCrazyCurrentTime = 0.0f, ghostCrazyPeriod = 3.0f, ghostCrazyActionInterval = 0.1f, crazyGhostChance = 0.1f; // Crazy

        public static int DoNothingWeight = 50, OpenCloseBigDoorsWeight = 20, MessWithLightsWeight = 16, MessWithBreakerWeight = 4, OpenCloseDoorsWeight = 9, lockUnlockDoorsWeight = 3;

        public static float chanceToOpenCloseDoor = 0.3f, chanceToLockUnlockDoor = 0.1f;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RoundManager), "Update")]
        static void OnUpdate()
        {
            if (!Events.FacilityGhost.Active || !RoundManager.Instance.IsHost) return;

            if(ghostCrazyCurrentTime > 0.0f)
            {
                ghostCrazyCurrentTime -= Time.deltaTime;
            }
            if(actionCurrentTime > 0.0f)
            {
                actionCurrentTime -= Time.deltaTime;
            } else
            {
                // Decide if ghosts goes crazy
                if(UnityEngine.Random.Range(0.0f, 1.0f) <= crazyGhostChance && ghostCrazyCurrentTime <= 0.0f)
                {
                    Log.LogInfo("Ghost has went crazy");
                    ghostCrazyCurrentTime = ghostCrazyPeriod;
                }

                if(ghostCrazyCurrentTime > 0.0f)
                {
                    actionCurrentTime = ghostCrazyActionInterval;
                } else
                {
                    actionCurrentTime = actionTimeCooldown;
                }

                int[] weights = new int[6] { DoNothingWeight, OpenCloseDoorsWeight, MessWithLightsWeight, MessWithBreakerWeight, OpenCloseDoorsWeight, lockUnlockDoorsWeight };
                if (ghostCrazyCurrentTime > 0.0f)
                {
                    weights[0] = 0; // Wont attempt to do nothing when going crazy
                    weights[5] = 0; // Wont attempt to open or close doors when going crazy
                }
                int ghostDecision = RoundManager.Instance.GetRandomWeightedIndex(weights, new System.Random());

                switch(ghostDecision)
                {
                    case 0: // Do Nothing
                        Log.LogInfo("Facility ghost did nothing");
                        break;
                    case 1: // Open or Close Big Doors
                        TerminalAccessibleObject[] doors = GameObject.FindObjectsOfType<TerminalAccessibleObject>();
                        if (doors.Length == 0) break;
                        Log.LogInfo("Facility ghost did OpenClose doors.");
                        foreach (TerminalAccessibleObject door in doors)
                        {
                            door.SetDoorOpenServerRpc(Convert.ToBoolean(UnityEngine.Random.Range(0, 2)));
                        }
                        break;
                    case 2: // Mess with lights
                        Log.LogInfo("Facility ghost messed with the lights");
                        Net.Instance.MessWithLightsServerRpc();
                        break;
                    case 3: // Mess with breaker
                        Log.LogInfo("Facility ghost messed with breaker");
                        Net.Instance.MessWithBreakerServerRpc(Convert.ToBoolean(UnityEngine.Random.Range(0, 2)));
                        break;
                    case 4:
                        Log.LogInfo("Facility ghost attempts to open and close doors");
                        Net.Instance.MessWithDoorsServerRpc(chanceToOpenCloseDoor);
                        break;
                    case 5:
                        Log.LogInfo("Facility ghost attempts to lock and unlock doors");
                        Net.Instance.MessWithDoorsServerRpc(chanceToOpenCloseDoor, true, chanceToLockUnlockDoor);
                        break;
                }
            }
        }
    }
}
