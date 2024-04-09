using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class RealityShift : MEvent
    {
        public static bool Active = false;
        public override string Name() => nameof(RealityShift);

        public static RealityShift Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Reality is not what it seems", "The scrap might turn into a landmine", "I hope you are ready to cry", "You might be lucky" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;
        }

        public override void Execute() => Net.Instance.SetRealityShiftActiveServerRpc(true);

        public override void OnShipLeave() => Net.Instance.SetRealityShiftActiveServerRpc(false);

        public override void OnGameStart() => Active = false;
    }
}
