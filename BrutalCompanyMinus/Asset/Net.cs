using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using HarmonyLib;
using BrutalCompanyMinus.Minus;
using Unity.Collections;

namespace BrutalCompanyMinus
{
    [HarmonyPatch]
    internal class Net : NetworkBehaviour
    {
        public static Net Instance { get; private set; }
        public static GameObject netObject { get; private set; }

        public NetworkList<Weather> currentWeatherMultipliers;
        public NetworkList<OutsideObjectsToSpawn> outsideObjectsToSpawn;

        public NetworkVariable<FixedString4096Bytes> textUI = new NetworkVariable<FixedString4096Bytes>();

        void Awake()
        {
            // Initalize or it will break
            currentWeatherMultipliers = new NetworkList<Weather>();
            outsideObjectsToSpawn = new NetworkList<OutsideObjectsToSpawn>();
        }

        public override void OnNetworkSpawn()
        {
            Instance = this;

            UI.SpawnObject(); // Spawn client side UI object

            if (IsServer) // Only call on server
            { 
                InitalizeCurrentWeatherMultipliersServerRpc();
            }

            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            UI.Instance.UnsubscribeFromKeyboardEvent();
            Destroy(GameObject.Find("EventGUI"));

            base.OnNetworkDespawn();
        }


        [ClientRpc]
        public void ClearGameObjectsClientRpc()
        {
            for (int i = 0; i != Manager.objectsToClear.Count; i++)
            {
                if (Manager.objectsToClear[i] != null)
                {
                    NetworkObject netObject = Manager.objectsToClear[i].GetComponent<NetworkObject>();

                    if (netObject != null) // If net object
                    {
                        netObject.Despawn(true);
                    }
                    else // If not net object
                    {
                        Destroy(Manager.objectsToClear[i]);
                    }
                }
            }
            Manager.objectsToClear.Clear(); // clear list
        }

        [ClientRpc]
        public void SyncValuesClientRpc(float factorySizeMultiplier, float scrapValueMultiplier, float scrapAmountMultiplier)
        {
            RoundManager.Instance.currentLevel.factorySizeMultiplier = factorySizeMultiplier;
            RoundManager.Instance.scrapValueMultiplier = scrapValueMultiplier;
            RoundManager.Instance.scrapAmountMultiplier = scrapAmountMultiplier;
            RoundManager.Instance.currentLevel.minScrap = Manager.scrapMinAmount;
            RoundManager.Instance.currentLevel.maxScrap = Manager.scrapMaxAmount;
        }

        [ClientRpc]
        public void ShowCaseEventsClientRpc()
        {
            // Showcase Events
            UI.Instance.curretShowCaseEventTime = UI.Instance.showCaseEventTime;
            UI.Instance.panelBackground.SetActive(true); // Show text
            UI.Instance.panelScrollBar.value = 1.0f; // Start from top
            UI.Instance.showCaseEvents = true;
        }

        [ServerRpc(RequireOwnership = false)]
        private void InitalizeCurrentWeatherMultipliersServerRpc()
        {
            currentWeatherMultipliers = Weather.InitalizeWeatherMultipliers(currentWeatherMultipliers);
            UpdateCurrentWeatherMultipliersServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdateCurrentWeatherMultipliersServerRpc()
        {
            currentWeatherMultipliers = Weather.RandomizeWeatherMultipliers(currentWeatherMultipliers);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "Start")]
        private static void InitalizeServerObject()
        {
            if (netObject != null) return;

            netObject = (GameObject)Assets.bundle.LoadAsset("BrutalCompanyMinus");
            netObject.AddComponent<Net>();

            NetworkManager.Singleton.AddNetworkPrefab(netObject);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Terminal), "Start")]
        private static void SpawnServerObject()
        {
            if (!FindObjectOfType<NetworkManager>().IsServer) return;

            GameObject net = Instantiate(netObject);
            net.GetComponent<NetworkObject>().Spawn(destroyWithScene: false);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartOfRound), "ShipLeave")]
        private static void OnGameEnd()
        {
            if(RoundManager.Instance.IsHost)
            {
                // Randomize weather multipliers
                Instance.UpdateCurrentWeatherMultipliersServerRpc();

                // If called on server
                if (RoundManager.Instance.IsServer)
                {
                    Instance.outsideObjectsToSpawn.Clear();
                    UI.ClearText();
                }
            }
        }
    }

    public struct OutsideObjectsToSpawn : INetworkSerializable, IEquatable<OutsideObjectsToSpawn>
    {
        public float density;
        public int objectEnumID;

        public OutsideObjectsToSpawn(float density, int objectEnumID)
        {
            this.density = density;
            this.objectEnumID = objectEnumID;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                FastBufferReader reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out density);
                reader.ReadValueSafe(out objectEnumID);
            }
            else
            {
                FastBufferWriter write = serializer.GetFastBufferWriter();
                write.WriteValueSafe(density);
                write.WriteValueSafe(objectEnumID);
            }
        }

        public bool Equals(OutsideObjectsToSpawn other)
        {
            return (objectEnumID == other.objectEnumID) && (density == other.density);
        }
    }
}
