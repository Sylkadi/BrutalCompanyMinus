using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class CustomMonsterEvent : MEvent
    {
        public static int CustomMonsterEventCount = 0;

        private string name = "0";
        public ConfigEntry<string> enemyName;

        public override string Name() => name;

        public override void Initalize()
        {
            name = string.Format("Z Temp Custom Monster Event {0}", CustomMonsterEventCount);
            CustomMonsterEventCount++;

            enemyName = Configuration.eventConfig.Bind(Name(), "Enemy Name", "", "To find out what string to use, open save in game, and then in the console it will generate warnings from this mod with enemy names, to get vanilla strings go to line 24 on https://github.com/Sylkadi/BrutalCompanyMinus/blob/master/BrutalCompanyMinus/Asset/Assets.cs ");

            Weight = 0;
            Description = "Description...";
            ColorHex = "#FF0000";
            Type = EventType.Neutral;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(30.0f, 0.5f, 30.0f, 80.0f));
            ScaleList.Add(ScaleType.OutsideEnemyRarity, new Scale(30.0f, 0.5f, 30.0f, 80.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.05f, 1.0f, 5.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(2.0f, 0.1f, 2.0f, 5.0f));
            ScaleList.Add(ScaleType.MinOutsideEnemy, new Scale(1.0f, 0.04f, 1.0f, 2.0f));
            ScaleList.Add(ScaleType.MaxOutsideEnemy, new Scale(2.0f, 0.08f, 2.0f, 5.0f));
        }

        public override void Execute()
        {
            EnemyType enemy = Assets.GetEnemy(enemyName.Value);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, enemy, Get(ScaleType.InsideEnemyRarity));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, enemy, Get(ScaleType.OutsideEnemyRarity));

            Manager.Spawn.OutsideEnemies(enemy, UnityEngine.Random.Range(Get(ScaleType.MinOutsideEnemy), Get(ScaleType.MaxOutsideEnemy) + 1));
            Manager.Spawn.InsideEnemies(enemy, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
