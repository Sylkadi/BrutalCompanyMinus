using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Shrimp : MEvent
    {
        public override string Name() => nameof(Shrimp);

        public static Shrimp Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Shrimp", "Actual doggo", "You have to feed it..." };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                "ShrimpEnemy",
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(2.0f, 0.05f, 4.0f, 10.0f),
                new Scale(1.0f, 0.04f, 1.0f, 5.0f),
                new Scale(1.0f, 0.06f, 1.0f, 7.0f),
                new Scale(0.0f, 0.0075f, 0.0f, 1.0f),
                new Scale(0.0f, 0.02f, 0.0f, 2.0f))
            };
        }

        public override bool AddEventIfOnly() => Compatibility.lcOfficePresent;

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
