using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class CursedGold : MEvent
    {
        public override string Name() => nameof(CursedGold);

        public override void Initalize()
        {
            Weight = 1;
            Description = "Its tempting...";
            ColorHex = "#FF0000";
            Type = EventType.Bad;
        }

        public override void Execute()
        {
            
        }
    }
}
