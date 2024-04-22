using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class KamikazieBugs : MEvent
    {
        public override string Name() => nameof(KamikazieBugs);

        public static KamikazieBugs Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "I rather not anger them if I were you.", "Did you know that hoarding bugs have organs? Well these ones have bombs...", "I hope you meet an army of them.", "An invasion of self-destructing pests insdie the facility." };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.kamikazieBug,
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(4.0f, 0.08f, 4.0f, 12.0f),
                new Scale(2.0f, 0.08f, 2.0f, 10.0f),
                new Scale(3.0f, 0.12f, 3.0f, 15.0f),
                new Scale(1.0f, 0.02f, 1.0f, 3.0f),
                new Scale(1.0f, 0.02f, 1.0f, 3.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
