using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Lizard : MEvent
    {
        public override string Name() => nameof(Lizard);

        public static Lizard Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;

            Descriptions = new List<string>() { "They dont bite... i swear", "Annoying ones...", "MOVE!!!!!!!!", "These will fart on you, And it isn't pleasent." };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.SporeLizard,
                new Scale(25.0f, 0.84f, 25.0f, 75.0f),
                new Scale(4.0f, 0.134f, 4.0f, 12.0f),
                new Scale(1.0f, 0.05f, 1.0f, 4.0f),
                new Scale(2.0f, 0.067f, 2.0f, 6.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
