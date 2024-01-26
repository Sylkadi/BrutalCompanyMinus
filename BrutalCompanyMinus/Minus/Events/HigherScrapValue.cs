using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class HigherScrapValue : MEvent
    {
        public override string Name() => nameof(HigherScrapValue);

        public override void Initalize()
        {
            Weight = 2;
            Description = "Everything is worth slightly more!";
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.ScrapValue, new Scale(1.20f, 0.005f));
        }

        public override void Execute()
        {
            Manager.scrapValueMultiplier *= Getf(ScaleType.ScrapValue);
        }
    }
}
