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

        public static GoldenFacility Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 2;
            Descriptions = new List<string>() { "The scrap looks shiny", "Valuable scrap ahead", "This facility is rich" };
            ColorHex = "#008000";
            Type = EventType.Good;

            EventsToRemove = new List<string>() { nameof(RealityShift) };

            scrapTransmutationEvent = new ScrapTransmutationEvent(
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.GoldenCup), rarity = 25 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.Ring), rarity = 20 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.GoldBar), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.FancyLamp), rarity = 10 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.PerfumeBottle), rarity = 26 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.Painting), rarity = 15 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.CashRegister), rarity = 3 }
            );

            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.0f, 0.005f, 1.0f, 1.3f));
        }

        public override bool AddEventIfOnly()
        {
            if (!Manager.transmuteScrap)
            {
                Manager.transmuteScrap = true;
                return true;
            }
            return false;
        }

        public override void Execute()
        {
            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);
            scrapTransmutationEvent.Execute();
        }
    }
}
