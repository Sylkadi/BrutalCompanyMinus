using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class ScrapGalore : MEvent
    {
        public override string Name() => nameof(ScrapGalore);

        public static ScrapGalore Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Description = "Scrap here is plentiful and of high quality.";
            ColorHex = "#00FF00";
            Type = EventType.VeryGood;

            ScaleList.Add(ScaleType.ScrapValue, new Scale(1.35f, 0.0117f, 1.35f, 2.05f));
            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.35f, 0.0117f, 1.35f, 2.05f));

            EventsToRemove = new List<string>() { nameof(HigherScrapValue), nameof(MoreScrap) };
        }

        public override void Execute()
        {
            Manager.scrapValueMultiplier *= Getf(ScaleType.ScrapValue);
            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);
        }
    }
}
