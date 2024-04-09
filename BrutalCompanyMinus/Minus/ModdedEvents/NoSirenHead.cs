using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoSirenHead : MEvent
    {
        public override string Name() => nameof(NoSirenHead);

        public static NoSirenHead Instance;

        public override void Initalize()
        {
            Instance = this;

            EventsToRemove = new List<string>() { nameof(Hell), nameof(SirenHead) };

            Weight = 1;
            Descriptions = new List<string>() { "You feel safe.", "God wont be here to speak with you." };
            ColorHex = "#008000";
            Type = EventType.Remove;
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists("SirenHead") && Compatibility.sirenheadPresent;

        public override void Execute() => Manager.RemoveSpawn("SirenHead");
    }
}
