using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoFiend : MEvent
    {
        public override string Name() => nameof(NoFiend);

        public static NoFiend Instance;

        public override void Initalize()
        {
            Instance = this;

            EventsToRemove = new List<string>() { nameof(Hell), nameof(NoFiend) };

            Weight = 1;
            Descriptions = new List<string>() { "No Fiends", "No jumpscares... I think", "No thing", "Flashing is allowed on this moon!!!" };
            ColorHex = "#008000";
            Type = EventType.Remove;
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists("TheFiend") && Compatibility.theFiendPresent;

        public override void Execute() => Manager.RemoveSpawn("TheFiend");
    }
}
