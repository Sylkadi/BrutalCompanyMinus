using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class BaboonHorde : MEvent
    {
        public override string Name() => nameof(BaboonHorde);

        public static BaboonHorde Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "You feel outnumbered", "Keep the door's closed.", "Hear their calls, see their shadows cover the land.", "Why are they also inside??" };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.BaboonHawk,
                new Scale(10.0f, 0.34f, 10.0f, 30.0f),
                new Scale(50.0f, 0.84f, 50.0f, 100.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(2.0f, 0.034f, 2.0f, 4.0f),
                new Scale(3.0f, 0.05f, 3.0f, 6.0f),
                new Scale(5.0f, 0.084f, 5.0f, 10.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
