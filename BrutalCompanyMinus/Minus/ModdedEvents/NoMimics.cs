using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoMimics : MEvent
    {
        public override string Name() => nameof(NoMimics);

        public static NoMimics Instance;

        public override void Initalize()
        {
            Instance = this;

            EventsToRemove = new List<string>() { nameof(Hell), nameof(ShyGuy) };

            Weight = 1;
            Descriptions = new List<string>() { "The fire exit's are safe", "No Mimics!" };
            ColorHex = "#008000";
            Type = EventType.Remove;
        }

        public override bool AddEventIfOnly() => Compatibility.mimicsPresent;

        public override void Execute() => Handlers.Mimics.NoMimics();

        public override void OnShipLeave() => Handlers.Mimics.Reset();

        public override void OnGameStart() => Handlers.Mimics.Reset();
    }
}
