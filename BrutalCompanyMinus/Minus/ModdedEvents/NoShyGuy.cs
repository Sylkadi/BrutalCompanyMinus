using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoShyGuy : MEvent
    {
        public override string Name() => nameof(NoShyGuy);

        public static NoShyGuy Instance;

        public override void Initalize()
        {
            Instance = this;

            EventsToRemove = new List<string>() { nameof(Hell), nameof(ShyGuy) };

            Weight = 1;
            Descriptions = new List<string>() { "SCP-096 is contained.", "You can open your eyes" };
            ColorHex = "#008000";
            Type = EventType.Remove;
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists("ShyGuyDef") && Compatibility.scopophobiaPresent;

        public override void Execute() => Manager.RemoveSpawn("ShyGuyDef");
    }
}
