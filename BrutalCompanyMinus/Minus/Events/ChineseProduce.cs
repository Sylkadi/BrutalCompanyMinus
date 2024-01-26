using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class ChineseProduce : MEvent
    {
        public override string Name() => nameof(ChineseProduce);

        public override void Initalize()
        {
            Weight = 1;
            Description = "Everything here is made cheaply...";
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            ScaleList.Add(ScaleType.ScrapValue, new Scale(0.6f, 0.0f));
            ScaleList.Add(ScaleType.ScrapAmount, new Scale(2.0f, 0.0f));
        }

        public override void Execute()
        {
            Manager.scrapValueMultiplier *= Getf(ScaleType.ScrapValue);
            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);
        }
    }
}
