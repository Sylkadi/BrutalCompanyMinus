using HarmonyLib;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using com.github.zehsteam.ToilHead.MonoBehaviours;
using BrutalCompanyMinus.Minus.Events;
using BrutalCompanyMinus.Minus.MonoBehaviours;
using static UnityEngine.GraphicsBuffer;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    internal class FacilityGhost
    {
        public static float actionCurrentTime = 0.0f, actionTimeCooldown = 15.0f; // Normal

        public static float ghostCrazyCurrentTime = 0.0f, ghostCrazyPeriod = 3.0f, ghostCrazyActionInterval = 0.1f, crazyGhostChance = 0.1f; // Crazy

        public static int DoNothingWeight = 20, OpenCloseBigDoorsWeight = 20, MessWithLightsWeight = 16, MessWithBreakerWeight = 4, OpenCloseDoorsWeight = 9, lockUnlockDoorsWeight = 3, disableTurretsWeight = 5, disableLandminesWeight = 5, disableSpikeTrapsWeight = 5, turretRageWeight = 15;

        public static float chanceToOpenCloseDoor = 0.3f, chanceToLockUnlockDoor = 0.1f, rageTurretsChance = 0.33f;

        private static System.Random rng = new System.Random();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RoundManager), "Update")]
        private static void OnUpdate()
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
                rng = new System.Random(Net.Instance._seed++);

                // Decide if ghosts goes crazy
                if(rng.NextDouble() <= crazyGhostChance && ghostCrazyCurrentTime <= 0.0f)
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

                int[] weights = new int[10] { DoNothingWeight, OpenCloseDoorsWeight, MessWithLightsWeight, MessWithBreakerWeight, OpenCloseDoorsWeight, lockUnlockDoorsWeight, disableTurretsWeight, disableLandminesWeight, disableSpikeTrapsWeight, turretRageWeight };
                if (ghostCrazyCurrentTime > 0.0f)
                {
                    weights[0] = 0; // Wont attempt to do nothing when going crazy
                    weights[5] = 0; // Wont attempt to lock or unlock doors when going crazy
                    weights[6] = 0; // Wont attempt to disable turrets whjen going crazy
                    weights[7] = 0; // Wont attempt to disable landmines when going crazy
                    weights[8] = 0; // Wont attempt to disable spiektraps when going crazy
                }
                int ghostDecision = RoundManager.Instance.GetRandomWeightedIndex(weights, rng);

                switch(ghostDecision)
                {
                    case 0:
                        Log.LogInfo("Facility ghost did nothing");
                        break;
                    case 1:
                        TerminalAccessibleObject[] doors = GameObject.FindObjectsOfType<TerminalAccessibleObject>();
                        if (doors.Length == 0) break;
                        Log.LogInfo("Facility ghost did OpenClose doors");
                        foreach (TerminalAccessibleObject door in doors)
                        {
                            door.SetDoorOpenServerRpc(Convert.ToBoolean(rng.Next(2)));
                        }
                        break;
                    case 2:
                        Log.LogInfo("Facility ghost messed with the lights");
                        Net.Instance.MessWithLightsServerRpc();
                        break;
                    case 3:
                        Log.LogInfo("Facility ghost messed with breaker");
                        Net.Instance.MessWithBreakerServerRpc(Convert.ToBoolean(rng.Next(2)));
                        break;
                    case 4:
                        Log.LogInfo("Facility ghost attempts to open and close doors");
                        Net.Instance.MessWithDoorsServerRpc(chanceToOpenCloseDoor);
                        break;
                    case 5:
                        Log.LogInfo("Facility ghost attempts to lock and unlock doors");
                        Net.Instance.MessWithDoorsServerRpc(chanceToOpenCloseDoor, true, chanceToLockUnlockDoor);
                        break;
                    case 6:
                        Log.LogInfo("Facility ghost attempts to disable turrets");

                        Turret[] turrets = GameObject.FindObjectsOfType<Turret>();
                        foreach(Turret turret in turrets)
                        {
                            if (Convert.ToBoolean(rng.Next(2))) RoundManager.Instance.StartCoroutine(DisableTurret(turret));
                        }
                        if (Compatibility.toilheadPresent) AttemptToDisableToilHeadTurrets();
                        break;
                    case 7:
                        Log.LogInfo("Facility ghost attempts to disable landmines");

                        Landmine[] landmines = GameObject.FindObjectsOfType<Landmine>();
                        foreach(Landmine landmine in landmines)
                        {
                            if (Convert.ToBoolean(rng.Next(2))) RoundManager.Instance.StartCoroutine(DisableLandmine(landmine));
                        }

                        GrabbableLandmine[] grabbableLandmines = GameObject.FindObjectsOfType<GrabbableLandmine>();
                        foreach (GrabbableLandmine grabbableLandmine in grabbableLandmines)
                        {
                            if (Convert.ToBoolean(rng.Next(2))) RoundManager.Instance.StartCoroutine(DisableGrabbableLandmine(grabbableLandmine));
                        }
                        break;
                    case 8:
                        Log.LogInfo("Facility ghost attempts to disable spiketraps");

                        SpikeRoofTrap[] spikeTraps = GameObject.FindObjectsOfType<SpikeRoofTrap>();
                        foreach (SpikeRoofTrap spikeTrap in spikeTraps)
                        {
                            if (Convert.ToBoolean(rng.Next(2))) RoundManager.Instance.StartCoroutine(DisableSpikeTrap(spikeTrap));
                        }
                        break;
                    case 9:
                        Log.LogInfo("Facility ghost attempts to rage turrets");
                        Turret[] _turrets = GameObject.FindObjectsOfType<Turret>();
                        foreach(Turret _turret in _turrets)
                        {
                            if(rng.NextDouble() <= rageTurretsChance) 
                            {
                                if (_turret.turretMode == TurretMode.Berserk || _turret.turretMode == TurretMode.Firing || !_turret.turretActive) continue;

                                _turret.turretMode = TurretMode.Berserk;
                                _turret.EnterBerserkModeServerRpc((int)GameNetworkManager.Instance.localPlayerController.playerClientId);
                            }
                        }
                        if (Compatibility.toilheadPresent) AttemptToRageToilHeadTurrets();
                        break;
                }
            }
        }

        private static void AttemptToDisableToilHeadTurrets()
        {
            ToilHeadTurretBehaviour[] turrets = GameObject.FindObjectsOfType<ToilHeadTurretBehaviour>();

            foreach(ToilHeadTurretBehaviour turret in turrets)
            {
                if (Convert.ToBoolean(rng.Next(2))) RoundManager.Instance.StartCoroutine(DisableToilHeadTurret(turret));
            }
        }

        private static void AttemptToRageToilHeadTurrets()
        {
            ToilHeadTurretBehaviour[] turrets = GameObject.FindObjectsOfType<ToilHeadTurretBehaviour>();

            foreach (ToilHeadTurretBehaviour turret in turrets)
            {
                if (rng.NextDouble() <= rageTurretsChance)
                {
                    if (turret.turretMode == TurretMode.Berserk || turret.turretMode == TurretMode.Firing || !turret.turretActive) continue;

                    turret.turretMode = TurretMode.Berserk;
                    turret.EnterBerserkModeServerRpc();
                }
            }
        }

        private static IEnumerator DisableTurret(Turret turret)
        {
            turret.ToggleTurretEnabled(false);
            yield return new WaitForSeconds(7.0f);
            turret.ToggleTurretEnabled(true);
        }
        
        private static IEnumerator DisableToilHeadTurret(object toilheadTurretobj) // Harmony why do you make me do this crap???
        {
            ToilHeadTurretBehaviour toilheadTurret = (ToilHeadTurretBehaviour)toilheadTurretobj;
            toilheadTurret.ToggleTurretEnabled(false);
            yield return new WaitForSeconds(7.0f);
            toilheadTurret.ToggleTurretEnabled(true);
        }

        private static IEnumerator DisableLandmine(Landmine landmine)
        {
            landmine.ToggleMine(false);
            yield return new WaitForSeconds(7.0f);
            landmine.ToggleMine(true);
        }

        private static IEnumerator DisableGrabbableLandmine(GrabbableLandmine grabbableLandmine)
        {
            grabbableLandmine.ToggleMine(false);
            yield return new WaitForSeconds(7.0f);
            grabbableLandmine.ToggleMine(true);
        }

        private static IEnumerator DisableSpikeTrap(SpikeRoofTrap trap)
        {
            trap.ToggleSpikesEnabled(false);
            yield return new WaitForSeconds(7.0f);
            trap.ToggleSpikesEnabled(true);
        }
    }
}
