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

        public static TransmuteScrapBig Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Most scrap has transmuted into something big...", "Everything is heavy...", "Bring your carts!!!", "This is going to be a two-handed job" };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.0f, 0.005f, 1.0f, 1.5f));
            ScaleList.Add(ScaleType.Percentage, new Scale(0.5f, 0.005f, 0.5f, 0.75f));

            EventsToRemove = new List<string>() { nameof(RealityShift) };
        }

        public override bool AddEventIfOnly() // If two-handed item exists in item pool
        {
            foreach (SpawnableItemWithRarity item in RoundManager.Instance.currentLevel.spawnableScrap)
            {
                if (!item.spawnableItem.twoHanded || Manager.transmuteScrap) continue;
                Manager.transmuteScrap = true;
                return true;
            }
            return false;
        }

        public override void Execute()
        {
            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);

            List<SpawnableItemWithRarity> BigScrapList = new List<SpawnableItemWithRarity>();
            foreach (SpawnableItemWithRarity item in RoundManager.Instance.currentLevel.spawnableScrap)
            {
                if (item.spawnableItem.twoHanded) BigScrapList.Add(item);
            }

            SpawnableItemWithRarity chosenScrap = BigScrapList[UnityEngine.Random.Range(0, BigScrapList.Count)];
            chosenScrap.spawnableItem = Assets.GetItem(chosenScrap.spawnableItem.name);

            Manager.TransmuteScrap(Getf(ScaleType.Percentage), new SpawnableItemWithRarity() { spawnableItem = chosenScrap.spawnableItem, rarity = 100 });

            // Scale scrap amount abit more
            float scrapValue = (chosenScrap.spawnableItem.minValue + chosenScrap.spawnableItem.maxValue) * 0.25f; // Intentionally
            if (scrapValue <= 0) scrapValue = 40;
            Manager.scrapAmountMultiplier *= Mathf.Clamp(Mathf.Log(Assets.averageScrapValueList[Manager.GetLevelIndex()] / scrapValue, 5) + 1, 1.0f, 4.0f); // Range : [1.0f, 4.0f]
        }
    }
}
