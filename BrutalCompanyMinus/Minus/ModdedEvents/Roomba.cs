using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Roomba : MEvent
    {
        public override string Name() => nameof(Roomba);

        public static Roomba Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Moving Landmines!!", "Facility hoovers", "Weapons of war", "These things are against the Geneva convention" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;


            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                "Boomba",
                new Scale(25.0f, 0.417f, 25.0f, 50.0f),
                new Scale(4.0f, 0.134f, 4.0f, 12.0f),
                new Scale(3.0f, 0.1f, 3.0f, 9.0f),
                new Scale(4.0f, 0.134f, 4.0f, 12.0f),
                new Scale(0.0f, 0.05f, 0.0f, 3.0f),
                new Scale(1.0f, 0.084f, 1.0f, 6.0f))
            };
        }

        public override bool AddEventIfOnly() => Compatibility.lethalThingsPresent;

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
