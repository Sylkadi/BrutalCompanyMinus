using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoHoardingBugs : MEvent
    {
        public override string Name() => nameof(NoHoardingBugs);

        public override void Initalize()
        {
            Weight = 1;
            Description = "No romani";
            ColorHex = "#008000";
            Type = EventType.Remove;

            EventsToRemove = new List<string>() { nameof(HoardingBugs), nameof(GypsyColony) };
        }

        public override void Execute() => Manager.RemoveSpawn("HoarderBug");
    }
}
