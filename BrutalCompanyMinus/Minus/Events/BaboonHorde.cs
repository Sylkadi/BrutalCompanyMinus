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
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(33.0f, 0.66f, 33.0f, 100.0f),
                new Scale(2.0f, 0.04f, 2.0f, 6.0f),
                new Scale(3.0f, 0.06f, 3.0f, 9.0f),
                new Scale(4.0f, 0.08f, 4.0f, 12.0f),
                new Scale(5.0f, 0.1f, 5.0f, 15.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
