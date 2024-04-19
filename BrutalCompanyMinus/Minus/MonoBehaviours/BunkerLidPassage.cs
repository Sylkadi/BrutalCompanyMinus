using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.MonoBehaviours
{
    internal class BunkerLidPassage : Passage
    {
        internal BunkerLid bunkerLid;

        public override void TeleportPlayer()
        {
            base.TeleportPlayer();

            PlayAudioAtTeleportPositionsServerRpc();
            bunkerLid.ToggleLidServerRpc(false);
        }
    }
}
