using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class LittleGirl : MEvent
    {
        public override string Name() => nameof(LittleGirl);

        public static LittleGirl Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "They just want to touch you", "Do you want your head to explode?", "They just want to play with you", "A kingergarten of dead children" };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.GhostGirl,
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(2.0f, 0.067f, 2.0f, 6.0f),
                new Scale(3.0f, 0.1f, 3.0f, 9.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f))
            };
        }
        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
