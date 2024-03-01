using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class MoreScrap : MEvent
    {
        public override string Name() => nameof(MoreScrap);

        public static MoreScrap Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 2;
            Description = "There is slighly more scrap in the facility!";
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.2f, 0.0067f, 1.2f, 1.6f));
        }

        public override void Execute() => Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);
    }
}
