using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class SlenderMan : MEvent
    {
        public override string Name() => nameof(SlenderMan);

        public static SlenderMan Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Childhood creepypasta", "Dont let it get close...", "You feel paranoid" };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                "SlendermanEnemy",
                new Scale(2.0f, 0.08f, 2.0f, 10.0f),
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(1.0f, 0.04f, 2.0f, 3.0f),
                new Scale(1.0f, 0.04f, 2.0f, 3.0f),
                new Scale(0.0f, 0.0075f, 0.0f, 1.0f),
                new Scale(0.0f, 0.02f, 0.0f, 1.0f))
            };
        }

        public override bool AddEventIfOnly() => Compatibility.facelessStalekerPresent;

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
