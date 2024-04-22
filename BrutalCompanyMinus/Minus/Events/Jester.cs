using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Jester : MEvent
    {
        public override string Name() => nameof(Jester);

        public static Jester Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "I want to go home", "Lovely...", "Freeee birdd" };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.Jester,
                new Scale(10.0f, 0.34f, 10.0f, 50.0f),
                new Scale(5.0f, 0.167f, 5.0f, 25.0f),
                new Scale(2.0f, 0.03f, 2.0f, 5.0f),
                new Scale(2.0f, 0.05f, 2.0f, 7.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.02f, 0.0f, 1.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
