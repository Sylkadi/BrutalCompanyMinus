using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Handlers
{
    internal class ArtillerySirens : NetworkBehaviour
    {
        public static ArtillerySirens instance;

        public AudioSource sirensClose;

        public AudioSource sirensFar;

        public Transform transform;

        public static float volume = 1.0f;

        [ServerRpc(RequireOwnership = false)]
        public void PlayServerRpc() => PlayClientRpc();

        [ClientRpc]
        public void PlayClientRpc()
        {
            sirensClose.Play();
            sirensFar.Play();
        }

        [ServerRpc(RequireOwnership = false)]
        public void StopServerRpc() => StopClientRpc();

        [ClientRpc]
        public void StopClientRpc()
        {
            sirensClose.Stop();
            sirensFar.Stop();
        }

        public void Start()
        {
            sirensClose.volume = volume;
            sirensFar.volume = volume;
        }

        public static void SpawnInstance()
        {
            Vector3 position = Vector3.zero;
            GameObject ship = GameObject.Find("HangarShip");

            if(ship != null) position = ship.transform.position;

            GameObject sirens = GameObject.Instantiate(Assets.artillerySirens, position, Quaternion.identity);
            sirens.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);

            instance = sirens.GetComponent<ArtillerySirens>();
        }

        public static void DestroyInstance()
        {
            if (instance == null) return;

            NetworkObject netObject = instance.transform.GetComponent<NetworkObject>();

            if(netObject == null) return;

            netObject.Despawn(true);
        } 

    }
}
