using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class BigDelivery : MEvent
    {
        public override string Name() => nameof(BigDelivery);

        public override void Initalize()
        {
            Weight = 1;
            Description = "The company has ordered a big delivery on this planet.";
            ColorHex = "#00FF00";
            Type = EventType.VeryGood;

            EventsToRemove = new List<string>() { nameof(SmallDeilvery) };

            ScaleList.Add(ScaleType.MinItemAmount, new Scale(4.0f, 0.134f, 4.0f, 12.0f));
            ScaleList.Add(ScaleType.MaxItemAmount, new Scale(6.0f, 0.2f, 6.0f, 18.0f));
            ScaleList.Add(ScaleType.MaxValue, new Scale(99999.0f, 0.0f, 99999.0f, 99999.0f));
        }

        public override void Execute() => Manager.DeliverRandomItems(UnityEngine.Random.Range(Get(ScaleType.MinItemAmount), Get(ScaleType.MaxItemAmount) + 1), Get(ScaleType.MaxValue));
    }
}
