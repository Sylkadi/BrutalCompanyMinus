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
                new Scale(25.0f, 0.417f, 25.0f, 50.0f),
                new Scale(4.0f, 0.134f, 4.0f, 12.0f),
                new Scale(5.0f, 0.117f, 5.0f, 12.0f),
                new Scale(6.0f, 0.2f, 6.0f, 18.0f),
                new Scale(7.0f, 0.184f, 7.0f, 18.0f),
                new Scale(10.0f, 0.267f, 10.0f, 26.0f))
            };
        }

        public override bool AddEventIfOnly() => Compatibility.peepersPresent;

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
