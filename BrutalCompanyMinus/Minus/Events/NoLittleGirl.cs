using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoLittleGirl : MEvent
    {
        public override string Name() => nameof(NoLittleGirl);

        public override void Initalize()
        {
            Weight = 1;
            Description = "No little shits";
            ColorHex = "#008000";
            Type = EventType.Remove;

            EventsToRemove = new List<string>() { nameof(LittleGirl) };
        }

        public override void Execute() => Manager.RemoveSpawn("DressGirl");
    }
}
