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
            Descriptions = new List<string>() { "There is slighly more scrap in the facility!", "This facility was slighly more productive than others.", "Scrap but more." };
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.1f, 0.007f, 1.1f, 1.8f));
        }

        public override void Execute() => Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);
    }
}
