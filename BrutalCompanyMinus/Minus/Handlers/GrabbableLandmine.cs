using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(Landmine))]
    public class GrabbableLandmine : GrabbableObject, IHittable
    {
        private bool mineActivated = true;

        public bool hasExploded;

        public ParticleSystem explosionParticle;

        public Animator mineAnimator;

        public AudioSource mineAudio;

        public AudioSource mineFarAudio;

        public AudioSource mineTickSource;

        public AudioClip mineDetonate;

        public AudioClip mineTrigger;

        public AudioClip mineDetonateFar;

        public AudioClip mineDeactivate;

        public AudioClip minePress;

        public Light mineLight1;

        public Light mineLight2;

        public Light mineLight3;

        private bool sendingExplosionRPC;

        private RaycastHit hit;

        private RoundManager roundManager;

        private float pressMineDebounceTimer;

        private bool localPlayerOnMine;

        private bool mineGrabbed = false;

        private bool onBlowUpSchedule = false;

        private float countDown = 0.0f;

        private float dropSafetyTime = 0.0f;

        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        private static void onLandmineStart(ref Landmine __instance)
        {
            if(Events.GrabbableLandmines.Active && RoundManager.Instance.IsHost)
            {
                __instance.StartCoroutine(destroySelfAndReplace(__instance));
            }
        }

        private static int seed = 0;
        private static IEnumerator destroySelfAndReplace(Landmine __instance)
        {
            MEvent _event = Events.GrabbableLandmines.Instance;

            float rarity = _event.Getf(MEvent.ScaleType.Rarity);

            seed++;
            System.Random rng = new System.Random(StartOfRound.Instance.randomMapSeed + seed);
            if (rng.NextDouble() <= rarity)
            {
                GameObject grabbableLandmine = Instantiate(Assets.grabbableLandmine.spawnPrefab, __instance.transform.position, Quaternion.identity);
                NetworkObject netObject = grabbableLandmine.GetComponent<NetworkObject>();
                netObject.Spawn();

                Net.Instance.SyncScrapValueServerRpc(netObject, (int)(UnityEngine.Random.Range(Assets.grabbableLandmine.minValue, Assets.grabbableLandmine.maxValue + 1) * RoundManager.Instance.scrapValueMultiplier));

                yield return new WaitForSeconds(5.0f);

                try
                {
                    __instance.transform.parent.gameObject.GetComponent<NetworkObject>().Despawn(destroy: true);
                } catch
                {

                }
            }
        }

        public override void Start()
        {
            StartCoroutine(StartIdleAnimation());
            base.Start();

            dropSafetyTime = 2.0f;
            mineGrabbed = true;
        }

        public override void Update()
        {
            if (countDown > 0.0f)
            {
                countDown -= Time.deltaTime;
            } 
            if (dropSafetyTime > 0.0f)
            {
                dropSafetyTime -= Time.deltaTime;
            } else
            {
                mineGrabbed = false;
            }
            if (countDown <= 0.0f && onBlowUpSchedule)
            {
                onBlowUpSchedule = false;
                playMineTickSourceServerRpc(false);
                ExplodeMineServerRpc();
            }
            if (pressMineDebounceTimer > 0f)
            {
                pressMineDebounceTimer -= Time.deltaTime;
            }
            if (localPlayerOnMine && GameNetworkManager.Instance.localPlayerController.teleportedLastFrame)
            {
                localPlayerOnMine = false;
                TriggerMineOnLocalClientByExiting();
            }
            base.Update();
        }

        private void onGrab()
        {
            mineGrabbed = true;
            onBlowUpSchedule = true;
            dropSafetyTime = 6.0f;
            countDown = 6.0f;
            playMineTickSourceServerRpc(true);
        }

        private void onDiscard()
        {
            mineGrabbed = true;
            onBlowUpSchedule = false;
            dropSafetyTime = 1.5f;
            countDown = 0.0f;
            playMineTickSourceServerRpc(false);
        }

        public override void GrabItem()
        {
            base.GrabItem();
            onGrab();
        }

        public override void DiscardItem()
        {
            base.DiscardItem();
            onDiscard();
        }

        public override void GrabItemFromEnemy(EnemyAI enemy)
        {
            base.GrabItemFromEnemy(enemy);
            onGrab();
        }

        public override void DiscardItemFromEnemy()
        {
            base.DiscardItemFromEnemy();
            onDiscard();
        }

        [ServerRpc(RequireOwnership = false)]
        private void playMineTickSourceServerRpc(bool toggle)
        {
            playMineTickSourceClientRpc(toggle);
        }

        [ClientRpc]
        private void playMineTickSourceClientRpc(bool toggle)
        {
            if(toggle)
            {
                mineTickSource.Play();
            } else
            {
                mineTickSource.Stop();
            }
        }

        public void ToggleMine(bool enabled)
        {
            if (mineActivated != enabled)
            {
                mineActivated = enabled;
                if (!enabled)
                {
                    mineAudio.PlayOneShot(mineDeactivate);
                    WalkieTalkie.TransmitOneShotAudio(mineAudio, mineDeactivate);
                }
                ToggleMineServerRpc(enabled);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ToggleMineServerRpc(bool enable)
        {
            ToggleMineClientRpc(enable);
        }

        [ClientRpc]
        public void ToggleMineClientRpc(bool enable)
        {
            ToggleMineEnabledLocalClient(enable);
        }

        public void ToggleMineEnabledLocalClient(bool enabled)
        {
            if (mineActivated != enabled)
            {
                mineActivated = enabled;
                if (!enabled)
                {
                    mineAudio.PlayOneShot(mineDeactivate);
                    WalkieTalkie.TransmitOneShotAudio(mineAudio, mineDeactivate);
                }
            }
        }

        private IEnumerator StartIdleAnimation()
        {
            roundManager = FindObjectOfType<RoundManager>();
            if (!(roundManager == null))
            {
                if (roundManager.BreakerBoxRandom != null)
                {
                    yield return new WaitForSeconds((float)roundManager.BreakerBoxRandom.NextDouble() + 0.5f);
                }
                mineAnimator.SetTrigger("startIdle");
                mineAudio.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasExploded || pressMineDebounceTimer > 0f || mineGrabbed || Events.GrabbableLandmines.LandmineDisabled)
            {
                return;
            }
            Debug.Log(string.Format("Trigger entered mine: {0}; {1}; {2}", other.tag, other.CompareTag("Player") || other.CompareTag("PlayerBody"), other.CompareTag("PhysicsProp") || other.tag.StartsWith("PlayerRagdoll")));
            if (other.CompareTag("Player") || other.CompareTag("PlayerBody"))
            {
                localPlayerOnMine = true;
                pressMineDebounceTimer = 0.5f;
                PressMineServerRpc();
            }
            else
            {
                if (!other.CompareTag("PhysicsProp") && !other.tag.StartsWith("PlayerRagdoll"))
                {
                    return;
                }
                if ((bool)other.GetComponent<DeadBodyInfo>())
                {
                    if (other.GetComponent<DeadBodyInfo>().playerScript != GameNetworkManager.Instance.localPlayerController)
                    {
                        return;
                    }
                }
                else if ((bool)other.GetComponent<GrabbableObject>() && !other.GetComponent<GrabbableObject>().NetworkObject.IsOwner)
                {
                    return;
                }
                pressMineDebounceTimer = 0.5f;
                PressMineServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void PressMineServerRpc()
        {
            PressMineClientRpc();
        }

        [ClientRpc]
        public void PressMineClientRpc()
        {
            pressMineDebounceTimer = 0.5f;
            mineAudio.PlayOneShot(minePress);
            WalkieTalkie.TransmitOneShotAudio(mineAudio, minePress);
        }

        private void OnTriggerExit(Collider other)
        {
            if (hasExploded || !mineActivated || mineGrabbed || Events.GrabbableLandmines.LandmineDisabled)
            {
                return;
            }
            Debug.Log("Object leaving mine trigger, gameobject name: " + other.gameObject.name);
            Debug.Log(string.Format("Trigger exited mine: {0}; ({1} / {2}) {3}; {4}", other.tag, other.gameObject.name, other.transform.name, other.CompareTag("Player") || other.CompareTag("PlayerBody"), other.CompareTag("PhysicsProp") || other.tag.StartsWith("PlayerRagdoll")));
            if (other.CompareTag("Player") || other.CompareTag("PlayerBody"))
            {
                localPlayerOnMine = false;
                TriggerMineOnLocalClientByExiting();
            }
            else
            {
                if (!other.tag.StartsWith("PlayerRagdoll") && !other.CompareTag("PhysicsProp"))
                {
                    return;
                }
                if ((bool)other.GetComponent<DeadBodyInfo>())
                {
                    if (other.GetComponent<DeadBodyInfo>().playerScript != GameNetworkManager.Instance.localPlayerController)
                    {
                        return;
                    }
                }
                else if ((bool)other.GetComponent<GrabbableObject>() && !other.GetComponent<GrabbableObject>().NetworkObject.IsOwner)
                {
                    return;
                }
                TriggerMineOnLocalClientByExiting();
            }
        }

        private void TriggerMineOnLocalClientByExiting()
        {
            if (!hasExploded || !mineGrabbed)
            {
                SetOffMineAnimation();
                sendingExplosionRPC = true;
                ExplodeMineServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ExplodeMineServerRpc()
        {
            ExplodeMineClientRpc();
        }

        [ClientRpc]
        public void ExplodeMineClientRpc()
        {
            if (sendingExplosionRPC)
            {
                sendingExplosionRPC = false;
            }
            else
            {
                SetOffMineAnimation();
            }
        }

        public void SetOffMineAnimation()
        {
            if (dropSafetyTime > 0.0f) return;
            hasExploded = true;
            mineAnimator.SetTrigger("detonate");
            mineAudio.PlayOneShot(mineTrigger, 1f);
        }

        private IEnumerator TriggerOtherMineDelayed(GrabbableLandmine mine)
        {
            if (!mine.hasExploded)
            {
                mine.mineAudio.pitch = UnityEngine.Random.Range(0.75f, 1.07f);
                mine.hasExploded = true;
                yield return new WaitForSeconds(0.2f);
                mine.SetOffMineAnimation();
            }
        }

        public void Detonate()
        {
            if (dropSafetyTime > 0.0f) return;
            mineAudio.pitch = UnityEngine.Random.Range(0.93f, 1.07f);
            mineAudio.PlayOneShot(mineDetonate, 1f);
            SpawnExplosion(base.transform.position + Vector3.up, spawnExplosionEffect: false, 5.7f, 6.4f);
        }

        public static void SpawnExplosion(Vector3 explosionPosition, bool spawnExplosionEffect = false, float killRange = 1f, float damageRange = 1f)
        {
            Debug.Log("Spawning explosion at pos: {explosionPosition}");
            if (spawnExplosionEffect)
            {
                Instantiate(StartOfRound.Instance.explosionPrefab, explosionPosition, Quaternion.Euler(-90f, 0f, 0f), RoundManager.Instance.mapPropsContainer.transform).SetActive(value: true);
            }
            float num = Vector3.Distance(GameNetworkManager.Instance.localPlayerController.transform.position, explosionPosition);
            if (num < 14f)
            {
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
            }
            else if (num < 25f)
            {
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Small);
            }
            Collider[] array = Physics.OverlapSphere(explosionPosition, 6f, 2621448, QueryTriggerInteraction.Collide);
            PlayerControllerB playerControllerB = null;
            for (int i = 0; i < array.Length; i++)
            {
                float num2 = Vector3.Distance(explosionPosition, array[i].transform.position);
                if (num2 > 4f && Physics.Linecast(explosionPosition, array[i].transform.position + Vector3.up * 0.3f, 256, QueryTriggerInteraction.Ignore))
                {
                    continue;
                }
                if (array[i].gameObject.layer == 3)
                {
                    playerControllerB = array[i].gameObject.GetComponent<PlayerControllerB>();
                    if (playerControllerB != null && playerControllerB.IsOwner)
                    {
                        if (num2 < killRange)
                        {
                            Vector3 bodyVelocity = (playerControllerB.gameplayCamera.transform.position - explosionPosition) * 80f / Vector3.Distance(playerControllerB.gameplayCamera.transform.position, explosionPosition);
                            playerControllerB.KillPlayer(bodyVelocity, spawnBody: true, CauseOfDeath.Blast);
                        }
                        else if (num2 < damageRange)
                        {
                            playerControllerB.DamagePlayer(50);
                        }
                    }
                }
                else if (array[i].gameObject.layer == 21)
                {
                    GrabbableLandmine componentInChildren = array[i].gameObject.GetComponentInChildren<GrabbableLandmine>();
                    if (componentInChildren != null && !componentInChildren.hasExploded && num2 < 6f)
                    {
                        Debug.Log("Setting off other mine");
                        componentInChildren.StartCoroutine(componentInChildren.TriggerOtherMineDelayed(componentInChildren));
                    }
                }
                else if (array[i].gameObject.layer == 19)
                {
                    EnemyAICollisionDetect componentInChildren2 = array[i].gameObject.GetComponentInChildren<EnemyAICollisionDetect>();
                    if (componentInChildren2 != null && componentInChildren2.mainScript.IsOwner && num2 < 4.5f)
                    {
                        componentInChildren2.mainScript.HitEnemyOnLocalClient(6);
                    }
                }
            }
            int num3 = ~LayerMask.GetMask("Room");
            num3 = ~LayerMask.GetMask("Colliders");
            array = Physics.OverlapSphere(explosionPosition, 10f, num3);
            for (int j = 0; j < array.Length; j++)
            {
                Rigidbody component = array[j].GetComponent<Rigidbody>();
                if (component != null)
                {
                    component.AddExplosionForce(70f, explosionPosition, 10f);
                }
            }
        }

        public bool MineHasLineOfSight(Vector3 pos)
        {
            return !Physics.Linecast(base.transform.position, pos, out hit, 256);
        }
        
        bool IHittable.Hit(int force, Vector3 hitDirection, PlayerControllerB playerWhoHit = null, bool playHitSFX = false)
        {
            SetOffMineAnimation();
            sendingExplosionRPC = true;
            ExplodeMineServerRpc();
            return true;
        }
       
    }

}
