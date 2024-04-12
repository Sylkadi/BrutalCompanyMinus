using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Bracken : MEvent
    {
        public override string Name() => nameof(Bracken);

        public static Bracken Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Your neck tingles", "Your local chiropractors", "I hope you have nyctophobia", "You wont win this staring competition" };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.Bracken,
                new Scale(25.0f, 0.84f, 25.0f, 75.0f),
                new Scale(5.0f, 0.167f, 5.0f, 15.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(2.0f, 0.067f, 2.0f, 6.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
