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

        public static BigDelivery Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "The company has ordered a big delivery on this planet.", "A Luxurious cargo descends.", "Corporate has sent you a massive package." };
            ColorHex = "#00FF00";
            Type = EventType.VeryGood;

            EventsToRemove = new List<string>() { nameof(SmallDelivery) };

            ScaleList.Add(ScaleType.MinItemAmount, new Scale(5.0f, 0.1f, 5.0f, 15.0f));
            ScaleList.Add(ScaleType.MaxItemAmount, new Scale(7.0f, 0.14f, 7.0f, 21.0f));
            ScaleList.Add(ScaleType.MinValue, new Scale(60.0f, 0.0f, 60.0f, 60.0f));
            ScaleList.Add(ScaleType.MaxValue, new Scale(99999.0f, 0.0f, 99999.0f, 99999.0f));
        }

        public override void Execute() => Manager.DeliverRandomItems(UnityEngine.Random.Range(Get(ScaleType.MinItemAmount), Get(ScaleType.MaxItemAmount) + 1), Get(ScaleType.MinValue), Get(ScaleType.MaxValue));
    }
}
