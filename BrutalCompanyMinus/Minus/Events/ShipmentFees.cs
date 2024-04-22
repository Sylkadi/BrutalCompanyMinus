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

        public static ShipmentFees Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "The company is now incurring a fee for shipments!", "You are going to be taxed for any shipments...", "I don't recommend buying anything today.", "You might go into debt, beware" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.MinCut, new Scale(0.2f, 0.004f, 0.2f, 0.6f));
            ScaleList.Add(ScaleType.MaxCut, new Scale(0.3f, 0.006f, 0.3f, 0.9f));
        }

        public override void Execute() => Active = true;

        public override void OnShipLeave() => Active = false;

        public override void OnGameStart() => Active = false;
    }
}
