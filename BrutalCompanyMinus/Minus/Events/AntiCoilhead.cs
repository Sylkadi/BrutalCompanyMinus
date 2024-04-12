using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class AntiCoilhead : MEvent
    {
        public override string Name() => nameof(AntiCoilhead);

        public static AntiCoilhead Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Dont look at them...", "Glowy eyes", "My favourite", "I hope you don't have friends when dealing with these" };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(Coilhead), nameof(LeaflessBrownTrees), nameof(Trees), nameof(HeavyRain) };
            EventsToSpawnWith = new List<string>() { nameof(LeaflessTrees), nameof(Gloomy) };

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.antiCoilHead,
                new Scale(25.0f, 0.84f, 25.0f, 75.0f),
                new Scale(10.0f, 0.34f, 10.0f, 30.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(1.0f, 0.05f, 1.0f, 4.0f),
                new Scale(1.0f, 0.017f, 1.0f, 2.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
