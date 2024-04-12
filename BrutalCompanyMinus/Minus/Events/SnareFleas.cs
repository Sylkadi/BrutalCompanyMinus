using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class SnareFleas : MEvent
    {
        public override string Name() => nameof(SnareFleas);

        public static SnareFleas Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Ceiling campers!", "A delicacy", "The finest of creatures", "Look up", "Look down" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.SnareFlea,
                new Scale(25.0f, 0.84f, 25.0f, 75.0f),
                new Scale(5.0f, 0.17f, 5.0f, 15.0f),
                new Scale(2.0f, 0.067f, 2.0f, 6.0f),
                new Scale(3.0f, 0.1f, 3.0f, 9.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
