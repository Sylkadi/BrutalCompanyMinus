using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class StrongEnemies : MEvent
    {
        public override string Name() => nameof(StrongEnemies);

        public static StrongEnemies Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Description = "Enemies here are a little more tougher than usual.";
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.MinHp, new Scale(1.0f, 0.034f, 3.0f, 1.0f));
            ScaleList.Add(ScaleType.MaxHp, new Scale(2.0f, 0.034f, 4.0f, 1.0f));
        }

        public override void Execute() => Manager.AddEnemyHp(UnityEngine.Random.Range(Get(ScaleType.MinHp), Get(ScaleType.MaxHp) + 1));
    }
}
