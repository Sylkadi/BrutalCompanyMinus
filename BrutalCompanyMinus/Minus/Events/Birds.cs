using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Birds : MEvent
    {
        public override string Name() => nameof(Birds);

        public static Birds Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 8;
            Descriptions = new List<string>() { "Its bird migration season", "Birb", "Do these things have feathers?", "You can shoot these" };
            ColorHex = "#FFFFFF";
            Type = EventType.Neutral;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.Manticoil,
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(5.0f, 0.0f, 5.0f, 5.0f),
                new Scale(8.0f, 0.0f, 8.0f, 8.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
