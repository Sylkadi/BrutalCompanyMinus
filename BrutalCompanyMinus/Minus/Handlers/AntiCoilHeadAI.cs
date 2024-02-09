using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Handlers
{
    public class AntiCoilHeadAI : EnemyAI
    {
        public AISearchRoutine searchForPlayers;

        private float checkLineOfSightInterval;

        private bool hasEnteredChaseMode;

        private bool stoppingMovement;

        private bool hasStopped;

        public AnimationStopPoints animStopPoints;

        private float currentChaseSpeed = 14.5f;

        private float currentAnimSpeed = 1f;

        private PlayerControllerB previousTarget;

        private bool wasOwnerLastFrame;

        private float stopAndGoMinimumInterval;

        private float timeSinceHittingPlayer;

        public AudioClip[] springNoises;

        public Collider mainCollider;

        public Light leftEyeLight;
        public Light rightEyeLight;

        public override void DoAIInterval()
        {
            base.DoAIInterval();
            if (StartOfRound.Instance.allPlayersDead || isEnemyDead)
            {
                return;
            }
            switch (currentBehaviourStateIndex)
            {
                case 0:
                    {
                        if (!base.IsServer)
                        {
                            ChangeOwnershipOfEnemy(StartOfRound.Instance.allPlayerScripts[0].actualClientId);
                            break;
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            if (PlayerIsTargetable(StartOfRound.Instance.allPlayerScripts[i]) && !Physics.Linecast(base.transform.position + Vector3.up * 0.5f, StartOfRound.Instance.allPlayerScripts[i].gameplayCamera.transform.position, StartOfRound.Instance.collidersAndRoomMaskAndDefault) && Vector3.Distance(base.transform.position, StartOfRound.Instance.allPlayerScripts[i].transform.position) < 30f)
                            {
                                SwitchToBehaviourState(1);
                                return;
                            }
                        }
                        agent.speed = 6f;
                        if (!searchForPlayers.inProgress)
                        {
                            movingTowardsTargetPlayer = false;
                            StartSearch(base.transform.position, searchForPlayers);
                        }
                        break;
                    }
                case 1:
                    if (searchForPlayers.inProgress)
                    {
                        StopSearch(searchForPlayers);
                    }
                    if (TargetClosestPlayer())
                    {
                        if (previousTarget != targetPlayer)
                        {
                            previousTarget = targetPlayer;
                            ChangeOwnershipOfEnemy(targetPlayer.actualClientId);
                        }
                        movingTowardsTargetPlayer = true;
                    }
                    else
                    {
                        SwitchToBehaviourState(0);
                        ChangeOwnershipOfEnemy(StartOfRound.Instance.allPlayerScripts[0].actualClientId);
                    }
                    break;
            }
        }

        public override void Update()
        {
            base.Update();
            if (isEnemyDead)
            {
                return;
            }
            if (timeSinceHittingPlayer >= 0f)
            {
                timeSinceHittingPlayer -= Time.deltaTime;
            }
            int num = currentBehaviourStateIndex;
            if (num == 0 || num != 1)
            {
                return;
            }
            if (base.IsOwner)
            {
                if (stopAndGoMinimumInterval > 0f)
                {
                    stopAndGoMinimumInterval -= Time.deltaTime;
                }
                if (!wasOwnerLastFrame)
                {
                    wasOwnerLastFrame = true;
                    if (!stoppingMovement && timeSinceHittingPlayer < 0.12f)
                    {
                        agent.speed = currentChaseSpeed;
                    }
                    else
                    {
                        agent.speed = 0f;
                    }
                }
                bool flag = true;
                for (int i = 0; i < 4; i++)
                {
                    if (PlayerIsTargetable(StartOfRound.Instance.allPlayerScripts[i]) && StartOfRound.Instance.allPlayerScripts[i].HasLineOfSightToPosition(base.transform.position + Vector3.up * 1.6f, 68f) && Vector3.Distance(StartOfRound.Instance.allPlayerScripts[i].gameplayCamera.transform.position, eye.position) > 0.3f)
                    {
                        flag = false;
                    }
                }
                if (stunNormalizedTimer > 0f)
                {
                    flag = true;
                }
                if (flag != stoppingMovement && stopAndGoMinimumInterval <= 0f)
                {
                    stopAndGoMinimumInterval = 0.15f;
                    if (flag)
                    {
                        SetAnimationStopServerRpc();
                    }
                    else
                    {
                        SetAnimationGoServerRpc();
                    }
                    stoppingMovement = flag;
                }
            }
            if (stoppingMovement)
            {
                if (!animStopPoints.canAnimationStop)
                {
                    return;
                }
                if (!hasStopped)
                {
                    hasStopped = true;
                    if (GameNetworkManager.Instance.localPlayerController.HasLineOfSightToPosition(base.transform.position, 70f, 25))
                    {
                        float num2 = Vector3.Distance(base.transform.position, GameNetworkManager.Instance.localPlayerController.transform.position);
                        if (num2 < 4f)
                        {
                            GameNetworkManager.Instance.localPlayerController.JumpToFearLevel(0.9f);
                        }
                        else if (num2 < 9f)
                        {
                            GameNetworkManager.Instance.localPlayerController.JumpToFearLevel(0.4f);
                        }
                    }
                    if (currentAnimSpeed > 2f)
                    {
                        RoundManager.PlayRandomClip(creatureVoice, springNoises, randomize: false);
                        if (animStopPoints.animationPosition == 1)
                        {
                            creatureAnimator.SetTrigger("springBoing");
                        }
                        else
                        {
                            creatureAnimator.SetTrigger("springBoingPosition2");
                        }
                    }
                }
                if (mainCollider.isTrigger && Vector3.Distance(GameNetworkManager.Instance.localPlayerController.transform.position, base.transform.position) > 0.25f)
                {
                    mainCollider.isTrigger = false;
                }
                creatureAnimator.SetFloat("walkSpeed", 0f);
                currentAnimSpeed = 0f;
                if (base.IsOwner)
                {
                    agent.speed = 0f;
                }
            }
            else
            {
                if (hasStopped)
                {
                    hasStopped = false;
                    mainCollider.isTrigger = true;
                }
                currentAnimSpeed = Mathf.Lerp(currentAnimSpeed, 6f, 5f * Time.deltaTime);
                creatureAnimator.SetFloat("walkSpeed", currentAnimSpeed);
                if (base.IsOwner)
                {
                    agent.speed = Mathf.Lerp(agent.speed, currentChaseSpeed, 4.5f * Time.deltaTime);
                }
            }
        }

        [ServerRpc]
        public void SetAnimationStopServerRpc()
        {
            SetAnimationStopClientRpc();
        }

        [ClientRpc]
        public void SetAnimationStopClientRpc()
        {
            stoppingMovement = true;
        }

        [ServerRpc]
        public void SetAnimationGoServerRpc()
        {
            SetAnimationGoClientRpc();
        }

        [ClientRpc]
        public void SetAnimationGoClientRpc()
        {
            stoppingMovement = false;
        }

        public override void OnCollideWithPlayer(Collider other)
        {
            base.OnCollideWithPlayer(other);
            if (!stoppingMovement && currentBehaviourStateIndex == 1 && !(timeSinceHittingPlayer >= 0f))
            {
                PlayerControllerB controller = other.gameObject.GetComponent<PlayerControllerB>();
                if (controller != null)
                {
                    timeSinceHittingPlayer = 0.2f;
                    controller.DamagePlayer(90, hasDamageSFX: true, callRPC: true, CauseOfDeath.Mauling, 2);
                    controller.JumpToFearLevel(1f);
                }
            }
        }

        protected override void __initializeVariables()
        {
            base.__initializeVariables();
        }
    }
}