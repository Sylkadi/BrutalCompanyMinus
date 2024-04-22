using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Dice : MEvent
    {
        public override string Name() => nameof(Dice);

        public static Dice Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 2;
            Descriptions = new List<string>() { "Dice!!!!", "Did you know that 90% of gamblers quit just before they hit big?", "50/50", "Questionable luck" };
            ColorHex = "#008000";
            Type = EventType.Good;

            scrapTransmutationEvent = new ScrapTransmutationEvent(
                new Scale(0.5f, 0.008f, 0.5f, 0.9f),
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("Saint"), rarity = 4 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("Chronos"), rarity = 10 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("Rusty"), rarity = 45 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("MysteryDiceItem"), rarity = 25 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("Sacrificer"), rarity = 16 }
            );

            EventsToRemove = new List<string>() { nameof(RealityShift) };

            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.0f, 0.005f, 1.0f, 1.5f));
        }

        public override bool AddEventIfOnly()
        {
            if (!Compatibility.emergencyDicePresent) return false;
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
