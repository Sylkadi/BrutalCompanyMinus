using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class GoldenBars : MEvent
    {
        public override string Name() => nameof(GoldenBars);

        public override void Initalize()
        {
            Weight = 1;
            Description = "Bling bling";
            ColorHex = "#00FF00";
            Type = EventType.VeryGood;

            EventsToRemove = new List<string>() { nameof(TransmuteScrapBig), nameof(TransmuteScrapSmall), nameof(Dentures), nameof(Pickles), nameof(GoldenFacility), nameof(Honk)};

            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.0f, 0.005f, 1.0f, 1.3f));
        }

        public override void Execute()
        {
            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);
            Manager.TransmuteScrap(new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.GoldBar), rarity = 100 });
        }
    }
}
