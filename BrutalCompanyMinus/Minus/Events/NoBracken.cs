using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoBracken : MEvent
    {
        public override string Name() => nameof(NoBracken);

        public override void Initalize()
        {
            Weight = 1;
            Description = "No stalkers";
            ColorHex = "#008000";
            Type = EventType.Remove;

            EventsToRemove = new List<string>() { nameof(Bracken) };
        }

        public override void Execute() => Manager.RemoveSpawn("Flowerman");
    }
}
