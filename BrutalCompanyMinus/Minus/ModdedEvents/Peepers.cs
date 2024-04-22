using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Peepers : MEvent
    {
        public override string Name() => nameof(Peepers);

        public static Peepers Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Weights!", "Group HUG!!!", "The air feels heavy...", "More annoying than cute" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                "PeeperType",
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(3.0f, 0.06f, 3.0f, 9.0f),
                new Scale(4.0f, 0.09f, 4.0f, 12.0f),
                new Scale(3.0f, 0.12f, 3.0f, 15.0f),
                new Scale(4.0f, 0.16f, 4.0f, 20.0f))
            };
        }

        public override bool AddEventIfOnly() => Compatibility.peepersPresent;

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
