using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class ForestGiant : MEvent
    {
        public override string Name() => nameof(ForestGiant);

        public static ForestGiant Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Eddie hall in the facility?", "Why not", "You hearing stomping inside the facility." };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.ForestKeeper,
                new Scale(1.0f, 0.09f, 1.0f, 10.0f),
                new Scale(20.0f, 0.8f, 20.0f, 100.0f),
                new Scale(1.0f, 0.03f, 1.0f, 3.0f),
                new Scale(1.0f, 0.04f, 1.0f, 4.0f),
                new Scale(0.0f, 0.02f, 0.0f, 1.0f),
                new Scale(0.0f, 0.03f, 0.0f, 3.0f)),
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
