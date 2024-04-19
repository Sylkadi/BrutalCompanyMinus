using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.MonoBehaviours
{
    [HarmonyPatch]
    public class Passage : NetworkBehaviour
    {
        internal Passage otherPassage;

        public bool isInsideBuilding;

        public Transform spawnPosition;

        public InteractTrigger passageTrigger;

        public int audioReverbPreset = -1;

        public AudioSource passageAudioSource;

        public AudioClip[] doorAudios;

        internal static float checkForEnemyInterval = 0.0f;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            EntranceTeleport[] entrances = FindObjectsOfType<EntranceTeleport>();
            foreach(EntranceTeleport entrance in entrances)
            {
                if(entrance != null && entrance.audioReverbPreset != -1)
                {
                    audioReverbPreset = entrance.audioReverbPreset;
                    break;
                }
            }
        }

        public virtual void TeleportPlayer()
        {
            Transform thisPlayer = GameNetworkManager.Instance.localPlayerController.thisPlayerBody;
            GameNetworkManager.Instance.localPlayerController.TeleportPlayer(otherPassage.spawnPosition.position);
            GameNetworkManager.Instance.localPlayerController.isInElevator = false;
            GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom = false;
            thisPlayer.eulerAngles = new Vector3(thisPlayer.eulerAngles.x, otherPassage.spawnPosition.eulerAngles.y, thisPlayer.eulerAngles.z);
            SetAudioPreset((int)GameNetworkManager.Instance.localPlayerController.playerClientId);
            for (int i = 0; i < GameNetworkManager.Instance.localPlayerController.ItemSlots.Length; i++)
            {
                if (GameNetworkManager.Instance.localPlayerController.ItemSlots[i] != null)
                {
                    GameNetworkManager.Instance.localPlayerController.ItemSlots[i].isInFactory = isInsideBuilding;
                }
            }
            TeleportPlayerServerRpc((int)GameNetworkManager.Instance.localPlayerController.playerClientId);
            GameNetworkManager.Instance.localPlayerController.isInsideFactory = isInsideBuilding;
        }

        [ServerRpc(RequireOwnership = false)]
        private void TeleportPlayerServerRpc(int playerID) => TeleportPlayerClientRpc(playerID);

        [ClientRpc]
        private void TeleportPlayerClientRpc(int playerID)
        {
            StartOfRound instance = StartOfRound.Instance;

            if (instance == null) return;

            instance.allPlayerScripts[playerID].TeleportPlayer(otherPassage.spawnPosition.position, withRotation: true, otherPassage.spawnPosition.eulerAngles.y);
            instance.allPlayerScripts[playerID].isInElevator = false;
            instance.allPlayerScripts[playerID].isInHangarShipRoom = false;
            instance.allPlayerScripts[playerID].isInsideFactory = isInsideBuilding;

            for (int i = 0; i < instance.allPlayerScripts[playerID].ItemSlots.Length; i++)
            {
                if (instance.allPlayerScripts[playerID].ItemSlots[i] != null)
                {
                    instance.allPlayerScripts[playerID].ItemSlots[i].isInFactory = isInsideBuilding;
                }
            }

            if (GameNetworkManager.Instance.localPlayerController.isPlayerDead && instance.allPlayerScripts[playerID] == GameNetworkManager.Instance.localPlayerController.spectatedPlayerScript)
            {
                SetAudioPreset(playerID);
            }
        }

        public void SetAudioPreset(int playerObj)
        {
            if (audioReverbPreset != -1)
            {
                GameObject.FindObjectOfType<AudioReverbPresets>().audioPresets[audioReverbPreset].ChangeAudioReverbForPlayer(StartOfRound.Instance.allPlayerScripts[playerObj]);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void PlayAudioAtTeleportPositionsServerRpc() => PlayAudioAtTeleportPositionsClientRpc();

        [ClientRpc]
        public void PlayAudioAtTeleportPositionsClientRpc() => PlayAudioAtTeleportPositions();

        public void PlayAudioAtTeleportPositions()
        {
            if (doorAudios.Length != 0)
            {
                passageAudioSource.PlayOneShot(doorAudios[UnityEngine.Random.Range(0, doorAudios.Length)]);
                otherPassage.passageAudioSource.PlayOneShot(doorAudios[UnityEngine.Random.Range(0, doorAudios.Length)]);
            }
        }

        private static int bunkerPassagesToSpawn = 0;

        public static void SpawnBunkerPassage(int amount) => bunkerPassagesToSpawn += amount;

        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch(typeof(RoundManager), "FinishGeneratingLevel")]
        private static void DoSpawnBunkerPassage()
        {
            int seed = Net.Instance._seed++ + Environment.TickCount;

            List<Vector3> bunkerEscapePositions = new List<Vector3>();
            List<Vector3> bunkerSpawnPositions = new List<Vector3>();

            for(int i = 0; i < bunkerPassagesToSpawn; i++)
            {
                System.Random rng = new System.Random(seed++);

                List<Vector3> outsideSpawnNodes = Helper.GetOutsideNodes();
                List<Vector3> insideSpawnNodes = Helper.GetInsideAINodes();
                List<Vector3> spawnDenialNodes = Helper.GetSpawnDenialNodes();

                Vector3 entranceSpawnPositon = Vector3.zero, escapeSpawnPosition = Vector3.zero;

                // Entrance
                for (int j = 0; j < 10; j++) // 10 Attempts
                {
                    rng = new System.Random(seed++);
                    Vector3 randomNode = RoundManager.Instance.GetRandomNavMeshPositionInBoxPredictable(outsideSpawnNodes[rng.Next(outsideSpawnNodes.Count)], 30.0f, RoundManager.Instance.navHit, rng);

                    if (!Helper.IsSafe(randomNode, spawnDenialNodes, 20.0f) || !Helper.IsSafe(randomNode, bunkerSpawnPositions, 15.0f)) continue;

                    Physics.Raycast(new Ray(randomNode, Vector3.down), out RaycastHit hit);
                    entranceSpawnPositon = hit.point;
                    break;
                }

                if (entranceSpawnPositon == Vector3.zero)
                {
                    entranceSpawnPositon = outsideSpawnNodes[rng.Next(outsideSpawnNodes.Count)];
                }
                bunkerSpawnPositions.Add(entranceSpawnPositon);

                // Exit
                for (int j = 0; j < 100; j++) // 100 Attempts
                {
                    rng = new System.Random(seed++);
                    Vector3 randomNode = RoundManager.Instance.GetRandomNavMeshPositionInBoxPredictable(insideSpawnNodes[rng.Next(outsideSpawnNodes.Count)], 50.0f, RoundManager.Instance.navHit, rng);

                    if (!Helper.IsSafe(randomNode, spawnDenialNodes, 30.0f) || !Helper.IsSafe(randomNode, bunkerEscapePositions, 20.0f)) continue;

                    // Get highest point
                    RaycastHit[] hits = Physics.RaycastAll(new Ray(randomNode, Vector3.up), 8.0f);
                    if (hits.Length == 0) continue;

                    int furthestIndex = 0;
                    float furthestDistance = 0.0f;
                    for(int k = 0; k < hits.Length; k++)
                    {
                        float distanceCalculation = Vector3.Distance(hits[k].point, randomNode);
                        if (distanceCalculation > furthestDistance)
                        {
                            furthestDistance = distanceCalculation;
                            furthestIndex = k;
                        }
                    }

                    // Check if there is anything underneath to land on
                    if (!Physics.Raycast(new Ray(randomNode - new Vector3(0, -2.0f, 0), Vector3.down))) continue;

                    escapeSpawnPosition = hits[furthestIndex].point;
                    break;
                }

                if (escapeSpawnPosition == Vector3.zero)
                {
                    escapeSpawnPosition = insideSpawnNodes[rng.Next(outsideSpawnNodes.Count)] + new Vector3(0, 5.3f, 0.0f);
                }
                bunkerEscapePositions.Add(escapeSpawnPosition);


                float yRotation = RoundManager.Instance.YRotationThatFacesTheFarthestFromPosition(escapeSpawnPosition + Vector3.up * 0.2f) - 180.0f;

                Quaternion bunkerEntranceRotation = Assets.bunkerEntrance.transform.rotation;
                bunkerEntranceRotation.eulerAngles = new Vector3(bunkerEntranceRotation.eulerAngles.x, yRotation, bunkerEntranceRotation.eulerAngles.z);
                GameObject bunkerEntrance = GameObject.Instantiate(Assets.bunkerEntrance, entranceSpawnPositon, bunkerEntranceRotation);
                bunkerEntrance.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
                BunkerLidPassage entrancePassage = bunkerEntrance.transform.Find("EntrancePassage").GetComponent<BunkerLidPassage>();

                Quaternion bunkerEscapeRotation = Assets.bunkerEscape.transform.rotation;
                bunkerEscapeRotation.eulerAngles = new Vector3(bunkerEscapeRotation.eulerAngles.x, yRotation - 90.0f, bunkerEscapeRotation.eulerAngles.z);
                GameObject bunkerEscape = GameObject.Instantiate(Assets.bunkerEscape, escapeSpawnPosition, bunkerEscapeRotation);
                bunkerEscape.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
                BunkerLadderPassage escapePassage = bunkerEscape.transform.Find("EscapePassage").GetComponent<BunkerLadderPassage>();

                entrancePassage.otherPassage = escapePassage;
                escapePassage.otherPassage = entrancePassage;

                BunkerLid lid = bunkerEntrance.transform.Find("Lid").Find("Interactable").GetComponent<BunkerLid>();

                entrancePassage.bunkerLid = lid;
                escapePassage.bunkerLid = lid;
            }

            bunkerPassagesToSpawn = 0;
        }
    }
}
