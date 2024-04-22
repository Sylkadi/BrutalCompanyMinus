using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class FragileEnemies : MEvent
    {
        public override string Name() => nameof(FragileEnemies);

        public static FragileEnemies Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Enemies here are a little more fragile than usual.", "Should take 1 less bonk", "A mysterious ailment is making the enemies fragile..." };
            ColorHex = "#008000";
            Type = EventType.Good;

            EventsToRemove = new List<string>() { nameof(StrongEnemies) };

            ScaleList.Add(ScaleType.MinHp, new Scale(-2.0f, -0.04f, -6.0f, -2.0f));
            ScaleList.Add(ScaleType.MaxHp, new Scale(-1.0f, -0.03f, -4.0f, -1.0f));
        }

        public override void Execute() => Manager.AddEnemyHp(UnityEngine.Random.Range(Get(ScaleType.MinHp), Get(ScaleType.MaxHp) + 1));
    }
}
