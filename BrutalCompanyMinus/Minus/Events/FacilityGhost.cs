using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class FacilityGhost : MEvent
    {
        public static bool Active = false;

        public override string Name() => nameof(FacilityGhost);

        public static FacilityGhost Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "There is a ghost in the facility", "Who keeps turning the lights off...", "Paranormal signature detected", "Bring your ouija board" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;
        }

        public override void Execute() => Active = true;

        public override void OnShipLeave() => Active = false;

        public override void OnGameStart() => Active = false;
    }
}
