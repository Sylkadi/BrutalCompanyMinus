using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Pickles : MEvent
    {
        public override string Name() => nameof(Pickles);

        public override void Initalize()
        {
            Weight = 2;
            Description = "Tastes salty...";
            ColorHex = "#008000";
            Type = EventType.Good;

            EventsToRemove = new List<string>() { nameof(TransmuteScrapBig), nameof(TransmuteScrapSmall), nameof(Dentures), nameof(GoldenFacility), nameof(Honk), nameof(GoldenBars)};

            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.0f, 0.005f, 1.0f, 1.3f));
        }

        public override void Execute()
        {
            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);
            Manager.TransmuteScrap(new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.JarOfPickles), rarity = 100 });
        }
    }
}
