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

        public override void Initalize()
        {
            Weight = 2;
            Description = "All the scrap has transmuted into something small...";
            ColorHex = "#008000";
            Type = EventType.Good;

            EventsToRemove = new List<string>() { nameof(TransmuteScrapBig), nameof(Dentures), nameof(Pickles), nameof(GoldenFacility), nameof(Honk), nameof(GoldenBars)};
        }

        public override bool AddEventIfOnly() // If one-handed item exists in item pool
        {
            foreach (SpawnableItemWithRarity item in RoundManager.Instance.currentLevel.spawnableScrap)
            {
                if (!item.spawnableItem.twoHanded) return true;
            }
            return false;
        }

        public override void Execute()
        {
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
            Manager.scrapAmountMultiplier *= Functions.Range(Mathf.Log(Assets.averageScrapValueList[Manager.GetLevelIndex()] / scrapValue, 5) + 1, 1.0f, 2.0f); // Range : [1.0f, 2.0f]
        }
    }
}
