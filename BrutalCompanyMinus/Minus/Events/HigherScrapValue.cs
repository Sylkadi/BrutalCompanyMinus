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
            Description = "Everything is worth slightly more!";
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.ScrapValue, new Scale(1.2f, 0.0067f, 1.2f, 1.6f));
        }

        public override void Execute() => Manager.scrapValueMultiplier *= Getf(ScaleType.ScrapValue);
    }
}
