using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class ShipmentFees : MEvent
    {
        public static bool Active = false;
        public override string Name() => nameof(ShipmentFees);

        public override void Initalize()
        {
            Weight = 3;
            Description = "The company is now incurring a fee for shipments!";
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.MinCut, new Scale(0.1f, 0.0034f, 0.1f, 0.3f));
            ScaleList.Add(ScaleType.MaxCut, new Scale(0.2f, 0.0067f, 0.2f, 0.6f));
        }

        public override void Execute() => Active = true;

        public override void OnShipLeave() => Active = false;

        public override void OnGameStart() => Active = false;
    }
}
