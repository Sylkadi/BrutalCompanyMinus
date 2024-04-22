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
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(5.0f, 0.2f, 5.0f, 25.0f),
                new Scale(1.0f, 0.03f, 1.0f, 4.0f),
                new Scale(1.0f, 0.06f, 1.0f, 7.0f),
                new Scale(1.0f, 0.02f, 1.0f, 3.0f),
                new Scale(1.0f, 0.02f, 1.0f, 3.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
