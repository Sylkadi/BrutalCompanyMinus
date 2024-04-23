using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class ShockwaveDrones : MEvent
    {
        public override string Name() => nameof(ShockwaveDrones);

        public static ShockwaveDrones Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Shockwave Drones", "Stay away", "Drones" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;


            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                "DroneEnemy",
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(5.0f, 0.1f, 5.0f, 15.0f),
                new Scale(1.0f, 0.04f, 3.0f, 5.0f),
                new Scale(2.0f, 0.04f, 3.0f, 6.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f))
            };
        }

        public override bool AddEventIfOnly() => Compatibility.shockwaveDronesPresent;

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
