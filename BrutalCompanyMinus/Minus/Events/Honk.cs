using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Honk : MEvent
    {
        public override string Name() => nameof(Honk);

        public override void Initalize()
        {
            Weight = 2;
            Description = "Honk!";
            ColorHex = "#008000";
            Type = EventType.Good;

            EventsToRemove = new List<string>() { nameof(TransmuteScrapBig), nameof(TransmuteScrapSmall), nameof(Dentures), nameof(Pickles), nameof(GoldenFacility), nameof(GoldenBars), nameof(CursedGold) };

            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.08f, 0.0015f));
        }

        public override void Execute()
        {
            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);

            RoundManager.Instance.currentLevel.spawnableScrap.Clear();
            RoundManager.Instance.currentLevel.spawnableScrap.Add(Manager.generateItemWithRarity(Assets.GetItem(Assets.ItemName.AirHorn), 50));
            RoundManager.Instance.currentLevel.spawnableScrap.Add(Manager.generateItemWithRarity(Assets.GetItem(Assets.ItemName.ClownHorn), 50));
        }
    }
}
