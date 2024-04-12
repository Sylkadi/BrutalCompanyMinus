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
                Assets.antiCoilHead,
                new Scale(50.0f, 0.84f, 50.0f, 100.0f),
                new Scale(10.0f, 0.34f, 10.0f, 30.0f),
                new Scale(4.0f, 0.084f, 3.0f, 9.0f),
                new Scale(6.0f, 0.134f, 4.0f, 14.0f),
                new Scale(2.0f, 0.034f, 1.0f, 4.0f),
                new Scale(3.0f, 0.05f, 3.0f, 6.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
