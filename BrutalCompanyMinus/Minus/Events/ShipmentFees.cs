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
        public override string Name() => nameof(ShipmentFees);

        public override void Initalize()
        {
            Weight = 3;
            Description = "The company is now incurring a fee for shipments!";
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.MinCash, new Scale(0.10f, 0.0f));
            ScaleList.Add(ScaleType.MaxCash, new Scale(0.25f, 0.0f));
        }

        public override void Execute() => Manager.ShipmentFees = true;
    }
}
