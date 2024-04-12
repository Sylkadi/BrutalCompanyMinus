using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Spiders : MEvent
    {
        public override string Name() => nameof(Spiders);

        public static Spiders Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Your skin crawls...", "Not for those with arachnophobia", "Bring your hoover" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            EventsToRemove = new List<string>() { nameof(Trees), nameof(LeaflessTrees) };
            EventsToSpawnWith = new List<string>() { nameof(LeaflessBrownTrees) };

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.BunkerSpider,
                new Scale(25.0f, 0.417f, 25.0f, 50.0f),
                new Scale(4.0f, 0.134f, 4.0f, 12.0f),
                new Scale(2.0f, 0.084f, 2.0f, 4.0f),
                new Scale(2.0f, 0.1f, 2.0f, 6.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(2.0f, 0.034f, 2.0f, 4.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
