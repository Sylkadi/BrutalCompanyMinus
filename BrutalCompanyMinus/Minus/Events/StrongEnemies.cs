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
            Descriptions = new List<string>() { "Enemies here are a little more tougher than usual.", "Should take an extra wack or 2", "These monsters are drugged" };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.MinHp, new Scale(1.0f, 0.03f, 1.0f, 4.0f));
            ScaleList.Add(ScaleType.MaxHp, new Scale(2.0f, 0.04f, 2.0f, 6.0f));
        }

        public override void Execute() => Manager.AddEnemyHp(UnityEngine.Random.Range(Get(ScaleType.MinHp), Get(ScaleType.MaxHp) + 1));
    }
}
