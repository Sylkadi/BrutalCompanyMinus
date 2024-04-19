using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Worms : MEvent
    {
        public override string Name() => nameof(Worms);

        public static Worms Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Bug Breach Detected", "The ultimate fine dining experience", "Dont make out with those things" };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(SnareFleas) };

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.EarthLeviathan,
                new Scale(5.0f, 0.17f, 5.0f, 15.0f),
                new Scale(30.0f, 1.0f, 30.0f, 90.0f),
                new Scale(1.0f, 0.0f, 1.0f, 1.0f),
                new Scale(1.0f, 0.0f, 1.0f, 1.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f)), new MonsterEvent(
                Assets.EnemyName.SnareFlea,
                new Scale(50.0f, 0.84f, 50.0f, 100.0f),
                new Scale(20.0f, 0.67f, 20.0f, 60.0f),
                new Scale(7.0f, 0.15f, 7.0f, 16.0f),
                new Scale(10.0f, 0.234f, 10.0f, 24.0f),
                new Scale(3.0f, 0.1f, 3.0f, 9.0f),
                new Scale(5.0f, 0.117f, 5.0f, 12.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
