using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class BugHorde : MEvent
    {
        public override string Name() => nameof(BugHorde);

        public static BugHorde Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Theres too many of them...", "You better be ready for this", "Which ones explode?", "A buzzing doom approaches", "Best with served with yippeeeee mod" };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(HoardingBugs), nameof(KamikazieBugs) };
            EventsToSpawnWith = new List<string> { nameof(ScarceOutsideScrap) };

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.HoardingBug,
                new Scale(33.0f, 0.66f, 33.0f, 100.0f),
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(6.0f, 0.12f, 6.0f, 18.0f),
                new Scale(8.0f, 0.16f, 8.0f, 24.0f),
                new Scale(2.0f, 0.04f, 2.0f, 6.0f),
                new Scale(3.0f, 0.06f, 3.0f, 9.0f)), new MonsterEvent(
                Assets.kamikazieBug,
                new Scale(33.0f, 0.66f, 33.0f, 100.0f),
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(6.0f, 0.12f, 6.0f, 18.0f),
                new Scale(8.0f, 0.16f, 8.0f, 24.0f),
                new Scale(2.0f, 0.04f, 2.0f, 6.0f),
                new Scale(3.0f, 0.06f, 3.0f, 9.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
