using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.MonoBehaviours
{
    internal class BunkerLid : NetworkBehaviour
    {
        public Animator animator;

        public AudioSource bunkerAudio;

        public GameObject passageInteractTrigger;

        public InteractTrigger bunkerInteractTrigger;

        public bool state = false;

        public void Start() => passageInteractTrigger.SetActive(false);

        [ServerRpc(RequireOwnership = false)]
        public void ToggleLidServerRpc(bool _state) => ToggleLidClientRpc(_state);

        [ServerRpc(RequireOwnership = false)]
        public void ToggleLidServerRpc() => ToggleLidClientRpc(!state);

        [ClientRpc]
        private void ToggleLidClientRpc(bool newState)
        {
            if (newState == state) return;
            state = newState;

            bunkerAudio.Play();
            animator.SetBool("Open", newState);
            passageInteractTrigger.SetActive(newState);
            bunkerInteractTrigger.hoverTip = newState ? "Close [LMB]" : "Open [LMB]";
        }
    }
}
