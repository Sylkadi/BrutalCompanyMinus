using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Football : MEvent
    {
        public override string Name() => nameof(Football);

        public static Football Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Simon says...", "Just do as she says", "Football!" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                "Football",
                new Scale(3.0f, 0.2f, 3.0f, 15.0f),
                new Scale(2.0f, 0.1f, 4.0f, 8.0f),
                new Scale(1.0f, 0.04f, 1.0f, 3.0f),
                new Scale(1.0f, 0.04f, 1.0f, 3.0f),
                new Scale(0.0f, 0.025f, 0.0f, 1.0f),
                new Scale(0.0f, 0.05f, 0.0f, 2.0f))
            };
        }

        public override bool AddEventIfOnly() => Compatibility.lcOfficePresent;

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
