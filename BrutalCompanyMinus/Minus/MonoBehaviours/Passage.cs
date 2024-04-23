using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
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

            audioReverbPreset = isInsideBuilding ? 2 : 1;
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

            if(Compatibility.cullFactoryPresent)
            {
                Compatibility.cullOnTeleportLocalPlayer.Invoke(null, null);
            }
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

            if (Compatibility.cullFactoryPresent)
            {
                Compatibility.cullOnTeleportOtherPlayer.Invoke(null, new object[] { playerID });
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

        [ServerRpc(RequireOwnership = false)]
        public void SyncBunkerPassagesServerRpc(NetworkObjectReference entrance, NetworkObjectReference exit) => SyncBunkerPassagesClientRpc(entrance, exit);

        [ClientRpc]
        public void SyncBunkerPassagesClientRpc(NetworkObjectReference entrance, NetworkObjectReference exit)
        {
            entrance.TryGet(out NetworkObject entranceNetObject);
            exit.TryGet(out NetworkObject exitNetObject);

            BunkerLidPassage entrancePassage = entranceNetObject.transform.Find("EntrancePassage").GetComponent<BunkerLidPassage>();
            BunkerLadderPassage exitPassage = exitNetObject.transform.Find("EscapePassage").GetComponent<BunkerLadderPassage>();
            BunkerLid lid = entranceNetObject.transform.Find("Lid").Find("Interactable").GetComponent<BunkerLid>();

            entrancePassage.otherPassage = exitPassage;
            exitPassage.otherPassage = entrancePassage;

            entrancePassage.bunkerLid = lid;
            exitPassage.bunkerLid = lid;
        }

        private static int bunkerPassagesToSpawn = 0;
        public static void SpawnBunkerPassage(int amount) => bunkerPassagesToSpawn += amount;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RoundManager), "RefreshEnemiesList")]
        private static void DoSpawnBunkerPassage()
        {
            if (bunkerPassagesToSpawn <= 0) return;
            int seed = Net.Instance._seed++ + Environment.TickCount;

            List<Vector3> bunkerEscapePositions = new List<Vector3>();
            List<Vector3> bunkerSpawnPositions = new List<Vector3>();

            List<Vector3> outsideSpawnNodes = Helper.GetOutsideNodes();
            List<Vector3> insideSpawnNodes = Helper.GetInsideAINodes();
            List<Vector3> spawnDenialNodes = Helper.GetSpawnDenialNodes();
            List<Vector3> spikeTraps = GameObject.FindObjectsOfType<SpikeRoofTrap>().Select(s => s.transform.position).ToList();
            List<Vector3> landmines = GameObject.FindObjectsOfType<Landmine>().Select(l => l.transform.position).ToList();

            for (int i = 0; i < bunkerPassagesToSpawn; i++)
            {
                System.Random rng = new System.Random(seed++);
                
                Vector3 entranceSpawnPositon = Vector3.zero, escapeSpawnPosition = Vector3.zero;

                // Entrance
                for (int j = 0; j < 20; j++) // 20 Attempts
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
                    Vector3 randomNode = insideSpawnNodes[rng.Next(outsideSpawnNodes.Count)];

                    if (!Helper.IsSafe(randomNode, spawnDenialNodes, 30.0f) || !Helper.IsSafe(randomNode, bunkerEscapePositions, 15.0f) || !Helper.IsSafe(randomNode, spikeTraps, 3.0f) || !Helper.IsSafe(randomNode, landmines, 0.5f)) continue;

                    // Get highest point
                    RaycastHit[] hits = Physics.RaycastAll(new Ray(randomNode, Vector3.up), 8.0f);
                    if (hits.Length == 0) continue;

                    int furthestIndex = 0;
                    float furthestDistance = 0.0f;
                    for (int k = 0; k < hits.Length; k++)
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

                entrancePassage.SyncBunkerPassagesServerRpc(bunkerEntrance.GetComponent<NetworkObject>(), bunkerEscape.GetComponent<NetworkObject>());

                Manager.objectsToClear.Add(bunkerEntrance);
                Manager.objectsToClear.Add(bunkerEscape);
            }

            bunkerPassagesToSpawn = 0;
        }
    }
}