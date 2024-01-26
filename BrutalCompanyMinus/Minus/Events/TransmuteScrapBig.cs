using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class TransmuteScrapBig : MEvent
    {
        public override string Name() => nameof(TransmuteScrapBig);


        public override void Initalize()
        {
            Weight = 1;
            Description = "All the scrap has transmuted into something big...";
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(TransmuteScrapSmall), nameof(Dentures), nameof(Pickles), nameof(GoldenFacility), nameof(Honk), nameof(GoldenBars), nameof(CursedGold) };

            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.08f, 0.0015f));
        }

        public override bool AddEventIfOnly() // If two-handed item exists in item pool
        {
            foreach(SpawnableItemWithRarity item in RoundManager.Instance.currentLevel.spawnableScrap)
            {
                if (item.spawnableItem.twoHanded) return true;
            }
            return false;
        }

        public override void Execute()
        {
            // Remove all small scrap
            List<SpawnableItemWithRarity> BigScrapList = new List<SpawnableItemWithRarity>();
            foreach (SpawnableItemWithRarity item in RoundManager.Instance.currentLevel.spawnableScrap)
            {
                if (!item.spawnableItem.twoHanded) BigScrapList.Add(item);
            }
            foreach (SpawnableItemWithRarity item in BigScrapList)
            {
                RoundManager.Instance.currentLevel.spawnableScrap.Remove(item);
            }

            SpawnableItemWithRarity chosenScrap = RoundManager.Instance.currentLevel.spawnableScrap[UnityEngine.Random.Range(0, RoundManager.Instance.currentLevel.spawnableScrap.Count)];
            chosenScrap.spawnableItem = Assets.GetItem(chosenScrap.spawnableItem.name);

            RoundManager.Instance.currentLevel.spawnableScrap.RemoveAll(x => x.spawnableItem.name != chosenScrap.spawnableItem.name);

            foreach (SpawnableItemWithRarity item in RoundManager.Instance.currentLevel.spawnableScrap)
            {
                if (chosenScrap.spawnableItem.name == item.spawnableItem.name) item.rarity = 100;
            }

            // Scale scrap amount abit more
            Manager.scrapMinAmount += RoundManager.Instance.currentLevel.minTotalScrapValue / chosenScrap.spawnableItem.minValue;
            Manager.scrapMaxAmount += RoundManager.Instance.currentLevel.maxTotalScrapValue / chosenScrap.spawnableItem.maxValue;
            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);
        }

    }
}
