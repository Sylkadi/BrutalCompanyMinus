using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Arachnophobia : MEvent
    {
        public override string Name() => nameof(Arachnophobia);

        public static Arachnophobia Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Nightmare facility", "I recommend bringing a vacuum cleaner.", "You are going to want to burn this facility down.", "I wish Zeeker's added a flamethrower." };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(Trees), nameof(LeaflessBrownTrees), nameof(Spiders) };
            EventsToSpawnWith = new List<string>() { nameof(LeaflessTrees) };

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.BunkerSpider,
                new Scale(33.0f, 0.66f, 33.0f, 100.0f),
                new Scale(5.0f, 0.2f, 5.0f, 25.0f),
                new Scale(7.0f, 0.14f, 7.0f, 21.0f),
                new Scale(9.0f, 0.18f, 9.0f, 27.0f),
                new Scale(2.0f, 0.04f, 2.0f, 6.0f),
                new Scale(3.0f, 0.06f, 3.0f, 9.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
