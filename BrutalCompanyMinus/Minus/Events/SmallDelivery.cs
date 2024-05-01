using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class SmallDelivery : MEvent
    {
        public override string Name() => nameof(SmallDelivery);

        public static SmallDelivery Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 2;
            Descriptions = new List<string>() { "The company has decided to give you a present.", "Small shipment for small jobs", "Congratz, you have been awarded." };
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.MinItemAmount, new Scale(2.0f, 0.06f, 2.0f, 8.0f));
            ScaleList.Add(ScaleType.MaxItemAmount, new Scale(3.0f, 0.09f, 3.0f, 12.0f));
            ScaleList.Add(ScaleType.MinValue, new Scale(0.0f, 0.0f, 0.0f, 0.0f));
            ScaleList.Add(ScaleType.MaxValue, new Scale(30.0f, 2.7f, 30.0f, 300.0f));
        }

        public override void Execute() => Manager.DeliverRandomItems(UnityEngine.Random.Range(Get(ScaleType.MinItemAmount), Get(ScaleType.MaxItemAmount) + 1), Get(ScaleType.MinValue), Get(ScaleType.MaxValue));
    }
}
