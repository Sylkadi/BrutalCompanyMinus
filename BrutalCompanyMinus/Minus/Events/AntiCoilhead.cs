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
                new Scale(20.0f, 0.8f, 20.0f, 100.0f),
                new Scale(5.0f, 0.2f, 5.0f, 25.0f),
                new Scale(2.0f, 0.04f, 2.0f, 6.0f),
                new Scale(2.0f, 0.06f, 2.0f, 8.0f),
                new Scale(0.0f, 0.02f, 0.0f, 1.0f),
                new Scale(0.0f, 0.03f, 0.0f, 3.0f))
            };
        }

        public override void Execute()
        {
            Manager.RemoveSpawn(Assets.EnemyName.CoilHead);
            ExecuteAllMonsterEvents();
        }
    }
}
