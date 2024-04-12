using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Dentures : MEvent
    {
        public override string Name() => nameof(Dentures);

        public static Dentures Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 2;
            Descriptions = new List<string>() { "Grandma forgot her dentures...", "The tooth fairies dream", "It grins from every corner" };
            ColorHex = "#008000";
            Type = EventType.Good;

            EventsToRemove = new List<string>() { nameof(RealityShift) };

            scrapTransmutationEvent = new ScrapTransmutationEvent(
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.Teeth), rarity = 100 }
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
