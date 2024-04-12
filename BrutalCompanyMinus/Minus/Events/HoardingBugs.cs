using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class HoardingBugs : MEvent
    {
        public override string Name() => nameof(HoardingBugs);

        public static HoardingBugs Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "They look cute.", "Best served bonked", "Pretty innocent" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            EventsToSpawnWith = new List<string>() { nameof(ScarceOutsideScrap) };

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.HoardingBug,
                new Scale(25.0f, 0.417f, 25.0f, 50.0f),
                new Scale(4.0f, 0.134f, 4.0f, 12.0f),
                new Scale(3.0f, 0.067f, 3.0f, 7.0f),
                new Scale(4.0f, 0.084f, 4.0f, 9.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(1.0f, 0.05f, 1.0f, 4.0f)),
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
