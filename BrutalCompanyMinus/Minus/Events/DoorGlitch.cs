using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class DoorGlitch : MEvent
    {
        public static bool Active = false;

        public override string Name() => nameof(DoorGlitch);

        public override void Initalize()
        {
            Weight = 3;
            Description = "There is a ghost in the facility";
            ColorHex = "#FF0000";
            Type = EventType.Bad;
        }

        public override void Execute() => Active = true;

        public override void OnShipLeave() => Active = false;

        public override void OnGameStart() => Active = false;
    }
}
