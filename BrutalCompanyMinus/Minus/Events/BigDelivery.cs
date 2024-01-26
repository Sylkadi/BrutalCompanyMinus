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

            ScaleList.Add(ScaleType.MinItemAmount, new Scale(7.0f, 0.07f));
            ScaleList.Add(ScaleType.MaxItemAmount, new Scale(10.0f, 0.12f));
            ScaleList.Add(ScaleType.MaxValue, new Scale(99999.0f, 0.0f));
        }

        public override void Execute() => Manager.DeliverRandomItems(UnityEngine.Random.Range(Get(ScaleType.MinItemAmount), Get(ScaleType.MaxItemAmount) + 1), Get(ScaleType.MaxValue));
    }
}
