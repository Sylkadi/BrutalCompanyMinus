using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Bounty : MEvent
    {
        public override string Name() => nameof(Bounty);

        public override void Initalize()
        {
            Weight = 2;
            Description = "The company is now paying for kills";
            ColorHex = "#008000";
            Type = EventType.Good;
        }

        public override void Execute()
        {
            Manager.BountyActive = true;
        }
    }
}
