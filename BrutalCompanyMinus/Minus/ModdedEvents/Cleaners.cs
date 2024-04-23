using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Cleaners : MEvent
    {
        public override string Name() => nameof(Cleaners);

        public static Cleaners Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Pest control", "Smoke machines", "Covid free facility", "Cleaners!" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;


            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                "Cleaner",
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(5.0f, 0.1f, 5.0f, 15.0f),
                new Scale(3.0f, 0.06f, 3.0f, 9.0f),
                new Scale(3.0f, 0.09f, 3.0f, 12.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f))
            };
        }

        public override bool AddEventIfOnly() => Compatibility.moonsweptPresent;

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
