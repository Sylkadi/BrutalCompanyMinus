using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class CityOfGold : MEvent
    {
        public override string Name() => nameof(CityOfGold);

        public static CityOfGold Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 2;
            Descriptions = new List<string>() { "Everything is golden!!", "Gold rush!", "The nigerian princes stash!" };
            ColorHex = "#00FF00";
            Type = EventType.VeryGood;

            EventsToRemove = new List<string>() { nameof(GoldenBars) };

            scrapTransmutationEvent = new ScrapTransmutationEvent(
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldenEggbeaterLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldMugLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldJugLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("PurifiedMaskLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldenBellLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldenHornLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GolderBarLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("TalkativeGoldBarLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("JacobsLadderLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldRegisterLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldAxleLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("CuddlyGoldLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldenGruntLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldNuggetGoldScrapShop"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("TiltControlsLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldenGlassLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("CookieGoldPanLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldSignLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldSpringLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldkeeperLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldenFlaskLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldToyRobotLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldenBootsLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldTypeEngineLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldenGuardianLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("ComedyGoldLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldPuzzleLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldBoltLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("GoldenAirhornLCGoldScrapMod"), rarity = 1 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("DuckOfGoldLCGoldScrapMod"), rarity = 1 }
            );

            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.0f, 0.005f, 1.0f, 1.3f));
        }

        public override bool AddEventIfOnly()
        {
            if (!Compatibility.goldScrapPresent) return false;
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
