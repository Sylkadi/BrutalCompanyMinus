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
                new Scale(2.0f, 0.08f, 2.0f, 10.0f),
                new Scale(33.0f, 0.66f, 33.0f, 100.0f),
                new Scale(1.0f, 0.0f, 1.0f, 1.0f),
                new Scale(1.0f, 0.0f, 1.0f, 1.0f),
                new Scale(1.0f, 0.02f, 1.0f, 3.0f),
                new Scale(1.0f, 0.02f, 1.0f, 3.0f)), new MonsterEvent(
                Assets.EnemyName.SnareFlea,
                new Scale(20.0f, 0.8f, 20.0f, 100.0f),
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(7.0f, 0.14f, 7.0f, 21.0f),
                new Scale(10.0f, 0.2f, 10.0f, 30.0f),
                new Scale(3.0f, 0.06f, 3.0f, 9.0f),
                new Scale(5.0f, 0.1f, 5.0f, 15.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
