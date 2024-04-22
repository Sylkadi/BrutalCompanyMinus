using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Lockers : MEvent
    {
        public override string Name() => nameof(Lockers);

        public static Lockers Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Lockers", "They remind me of iron maidens", "The chance of you dying has increased" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                "LockerEnemy",
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(5.0f, 0.1f, 5.0f, 15.0f),
                new Scale(1.0f, 0.04f, 1.0f, 5.0f),
                new Scale(1.0f, 0.06f, 1.0f, 7.0f),
                new Scale(0.0f, 0.0075f, 0.0f, 1.0f),
                new Scale(0.0f, 0.02f, 0.0f, 2.0f))
            };
        }

        public override bool AddEventIfOnly() => Compatibility.lockerPresent;

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
