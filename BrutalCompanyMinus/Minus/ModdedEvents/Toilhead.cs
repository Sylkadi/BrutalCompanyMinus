using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class ToilHead : MEvent
    {
        public override string Name() => nameof(ToilHead);

        public static ToilHead Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "These things are against the geneva convention.", "Toilheads!", "The coilheads have gotten a software upgrade.", "All the heads!", "You downloaded this mod, not me..." };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(Coilhead), nameof(AntiCoilhead) };

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.CoilHead,
                new Scale(30.0f, 1.0f, 30.0f, 90.0f),
                new Scale(10.0f, 0.34f, 10.0f, 30.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(2.0f, 0.034f, 1.0f, 4.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f)), new MonsterEvent(
                Assets.antiCoilHead,
                new Scale(10.0f, 0.34f, 10.0f, 30.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(0.0f, 0.022f, 0.0f, 1.0f),
                new Scale(1.0f, 0.0167f, 1.0f, 2.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f))
            };
        }

        public override bool AddEventIfOnly() => Compatibility.toilheadPresent;

        public override void Execute()
        {
            if (!Compatibility.toilheadPresent) return;
            Assets.antiCoilHead.enemyName = "Spring";
            ExecuteAllMonsterEvents();
            com.github.zehsteam.ToilHead.Api.forceSpawns = true;
        }
    }
}
