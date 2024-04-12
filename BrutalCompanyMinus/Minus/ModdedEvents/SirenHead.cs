using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class SirenHead : MEvent
    {
        public override string Name() => nameof(SirenHead);

        public static SirenHead Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "God would like to speak with you...", "NINE. EIGHTEEN. ONE. CHILD. SEVENTEEN. REMOVE. VILE." };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                "SirenHead",
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(30.0f, 1.0f, 1.0f, 90.0f),
                new Scale(0.0f, 0.022f, 0.0f, 1.0f),
                new Scale(0.0f, 0.0167f, 0.0f, 2.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f))
            };
        }

        public override bool AddEventIfOnly() => Compatibility.sirenheadPresent;

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
