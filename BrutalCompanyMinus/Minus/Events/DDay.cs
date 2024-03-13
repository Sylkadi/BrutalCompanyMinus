using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.XR.OpenVR;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class DDay : MEvent
    {
        public static bool Active = false;
        public override string Name() => nameof(DDay);

        public static DDay Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Description = "You better start praying";
            ColorHex = "#FF0000";
            Type = EventType.Bad;
        }

        public override void Execute()
        {
            Handlers.DDay.currentTime = Handlers.DDay.bombardmentInterval;
            Active = true;
        }

        public override void OnShipLeave()
        {
            if(Handlers.ArtillerySirens.instance != null)
            {
                Handlers.ArtillerySirens.instance.StopServerRpc();
            }
            Active = false;
        }

        public override void OnGameStart() => Active = false;
    }
}
