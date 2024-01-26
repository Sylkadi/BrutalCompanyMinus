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

            EventsToRemove = new List<string>() { nameof(TransmuteScrapBig), nameof(TransmuteScrapSmall), nameof(Dentures), nameof(Pickles), nameof(Honk), nameof(GoldenBars), nameof(CursedGold) };

            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.08f, 0.0015f));
        }

        public override void Execute()
        {
            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);

            RoundManager.Instance.currentLevel.spawnableScrap.Clear();

            List<SpawnableItemWithRarity> ScrapToSpawn = new List<SpawnableItemWithRarity>
            {
                    Manager.generateItemWithRarity(Assets.GetItem(Assets.ItemName.GoldenCup), 25),
                    Manager.generateItemWithRarity(Assets.GetItem(Assets.ItemName.Ring), 20),
                    Manager.generateItemWithRarity(Assets.GetItem(Assets.ItemName.GoldBar), 2),
                    Manager.generateItemWithRarity(Assets.GetItem(Assets.ItemName.FancyLamp), 10),
                    Manager.generateItemWithRarity(Assets.GetItem(Assets.ItemName.PerfumeBottle), 25),
                    Manager.generateItemWithRarity(Assets.GetItem(Assets.ItemName.Painting), 15),
                    Manager.generateItemWithRarity(Assets.GetItem(Assets.ItemName.CashRegister), 3)
            };

            foreach (SpawnableItemWithRarity scrap in ScrapToSpawn) RoundManager.Instance.currentLevel.spawnableScrap.Add(scrap);
        }
    }
}
