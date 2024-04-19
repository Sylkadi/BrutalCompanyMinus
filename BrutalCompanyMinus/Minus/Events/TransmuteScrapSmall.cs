using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class TransmuteScrapSmall : MEvent
    {
        public override string Name() => nameof(TransmuteScrapSmall);

        public static TransmuteScrapSmall Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 2;
            Descriptions = new List<string>() { "All the scrap has transmuted into something small...", "This is going to be a one-handed job", "It's all the light stuff" };
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.0f, 0.005f, 1.0f, 1.3f));

            EventsToRemove = new List<string>() { nameof(RealityShift) };
        }

        public override bool AddEventIfOnly() // If one-handed item exists in item pool
        {
            foreach (SpawnableItemWithRarity item in RoundManager.Instance.currentLevel.spawnableScrap)
            {
                if (item.spawnableItem.twoHanded || Manager.transmuteScrap) continue;
                Manager.transmuteScrap = true;
                return true;
            }
            return false;
        }

        public override void Execute()
        {
            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);

            List<SpawnableItemWithRarity> SmallScrapList = new List<SpawnableItemWithRarity>();
            foreach (SpawnableItemWithRarity item in RoundManager.Instance.currentLevel.spawnableScrap)
            {
                if (!item.spawnableItem.twoHanded) SmallScrapList.Add(item);
            }

            SpawnableItemWithRarity chosenScrap = SmallScrapList[UnityEngine.Random.Range(0, SmallScrapList.Count)];
            chosenScrap.spawnableItem = Assets.GetItem(chosenScrap.spawnableItem.name);

            Manager.TransmuteScrap(new SpawnableItemWithRarity() { spawnableItem = chosenScrap.spawnableItem, rarity = 100 });

            // Scale scrap amount abit more
            float scrapValue = (chosenScrap.spawnableItem.minValue + chosenScrap.spawnableItem.maxValue) * 0.25f; // Intentionally
            if (scrapValue <= 0) scrapValue = 40;
            Manager.scrapAmountMultiplier *= Mathf.Clamp(Mathf.Log(Assets.averageScrapValueList[Manager.GetLevelIndex()] / scrapValue, 5) + 1, 1.0f, 4.0f); // Range : [1.0f, 4.0f]
        }
    }
}
