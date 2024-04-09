using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Masked : MEvent
    {
        public override string Name() => nameof(Masked);

        public static Masked Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Friends!!", "Say hi to your new friends", "Lovely bunch of lads", "You might have trust issues after this", "Who's this new guy???" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.InsideEnemyRarity, new Scale(20.0f, 0.34f, 25.0f, 40.0f));
            ScaleList.Add(ScaleType.MinInsideEnemy, new Scale(1.0f, 0.034f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.MaxInsideEnemy, new Scale(2.0f, 0.05f, 2.0f, 5.0f));
        }

        public override void Execute()
        {
            EnemyType Masked = Assets.GetEnemy(Assets.EnemyName.Masked);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, Masked, Get(ScaleType.InsideEnemyRarity));
            Manager.Spawn.InsideEnemies(Masked, UnityEngine.Random.Range(Get(ScaleType.MinInsideEnemy), Get(ScaleType.MaxInsideEnemy) + 1));
        }
    }
}
