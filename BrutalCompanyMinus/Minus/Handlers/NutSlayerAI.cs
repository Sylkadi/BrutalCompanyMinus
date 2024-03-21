using System;
using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace BrutalCompanyMinus.Minus.Handlers
{
    public class NutSlayerAI : EnemyAI
    {
#pragma warning disable 0649
        private int previousBehaviourState = -1;

        private int previousBehaviourStateAIInterval = -1;

        public static float timeAtNextInspection;

        private bool inspectingLocalPlayer;

        private float localPlayerTurnDistance;

        private bool isInspecting;

        private bool hasGun;

        private int randomSeedNumber;

        public GameObject gunPrefab;

        public SlayerShotgun gun;

        public Transform gunPoint;

        private NetworkObjectReference gunObjectRef;

        public AISearchRoutine patrol;

        public AISearchRoutine attackSearch;

        public Transform torsoContainer;

        public float currentTorsoRotation;

        public int targetTorsoDegrees;

        public float torsoTurnSpeed = 6f;

        public AudioSource torsoTurnAudio;

        public AudioSource longRangeAudio;

        public AudioClip[] torsoFinishTurningClips;

        public AudioClip aimSFX;

        public AudioClip kickSFX;

        public GameObject shotgunShellPrefab;

        private bool torsoTurning;

        private System.Random NutcrackerRandom;

        private int timesDoingInspection;

        private Coroutine inspectionCoroutine;

        public int lastPlayerSeenMoving = 0;

        private float timeSinceSeeingTarget;

        private float timeSinceInspecting;

        private float timeSinceFiringGun;

        private bool aimingGun;

        private bool reloadingGun;

        private Vector3 lastSeenPlayerPos;

        private RaycastHit rayHit;

        private Coroutine gunCoroutine;

        private bool isLeaderScript;

        private Vector3 positionLastCheck;

        private Vector3 strafePosition;

        private bool reachedStrafePosition;

        private bool lostPlayerInChase;

        private float timeSinceHittingPlayer;

        private Coroutine waitToFireGunCoroutine;

        private float walkCheckInterval;

        private int setShotgunScrapValue;

        private int timesSeeingSamePlayer;

        private int previousPlayerSeenWhenAiming;

        private float speedWhileAiming;

        // Custom variables

        private float speedWhileMoving = 8.0f;
        private float widthSearch = 45f;
        private int rangeSearch = 30;
        private Transform target;
        private List<string> aiBlackList = new List<string>();
        private bool isFiring = false;

        private int setHp = 5;
        private int Lives = 4;
        private bool Immortal = false;

        public Light leftEyeLight;
        public Light rightEyeLight;

#pragma warning restore 0649

        public override void Start()
        {
            base.Start();
            lastPlayerSeenMoving = 0;
            if (base.IsServer)
            {
                InitializeNutcrackerValuesServerRpc();
                if (enemyType.numberSpawned <= 1)
                {
                    isLeaderScript = true;
                }
            }
            rayHit = default(RaycastHit);

            setHp = Configuration.nutSlayerHp.Value;
            Lives = Configuration.nutSlayerLives.Value;
            Immortal = Configuration.nutSlayerImmortal.Value;

            enemyHP = setHp;
        }

        [ServerRpc]
        public void InitializeNutcrackerValuesServerRpc()
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(gunPrefab, base.transform.position + Vector3.up * 0.5f, Quaternion.identity, RoundManager.Instance.spawnedScrapContainer);
            gameObject.GetComponent<NetworkObject>().Spawn();
            setShotgunScrapValue = UnityEngine.Random.Range(Configuration.slayerShotgunMinValue.Value, Configuration.slayerShotgunMaxValue.Value + 1);
            GrabGun(gameObject);
            randomSeedNumber = UnityEngine.Random.Range(0, 10000);
            InitializeNutcrackerValuesClientRpc(randomSeedNumber, gameObject.GetComponent<NetworkObject>(), setShotgunScrapValue);
        }

        [ClientRpc]
        public void InitializeNutcrackerValuesClientRpc(int randomSeed, NetworkObjectReference gunObject, int setShotgunValue)
        {
            setShotgunScrapValue = setShotgunValue;
            randomSeedNumber = randomSeed;
            gunObjectRef = gunObject;
        }

        private void GrabGun(GameObject gunObject)
        {
            gun = gunObject.GetComponent<SlayerShotgun>();
            if (gun == null)
            {
                LogEnemyError("Gun in GrabGun function did not contain ShotgunItem component.");
                return;
            }
            gun.SetScrapValue(setShotgunScrapValue);
            RoundManager.Instance.totalScrapValueInLevel += gun.scrapValue;
            gun.parentObject = gunPoint;
            gun.isHeldByEnemy = true;
            gun.grabbableToEnemies = false;
            gun.grabbable = false;
            gun.shellsLoaded = 2;
            gun.GrabItemFromEnemy(this);
        }

        private void DropGun(Vector3 dropPosition)
        {
            if (gun == null)
            {
                LogEnemyError("Could not drop gun since no gun was held!");
                return;
            }
            gun.DiscardItemFromEnemy();
            gun.isHeldByEnemy = false;
            gun.grabbableToEnemies = true;
            gun.grabbable = true;
        }

        private void SpawnShotgunShells()
        {
            if (base.IsOwner)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector3 position = base.transform.position + Vector3.up * 0.6f;
                    position += new Vector3(UnityEngine.Random.Range(-0.8f, 0.8f), 0f, UnityEngine.Random.Range(-0.8f, 0.8f));
                    GameObject obj = UnityEngine.Object.Instantiate(shotgunShellPrefab, position, Quaternion.identity, RoundManager.Instance.spawnedScrapContainer);
                    obj.GetComponent<GrabbableObject>().fallTime = 0f;
                    obj.GetComponent<NetworkObject>().Spawn();
                }
            }
        }

        [ServerRpc]
        public void DropGunServerRpc(Vector3 dropPosition)
        {
            DropGunClientRpc(dropPosition);
        }

        [ClientRpc]
        public void DropGunClientRpc(Vector3 dropPosition)
        {
            DropGun(dropPosition);
        }

        public override void DoAIInterval()
        {
            base.DoAIInterval();
            if (isEnemyDead || stunNormalizedTimer > 0f || gun == null)
            {
                return;
            }
            switch (currentBehaviourStateIndex)
            {
                case 0:
                    if (previousBehaviourStateAIInterval != currentBehaviourStateIndex)
                    {
                        previousBehaviourStateAIInterval = currentBehaviourStateIndex;
                        agent.stoppingDistance = 0.02f;
                    }
                    if (!patrol.inProgress)
                    {
                        StartSearch(base.transform.position, patrol);
                    }
                    break;
                case 2:
                    if (previousBehaviourStateAIInterval != currentBehaviourStateIndex)
                    {
                        previousBehaviourStateAIInterval = currentBehaviourStateIndex;
                        if (patrol.inProgress)
                        {
                            StopSearch(patrol);
                        }
                    }
                    if (timeSinceSeeingTarget >= 3.0f)
                    {
                        SwitchToBehaviourState(0);

                    }
                    if(timeSinceFiringGun >= 10.0f && !attackSearch.inProgress)
                    {
                        SwitchToBehaviourState(0);
                        timeSinceFiringGun = 0.0f;
                    }
                    if (!base.IsOwner)
                    {
                        break;
                    }
                    if (timeSinceSeeingTarget < 0.5f)
                    {
                        if (attackSearch.inProgress)
                        {
                            StopSearch(attackSearch);
                        }
                        reachedStrafePosition = false;
                        SetDestinationToPosition(lastSeenPlayerPos);
                        agent.stoppingDistance = 1f;
                        if (lostPlayerInChase)
                        {
                            lostPlayerInChase = false;
                            SetLostPlayerInChaseServerRpc(lostPlayer: false);
                        }
                        break;
                    }
                    agent.stoppingDistance = 0.02f;
                    if (!reachedStrafePosition)
                    {
                        if (!agent.CalculatePath(lastSeenPlayerPos, path1))
                        {
                            break;
                        }
                        if (DebugEnemy)
                        {
                            for (int i = 1; i < path1.corners.Length; i++)
                            {
                                Debug.DrawLine(path1.corners[i - 1], path1.corners[i], Color.red, AIIntervalTime);
                            }
                        }
                        if (path1.corners.Length > 1)
                        {
                            Ray ray = new Ray(path1.corners[path1.corners.Length - 1], path1.corners[path1.corners.Length - 1] - path1.corners[path1.corners.Length - 2]);
                            if (Physics.Raycast(ray, out rayHit, 5f, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore))
                            {
                                strafePosition = RoundManager.Instance.GetNavMeshPosition(ray.GetPoint(Mathf.Max(0f, rayHit.distance - 2f)));
                            }
                            else
                            {
                                strafePosition = RoundManager.Instance.GetNavMeshPosition(ray.GetPoint(6f));
                            }
                        }
                        else
                        {
                            strafePosition = lastSeenPlayerPos;
                        }
                        SetDestinationToPosition(strafePosition);
                        if (Vector3.Distance(base.transform.position, strafePosition) < 2f)
                        {
                            reachedStrafePosition = true;
                        }
                    }
                    else
                    {
                        if (!lostPlayerInChase)
                        {
                            lostPlayerInChase = true;
                            SetLostPlayerInChaseServerRpc(lostPlayer: true);
                        }
                        if (!attackSearch.inProgress)
                        {
                            StartSearch(strafePosition, attackSearch);
                        }
                    }
                    break;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetLostPlayerInChaseServerRpc(bool lostPlayer)
        {
            SetLostPlayerInChaseClientRpc(lostPlayer);
        }

        [ClientRpc]
        public void SetLostPlayerInChaseClientRpc(bool lostPlayer)
        {
            lostPlayerInChase = lostPlayer;
            if (!lostPlayer)
            {
                timeSinceSeeingTarget = 0f;
            }
        }

        private bool GrabGunIfNotHolding()
        {
            if (gun != null)
            {
                return true;
            }
            if (gunObjectRef.TryGet(out var networkObject))
            {
                gun = networkObject.gameObject.GetComponent<SlayerShotgun>();
                GrabGun(gun.gameObject);
            }
            return gun != null;
        }

        public void TurnTorsoToTargetDegrees()
        {
            currentTorsoRotation = Mathf.MoveTowardsAngle(currentTorsoRotation, targetTorsoDegrees, Time.deltaTime * torsoTurnSpeed);
            torsoContainer.localEulerAngles = new Vector3(currentTorsoRotation + 90f, 90f, 90f);
            if (Mathf.Abs(currentTorsoRotation - (float)targetTorsoDegrees) > 5f)
            {
                if (!torsoTurning)
                {
                    torsoTurning = true;
                    torsoTurnAudio.Play();
                }
            }
            else if (torsoTurning)
            {
                torsoTurning = false;
                torsoTurnAudio.Stop();
                RoundManager.PlayRandomClip(torsoTurnAudio, torsoFinishTurningClips);
            }
            torsoTurnAudio.volume = Mathf.Lerp(torsoTurnAudio.volume, 1f, Time.deltaTime * 2f);
        }

        private void SetTargetDegreesToPosition(Vector3 pos)
        {
            pos.y = base.transform.position.y;
            Vector3 vector = pos - base.transform.position;
            targetTorsoDegrees = (int)Vector3.Angle(vector, base.transform.forward);
            if (Vector3.Cross(base.transform.forward, vector).y > 0f)
            {
                targetTorsoDegrees = 360 - targetTorsoDegrees;
            }
            torsoTurnSpeed = 1500f;
        }

        private void StartInspectionTurn()
        {
            if (!isInspecting && !isEnemyDead)
            {
                timesDoingInspection++;
                if (inspectionCoroutine != null)
                {
                    StopCoroutine(inspectionCoroutine);
                }
                inspectionCoroutine = StartCoroutine(InspectionTurn());
            }
        }

        private IEnumerator InspectionTurn()
        {
            yield return new WaitForSeconds(0.75f);
            isInspecting = true;
            NutcrackerRandom = new System.Random(randomSeedNumber + timesDoingInspection);
            int degrees = 0;
            int turnTime = 1;
            for (int i = 0; i < 8; i++)
            {
                degrees = Mathf.Min(degrees + NutcrackerRandom.Next(45, 95), 360);
                if (Physics.Raycast(eye.position, eye.forward, 5f, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore))
                {
                    turnTime = 1;
                }
                else
                {
                    int a = ((!((float)turnTime > 2f)) ? 4 : (turnTime / 3));
                    turnTime = NutcrackerRandom.Next(1, Mathf.Max(a, 3));
                }
                targetTorsoDegrees = degrees;
                torsoTurnSpeed = NutcrackerRandom.Next(275, 855) / turnTime;
                yield return new WaitForSeconds(turnTime);
                if (degrees >= 360)
                {
                    break;
                }
            }
            if (base.IsOwner)
            {
                SwitchToBehaviourState(0);
            }
        }

        public void StopInspection()
        {
            if (isInspecting)
            {
                isInspecting = false;
            }
            if (inspectionCoroutine != null)
            {
                StopCoroutine(inspectionCoroutine);
            }
        }

        // Player

        [ServerRpc(RequireOwnership = false)]
        public void SeeMovingThreatServerRpc(Vector3 position, bool enterAttackFromPatrolMode = false, int playerId = -1)
        {
            SeeMovingThreatClientRpc(position, enterAttackFromPatrolMode, playerId);
        }

        [ClientRpc]
        public void SeeMovingThreatClientRpc(Vector3 position, bool enterAttackFromPatrolMode = false, int playerId = -1)
        {
            SwitchTargetTo(position, playerId);
            SwitchToBehaviourStateOnLocalClient(2);
        }

        private void GlobalNutcrackerClock()
        {
            if (isLeaderScript && Time.realtimeSinceStartup - timeAtNextInspection > 2f)
            {
                timeAtNextInspection = Time.realtimeSinceStartup + UnityEngine.Random.Range(6f, 15f);
            }
        }

        public override void Update()
        {
            base.Update();
            TurnTorsoToTargetDegrees();
            if (isEnemyDead)
            {
                StopInspection();
                return;
            }
            GlobalNutcrackerClock();
            if (!isEnemyDead && !GrabGunIfNotHolding())
            {
                return;
            }
            if (walkCheckInterval <= 0f)
            {
                walkCheckInterval = 0.1f;
                creatureAnimator.SetBool("IsWalking", (base.transform.position - positionLastCheck).sqrMagnitude > 0.001f);
                positionLastCheck = base.transform.position;
            }
            else
            {
                walkCheckInterval -= Time.deltaTime;
            }
            if (stunNormalizedTimer >= 0f)
            {
                agent.speed = 0f;
                return;
            }
            timeSinceSeeingTarget += Time.deltaTime;
            timeSinceInspecting += Time.deltaTime;
            timeSinceFiringGun += Time.deltaTime;
            timeSinceHittingPlayer += Time.deltaTime;
            creatureAnimator.SetInteger("State", currentBehaviourStateIndex);
            creatureAnimator.SetBool("Aiming", aimingGun);
            switch (currentBehaviourStateIndex)
            {
                case 0:
                    if (previousBehaviourState != currentBehaviourStateIndex)
                    {
                        previousBehaviourState = currentBehaviourStateIndex;
                        isInspecting = false;
                        lostPlayerInChase = false;
                        creatureVoice.Stop();
                    }
                    agent.speed = speedWhileMoving;
                    targetTorsoDegrees = 0;
                    torsoTurnSpeed = 525f;
                    if (CheckLineOfSightForTarget(widthSearch, rangeSearch, 1))
                    {
                        if (IsLocalPlayerMoving())
                        {
                            SeeMovingThreatServerRpc(Vector3.zero, false, (int)GameNetworkManager.Instance.localPlayerController.playerClientId);
                        }
                        else
                        {
                            SeeMovingThreatServerRpc(target.position, false, -1);
                        }
                    }
                    break;
                case 2:
                    if (previousBehaviourState != currentBehaviourStateIndex)
                    {
                        if (previousBehaviourState != 1)
                        {
                            longRangeAudio.PlayOneShot(enemyType.audioClips[3]);
                        }
                        StopInspection();
                        previousBehaviourState = currentBehaviourStateIndex;
                    }
                    if (base.IsOwner)
                    {
                        if (reloadingGun || aimingGun || (timeSinceFiringGun < 1.2f && timeSinceSeeingTarget < 0.5f) || timeSinceHittingPlayer < 1f)
                        {
                            if (aimingGun && !reloadingGun)
                            {
                                agent.speed = speedWhileAiming;
                            }
                            else
                            {
                                agent.speed = 0f;
                            }
                        }
                        else
                        {
                            agent.speed = 7f;
                        }
                    }
                    if (base.IsOwner && timeSinceFiringGun > 0.75f && gun.shellsLoaded <= 0 && !reloadingGun && !aimingGun)
                    {
                        reloadingGun = true;
                        ReloadGunServerRpc();
                    }
                    if (lostPlayerInChase)
                    {
                        targetTorsoDegrees = 0;
                    }
                    else
                    {
                        SetTargetDegreesToPosition(lastSeenPlayerPos);
                    }
                    if (HasLineOfSightToPosition(target.position, widthSearch, rangeSearch, 1f))
                    {
                        timeSinceSeeingTarget = 0f;
                        lastSeenPlayerPos = target.position;
                    }
                    if (!CheckLineOfSightForTarget(70f, 12, 1))
                    {
                        break;
                    }
                    if (CheckLineOfSightForTarget(widthSearch, 12, 1) && timeSinceSeeingTarget < 3.0f)
                    {
                        SetTargetDegreesToPosition(target.position);
                        TurnTorsoToTargetDegrees();
                        if (timeSinceFiringGun > gun.useCooldown && !reloadingGun && !aimingGun && timeSinceHittingPlayer > 1f)
                        {
                            timeSinceFiringGun = 0f;
                            agent.speed = 0f;
                            AimGunServerRpc(base.transform.position);
                        }
                        if (lostPlayerInChase)
                        {
                            lostPlayerInChase = false;
                            SetLostPlayerInChaseServerRpc(lostPlayer: false);
                        }
                        timeSinceSeeingTarget = 0f;
                        lastSeenPlayerPos = target.position;
                    }
                    else if (IsLocalPlayerMoving())
                    {
                        bool flag = (int)GameNetworkManager.Instance.localPlayerController.playerClientId == lastPlayerSeenMoving;
                        if (flag)
                        {
                            timeSinceSeeingTarget = 0f;
                        }
                        if (Vector3.Distance(base.transform.position, StartOfRound.Instance.allPlayerScripts[lastPlayerSeenMoving].transform.position) - Vector3.Distance(base.transform.position, GameNetworkManager.Instance.localPlayerController.transform.position) > 3f || (timeSinceSeeingTarget > 3f && !flag))
                        {
                            lastPlayerSeenMoving = (int)GameNetworkManager.Instance.localPlayerController.playerClientId;
                            SwitchTargetServerRpc(Vector3.zero, (int)GameNetworkManager.Instance.localPlayerController.playerClientId);
                        }
                    }
                    break;
            }
        }

        [ServerRpc]
        public void ReloadGunServerRpc()
        {
            if (aimingGun)
            {
                reloadingGun = false;
            }
            else
            {
                ReloadGunClientRpc();
            }
        }

        [ClientRpc]
        public void ReloadGunClientRpc()
        {
            StopAimingGun();
            gun.shellsLoaded = 2;
            gunCoroutine = StartCoroutine(ReloadGun());
        }

        private IEnumerator ReloadGun()
        {
            reloadingGun = true;
            creatureSFX.PlayOneShot(enemyType.audioClips[2]);
            creatureAnimator.SetBool("Reloading", value: true);
            yield return new WaitForSeconds(0.32f);
            gun.gunAnimator.SetBool("Reloading", value: true);
            yield return new WaitForSeconds(0.92f);
            gun.gunAnimator.SetBool("Reloading", value: false);
            creatureAnimator.SetBool("Reloading", value: false);
            yield return new WaitForSeconds(0.5f);
            reloadingGun = false;
        }

        private void StopReloading()
        {
            reloadingGun = false;
            gun.gunAnimator.SetBool("Reloading", value: false);
            creatureAnimator.SetBool("Reloading", value: false);
            if (gunCoroutine != null)
            {
                StopCoroutine(gunCoroutine);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void AimGunServerRpc(Vector3 enemyPos)
        {
            if (gun.shellsLoaded <= 0)
            {
                aimingGun = false;
                ReloadGunClientRpc();
            }
            else if (!reloadingGun)
            {
                aimingGun = true;
                AimGunClientRpc(enemyPos);
            }
        }

        [ClientRpc]
        public void AimGunClientRpc(Vector3 enemyPos)
        {
            StopReloading();
            gunCoroutine = StartCoroutine(AimGun(enemyPos));
        }

        private IEnumerator AimGun(Vector3 enemyPos)
        {
            aimingGun = true;
            if (lastPlayerSeenMoving == previousPlayerSeenWhenAiming)
            {
                timesSeeingSamePlayer++;
            }
            else
            {
                previousPlayerSeenWhenAiming = lastPlayerSeenMoving;
                timesSeeingSamePlayer = 0;
            }
            longRangeAudio.PlayOneShot(aimSFX);
            speedWhileAiming = speedWhileMoving * 0.35f;
            inSpecialAnimation = true;
            serverPosition = enemyPos;
            if (enemyHP <= 1)
            {
                yield return new WaitForSeconds(0.75f);
            }
            else if (gun.shellsLoaded == 1)
            {
                yield return new WaitForSeconds(1.3f);
            }
            else
            {
                yield return new WaitForSeconds(0.75f);
            }
            yield return new WaitForEndOfFrame();
            if (base.IsOwner && !isFiring)
            {
                FireGunServerRpc();
            }
            timeSinceFiringGun = 0f;
            yield return new WaitForSeconds(0.35f);
            aimingGun = false;
            inSpecialAnimation = false;
            creatureVoice.Play();
            creatureVoice.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        }

        [ServerRpc]
        public void FireGunServerRpc()
        {
            if (stunNormalizedTimer <= 0f)
            {
                FireGunClientRpc();
            }
            else
            {
                StartCoroutine(waitToFireGun());
            }
        }

        [ClientRpc]
        public void FireGunClientRpc()
        {
            Vector3 targetForward = new Vector3(gun.shotgunRayPoint.forward.x, (target.position - gun.shotgunRayPoint.position).normalized.y, gun.shotgunRayPoint.forward.z);
            FireGun(gun.shotgunRayPoint.position, targetForward);
        }

        private IEnumerator waitToFireGun()
        {
            yield return new WaitUntil(() => stunNormalizedTimer <= 0f);
            yield return new WaitForSeconds(0.5f);
            FireGunClientRpc();
        }

        private void StopAimingGun()
        {
            inSpecialAnimation = false;
            aimingGun = false;
            if (gunCoroutine != null)
            {
                StopCoroutine(gunCoroutine);
            }
        }

        private void FireGun(Vector3 gunPosition, Vector3 gunForward)
        {
            isFiring = true;
            fire(gunPosition, gunForward);
            StartCoroutine(fireAfterDelay(0.35f, gunPosition, gunForward));
            StartCoroutine(fireAfterDelay(0.7f, gunPosition, gunForward));
            isFiring = false;
        }

        private IEnumerator fireAfterDelay(float time, Vector3 gunPosition, Vector3 gunForward)
        {
            yield return new WaitForSeconds(time);
            gun.currentUseCooldown = -1.0f;
            fire(gunPosition, gunForward);
        }

        private void fire(Vector3 gunPosition, Vector3 gunForward)
        {
            creatureAnimator.ResetTrigger("ShootGun");
            creatureAnimator.SetTrigger("ShootGun");
            if (gun == null)
            {
                LogEnemyError("No gun held on local client, unable to shoot");
            }
            else
            {
                gun.ShootGun(gunPosition, gunForward);
            }
        }


        [ServerRpc(RequireOwnership = false)]
        public void SwitchTargetServerRpc(Vector3 position, int playerId = -1)
        {
            SwitchTargetClientRpc(position, playerId);
        }

        [ClientRpc]
        public void SwitchTargetClientRpc(Vector3 position, int playerId = -1)
        {
            SwitchTargetTo(position, playerId);
        }

        private void SwitchTargetTo(Vector3 position, int playerId = -1)
        {
            if (playerId != -1)
            {
                lastPlayerSeenMoving = playerId;
                timeSinceSeeingTarget = 0f;
                lastSeenPlayerPos = StartOfRound.Instance.allPlayerScripts[playerId].transform.position;
            }
            else
            {
                timeSinceSeeingTarget = 0f;
                lastSeenPlayerPos = position;
            }
        }

        public bool CheckLineOfSightForTarget(float width = 45f, int range = 60, int proximityAwareness = -1)
        {
            Vector3 position = GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform.position;
            if (Vector3.Distance(position, eye.position) < (float)range && !Physics.Linecast(eye.position, position, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
            {
                Vector3 to = position - eye.position;
                if (Vector3.Angle(eye.forward, to) < width || (proximityAwareness != -1 && Vector3.Distance(eye.position, position) < (float)proximityAwareness))
                {
                    target = GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform;
                    return true;
                }
            }
            foreach (EnemyAI ai in RoundManager.Instance.SpawnedEnemies)
            {
                if (ai == null || ai.transform == null || ai.isEnemyDead) continue; // Skip
                position = ai.transform.position;
                Vector3 to = position - eye.position;
                if (Vector3.Distance(position, eye.position) < (float)range && !Physics.Linecast(eye.position, position, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
                {
                    if (Vector3.Angle(eye.forward, to) < width || (proximityAwareness != -1 && Vector3.Distance(eye.position, position) < (float)proximityAwareness))
                    {
                        target = ai.transform;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsLocalPlayerMoving()
        {
            localPlayerTurnDistance += StartOfRound.Instance.playerLookMagnitudeThisFrame;
            if (localPlayerTurnDistance > 0.1f && Vector3.Distance(GameNetworkManager.Instance.localPlayerController.transform.position, base.transform.position) < 10f)
            {
                return true;
            }
            if (GameNetworkManager.Instance.localPlayerController.performingEmote)
            {
                return true;
            }
            if (Time.realtimeSinceStartup - StartOfRound.Instance.timeAtMakingLastPersonalMovement < 0.25f)
            {
                return true;
            }
            if (GameNetworkManager.Instance.localPlayerController.timeSincePlayerMoving < 0.02f)
            {
                return true;
            }
            return false;
        }

        public override void OnCollideWithPlayer(Collider other)
        {
            base.OnCollideWithPlayer(other);
            if (!isEnemyDead && !(timeSinceHittingPlayer < 1f) && !(stunNormalizedTimer >= 0f))
            {
                PlayerControllerB playerControllerB = MeetsStandardPlayerCollisionConditions(other, reloadingGun || aimingGun);
                if (playerControllerB != null)
                {
                    timeSinceHittingPlayer = 0f;
                    LegKickPlayerServerRpc((int)playerControllerB.playerClientId);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void LegKickPlayerServerRpc(int playerId)
        {
            LegKickPlayerClientRpc(playerId);
        }

        [ClientRpc]
        public void LegKickPlayerClientRpc(int playerId)
        {
            LegKickPlayer(playerId);
        }

        private void LegKickPlayer(int playerId)
        {
            timeSinceHittingPlayer = 0f;
            PlayerControllerB playerControllerB = StartOfRound.Instance.allPlayerScripts[playerId];
            RoundManager.Instance.tempTransform.position = base.transform.position;
            RoundManager.Instance.tempTransform.LookAt(playerControllerB.transform.position);
            base.transform.eulerAngles = new Vector3(0f, RoundManager.Instance.tempTransform.eulerAngles.y, 0f);
            serverRotation = new Vector3(0f, RoundManager.Instance.tempTransform.eulerAngles.y, 0f);
            Vector3 bodyVelocity = Vector3.Normalize((playerControllerB.transform.position + Vector3.up * 0.75f - base.transform.position) * 100f) * 25f;
            playerControllerB.KillPlayer(bodyVelocity, spawnBody: true, CauseOfDeath.Kicking);
            creatureAnimator.SetTrigger("Kick");
            creatureSFX.Stop();
            torsoTurnAudio.volume = 0f;
            creatureSFX.PlayOneShot(kickSFX);
            if (currentBehaviourStateIndex != 2)
            {
                SwitchTargetTo(Vector3.zero, playerId);
                SwitchToBehaviourStateOnLocalClient(2);
            }
        }

        public override void HitEnemy(int force = 1, PlayerControllerB playerWhoHit = null, bool playHitSFX = false)
        {
            if (Immortal) return;
            base.HitEnemy(force, playerWhoHit, playHitSFX);
            if (!isEnemyDead)
            {
                if (isInspecting || currentBehaviourStateIndex == 2)
                {
                    creatureSFX.PlayOneShot(enemyType.audioClips[0]);
                    enemyHP -= force;
                }
                else
                {
                    creatureSFX.PlayOneShot(enemyType.audioClips[1]);
                }
                if (playerWhoHit != null)
                {
                    SeeMovingThreatServerRpc(Vector3.zero, enterAttackFromPatrolMode: true, (int)playerWhoHit.playerClientId);
                }
                if (enemyHP <= 0 && base.IsOwner)
                {
                    enemyHP = setHp;
                    Lives--;
                    Log.LogInfo(string.Format("Nutslayer new lives:{0}", Lives));
                    if(Lives <= 0) KillEnemyOnOwnerClient();
                }
            }
        }

        public override void KillEnemy(bool destroy = false)
        {
            if (Immortal) return;
            base.KillEnemy(destroy);
            targetTorsoDegrees = 0;
            StopInspection();
            StopReloading();
            if (base.IsOwner)
            {
                DropGunServerRpc(gunPoint.position);
                StartCoroutine(spawnShotgunShellsOnDelay());
            }
            creatureVoice.Stop();
            torsoTurnAudio.Stop();
        }

        private IEnumerator spawnShotgunShellsOnDelay()
        {
            yield return new WaitForSeconds(1.2f);
            SpawnShotgunShells();
        }
    }
}
