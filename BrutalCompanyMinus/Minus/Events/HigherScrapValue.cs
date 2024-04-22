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

        public static HigherScrapValue Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 2;
            Descriptions = new List<string>() { "Everything is worth slightly more!", "Premium scrap", "Gucci scrap" };
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.ScrapValue, new Scale(1.1f, 0.007f, 1.1f, 1.8f));
        }

        public override void Execute() => Manager.scrapValueMultiplier *= Getf(ScaleType.ScrapValue);
    }
}
