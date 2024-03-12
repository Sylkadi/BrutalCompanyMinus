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
            ColorHex = "#800000";
            Type = EventType.VeryBad;
        }

        public override void Execute() => Active = true;

        public override void OnShipLeave() => Active = false;

        public override void OnGameStart() => Active = false;
    }
}
