using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class BlackFriday : MEvent
    {
        public override string Name() => nameof(BlackFriday);

        public static BlackFriday Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Everything is on sale!!!!", "The marketplace is set ablaze with these discounts", "Dont miss out on these deals!" };
            ColorHex = "#00FF00";
            Type = EventType.VeryGood;

            EventsToRemove = new List<string>() { nameof(ShipmentFees) };

            ScaleList.Add(ScaleType.MinPercentageCut, new Scale(25.0f, 0.584f, 25.0f, 60.0f));
            ScaleList.Add(ScaleType.MaxPercentageCut, new Scale(55.0f, 0.584f, 50.0f, 90.0f));
        }

        public override void Execute() => Net.Instance.BlackFridayServerRpc(Get(ScaleType.MinPercentageCut), Get(ScaleType.MaxPercentageCut));
    }
}
