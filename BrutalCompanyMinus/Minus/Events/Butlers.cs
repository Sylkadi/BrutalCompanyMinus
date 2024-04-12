using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Butlers : MEvent
    {
        public override string Name() => nameof(Butlers);

        public static Butlers Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Nicely fashioned gentlemen", "A touch of class descends upon this planet", "Pop!", "Knives" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.Butler,
                new Scale(25.0f, 0.417f, 25.0f, 50.0f),
                new Scale(5.0f, 0.17f, 5.0f, 15.0f),
                new Scale(1.0f, 0.017f, 1.0f, 3.0f),
                new Scale(2.0f, 0.034f, 2.0f, 6.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
