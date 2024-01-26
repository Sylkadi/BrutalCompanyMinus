using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoWorm : MEvent
    {
        public override string Name() => nameof(NoWorm);

        public override void Initalize()
        {
            Weight = 1;
            Description = "No worms";
            ColorHex = "#008000";
            Type = EventType.Remove;
        }

        public override void Execute() => Manager.RemoveSpawn("SandWorm");
    }
}
