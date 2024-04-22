using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NutSlayer : MEvent
    {
        public override string Name() => nameof(NutSlayer);

        public static NutSlayer Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "The nut slayer is inside the facility...", "Enjoy", "I should make this thing play doom music.", "Even god wont save you from him." };
            ColorHex = "#280000";
            Type = EventType.VeryBad;

            EventsToSpawnWith = new List<string>() { nameof(Gloomy), nameof(Thumpers), nameof(Spiders), nameof(Masked) };
            EventsToRemove = new List<string>() { nameof(HeavyRain), nameof(Raining) };

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.nutSlayer,
                new Scale(1.0f, 0.04f, 1.0f, 5.0f),
                new Scale(1.0f, 0.02f, 1.0f, 3.0f),
                new Scale(1.0f, 0.0f, 1.0f, 1.0f),
                new Scale(1.0f, 0.0f, 1.0f, 1.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f))
            };

            ScaleList.Add(ScaleType.SpawnMultiplier, new Scale(1.25f, 0.0075f, 1.25f, 2.0f));
            ScaleList.Add(ScaleType.SpawnCapMultiplier, new Scale(1.4f, 0.016f, 1.4f, 3.0f));
        }

        public override void Execute() 
        {
            ExecuteAllMonsterEvents();

            Manager.MultiplySpawnChance(RoundManager.Instance.currentLevel, Getf(ScaleType.SpawnMultiplier));
            Manager.MultiplySpawnCap(Getf(ScaleType.SpawnCapMultiplier));
        } 
    }
}
