using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using HarmonyLib;
using BrutalCompanyMinus.Minus;
using Unity.Collections;
using GameNetcodeStuff;

namespace BrutalCompanyMinus
{
    [HarmonyPatch]
    internal class Net : NetworkBehaviour
    {
        public static Net Instance { get; private set; }
        public static GameObject netObject { get; private set; }

        public NetworkList<Weather> currentWeatherMultipliers;
        public NetworkList<OutsideObjectsToSpawn> outsideObjectsToSpawn;
        public NetworkList<CurrentWeatherEffect> currentWeatherEffects;

        public NetworkVariable<FixedString4096Bytes> textUI = new NetworkVariable<FixedString4096Bytes>();

        void Awake()
        {
            // Initalize or it will break
            currentWeatherMultipliers = new NetworkList<Weather>();
            outsideObjectsToSpawn = new NetworkList<OutsideObjectsToSpawn>();
            currentWeatherEffects = new NetworkList<CurrentWeatherEffect>();
        }

        void FixedUpdate()
        {
            if(currentWeatherEffects.Count > 0) // Set atmosphere
            {
                foreach(CurrentWeatherEffect e in currentWeatherEffects)
                {
                    PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;
                    if(localPlayer != null)
                    {
                        if(!localPlayer.isInsideFactory) UpdateAtmosphere(e.name, e.state);
                    }
                }
            }
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
        }

        [ServerRpc]
        public void SyncScrapValueServerRpc(NetworkObjectReference obj, int value)
        {
            SyncScrapValueClientRpc(obj, value);
        }

        [ClientRpc]
        private void SyncScrapValueClientRpc(NetworkObjectReference obj, int value)
        {
            obj.TryGet(out NetworkObject netObj);
            netObj.GetComponent<GrabbableObject>().SetScrapValue(value);
        }

        private void UpdateAtmosphere(FixedString128Bytes name, bool state)
        {
            for (int i = 0; i < TimeOfDay.Instance.effects.Length; i++)
            {
                if (TimeOfDay.Instance.effects[i].name == name)
                {
                    TimeOfDay.Instance.effects[i].effectEnabled = state;
                }
            }
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
                if (RoundManager.Instance.IsServer) // Why did i write if is Server when i already check if host?? just gona leave this here.
                {
                    Instance.currentWeatherEffects.Clear(); // Clear weather effects
                    Instance.outsideObjectsToSpawn.Clear();
                    UI.ClearText();
                }
            }
        }
    }

    public struct CurrentWeatherEffect : INetworkSerializable, IEquatable<CurrentWeatherEffect>
    {
        public FixedString128Bytes name;
        public bool state;

        public CurrentWeatherEffect(FixedString128Bytes name, bool state)
        {
            this.name = name;
            this.state = state;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                FastBufferReader reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out name);
                reader.ReadValueSafe(out state);
            }
            else
            {
                FastBufferWriter write = serializer.GetFastBufferWriter();
                write.WriteValueSafe(name);
                write.WriteValueSafe(state);
            }
        }

        public bool Equals(CurrentWeatherEffect other)
        {
            return name == other.name;
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
