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
    internal class GoldenFacility : MEvent
    {
        public override string Name() => nameof(GoldenFacility);

        public override void Initalize()
        {
            Weight = 2;
            Description = "The scrap looks shiny";
            ColorHex = "#008000";
            Type = EventType.Good;

            EventsToRemove = new List<string>() { nameof(TransmuteScrapBig), nameof(TransmuteScrapSmall), nameof(Dentures), nameof(Pickles), nameof(Honk), nameof(GoldenBars)};

            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.08f, 0.0015f));
        }

        public override void Execute()
        {
            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);
            Manager.TransmuteScrap(
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.GoldenCup), rarity = 25 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.Ring), rarity = 20 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.GoldBar), rarity = 2 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.FancyLamp), rarity = 10 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.PerfumeBottle), rarity = 25 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.Painting), rarity = 15 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.CashRegister), rarity = 3 }
            );
        }
    }
}
