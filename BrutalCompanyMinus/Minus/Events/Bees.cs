using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Bees : MEvent
    {
        public override string Name() => nameof(Bees);

        public static Bees Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "BEES!!", "BZZZZZ", "Nature's architects are at work.", "Balls" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.CircuitBee,
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(2.0f, 0.034f, 2.0f, 4.0f),
                new Scale(2.0f, 0.067f, 2.0f, 6.0f))
            };

            ScaleList.Add(ScaleType.DaytimeEnemyRarity, new Scale(25.0f, 0.84f, 25.0f, 75.0f));
        }

        public override void Execute()
        {
            ExecuteAllMonsterEvents();
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.DaytimeEnemies, Assets.EnemyName.CircuitBee, Get(ScaleType.DaytimeEnemyRarity));
        }
    }
}
