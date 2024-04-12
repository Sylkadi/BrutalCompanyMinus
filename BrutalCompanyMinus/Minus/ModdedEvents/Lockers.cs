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
            Descriptions = new List<string>() { "Lockers", "They look like iron maidens", "The chance of you dying has increased" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                "LockerEnemy",
                new Scale(20.0f, 0.67f, 20.0f, 60.0f),
                new Scale(3.0f, 0.084f, 3.0f, 9.0f),
                new Scale(1.0f, 0.05f, 1.0f, 4.0f),
                new Scale(2.0f, 0.05f, 2.0f, 5.0f),
                new Scale(0.0f, 0.05f, 0.0f, 1.0f),
                new Scale(0.0f, 0.084f, 0.0f, 2.0f))
            };
        }

        public override bool AddEventIfOnly() => Compatibility.lockerPresent;

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
