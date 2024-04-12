using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Slimes : MEvent
    {
        public override string Name() => nameof(Slimes);

        public static Slimes Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "The ground is sticky", "It's very slow unless you wack it", "Don't get lost in the sauce", "It's mostly water and pain", "blob" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.Hygrodere,
                new Scale(20.0f, 0.67f, 25.0f, 60.0f),
                new Scale(8.0f, 0.27f, 8.0f, 24.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(2.0f, 0.067f, 1.0f, 6.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(2.0f, 0.067f, 1.0f, 6.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
