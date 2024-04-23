using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class HolidaySeason : MEvent
    {
        public override string Name() => nameof(HolidaySeason);

        public static HolidaySeason Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "It's holiday season", "All the holidays!", "Easter, Halloween and Christmas all in one day." };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.NutCracker,
                new Scale(7.0f, 0.28f, 7.0f, 35.0f),
                new Scale(2.0f, 0.08f, 2.0f, 10.0f),
                new Scale(1.0f, 0.02f, 1.0f, 3.0f),
                new Scale(1.0f, 0.03f, 1.0f, 4.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f)), new MonsterEvent(
                Assets.EnemyName.HoardingBug,
                new Scale(5.0f, 0.2f, 5.0f, 25.0f),
                new Scale(2.0f, 0.04f, 2.0f, 6.0f),
                new Scale(1.0f, 0.04f, 1.0f, 5.0f),
                new Scale(2.0f, 0.05f, 2.0f, 7.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f))
            };

            EventsToRemove = new List<string>() { nameof(Trees), nameof(LeaflessTrees), nameof(LeaflessBrownTrees) };

            scrapTransmutationEvent = new ScrapTransmutationEvent(
                new Scale(0.5f, 0.008f, 0.5f, 0.9f),
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.EasterEgg), rarity = 40 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem(Assets.ItemName.Gift), rarity = 60 }
            );

            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.0f, 0.005f, 1.0f, 1.5f));
            ScaleList.Add(ScaleType.MinDensity, new Scale(0.0018f, 0.0f, 0.0018f, 0.0018f));
            ScaleList.Add(ScaleType.MaxDensity, new Scale(0.0025f, 0.0f, 0.0025f, 0.0025f));
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

            ExecuteAllMonsterEvents();
            scrapTransmutationEvent.Execute();

            Net.Instance.outsideObjectsToSpawn.Add(new OutsideObjectsToSpawn(UnityEngine.Random.Range(Getf(ScaleType.MinDensity), Getf(ScaleType.MaxDensity)), (int)Assets.ObjectName.GiantPumkin));
        }
    }
}
