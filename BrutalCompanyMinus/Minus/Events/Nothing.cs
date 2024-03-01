using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Nothing : MEvent
    {
        public override string Name() => nameof(Nothing);

        public static Nothing Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 8;
            Description = "--Nothing--";
            ColorHex = "#FFFFFF";
            Type = EventType.Neutral;
        }
    }
}
