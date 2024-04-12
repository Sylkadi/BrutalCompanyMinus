using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class FlowerSnake : MEvent
    {
        public override string Name() => nameof(FlowerSnake);

        public static FlowerSnake Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "It helps if you weight a little more", "These might take your head off", "Flower snakes!" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.FlowerSnake,
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(2.0f, 0.084f, 2.0f, 7.0f),
                new Scale(4.0f, 0.117f, 0.0f, 11.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f))
            };

            ScaleList.Add(ScaleType.DaytimeEnemyRarity, new Scale(50.0f, 0.84f, 25.0f, 100.0f));
        }

        public override void Execute()
        {
            ExecuteAllMonsterEvents();
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.DaytimeEnemies, Assets.EnemyName.FlowerSnake, Get(ScaleType.DaytimeEnemyRarity));
        }
    }
}
