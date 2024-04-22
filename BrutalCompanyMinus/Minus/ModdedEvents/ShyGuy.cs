using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class ShyGuy : MEvent
    {
        public override string Name() => nameof(ShyGuy);

        public static ShyGuy Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Object Class: Euclid", "All personnel, proceed with caution.", "Look at it, I dare you..." };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                "ShyGuyDef",
                new Scale(20.0f, 0.8f, 20.0f, 100.0f),
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(2.0f, 0.04f, 2.0f, 6.0f),
                new Scale(2.0f, 0.04f, 2.0f, 6.0f),
                new Scale(1.0f, 0.02f, 1.0f, 3.0f),
                new Scale(2.0f, 0.04f, 2.0f, 6.0f))
            };
        }

        public override bool AddEventIfOnly() => Compatibility.scopophobiaPresent;

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
