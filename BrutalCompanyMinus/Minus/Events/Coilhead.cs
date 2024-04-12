using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Coilhead : MEvent
    {
        public override string Name() => nameof(Coilhead);

        public static Coilhead Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Coilheads detected in the facility!", "Containment breach!", "Dont turn your back on them...", "Did you know that a severed head usually keeps it's consciousness for about 4 to 5 seconds." };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.CoilHead,
                new Scale(30.0f, 1.0f, 30.0f, 90.0f),
                new Scale(4.0f, 0.134f, 4.0f, 12.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(2.0f, 0.05f, 1.0f, 5.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
