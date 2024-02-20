using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(EnemyAI))]
    internal class Bounty : MEvent
    {
        public static bool Active = false;
        public override string Name() => nameof(Bounty);

        public override void Initalize()
        {
            Weight = 2;
            Description = "The company is now paying for kills";
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.MinValue, new Scale(20.0f, 0.34f, 20.0f, 40.0f));
            ScaleList.Add(ScaleType.MaxValue, new Scale(30.0f, 0.5f, 30.0f, 60.0f));
        }

        public override void Execute() => Active = true;

        public override void OnShipLeave() => Active = false;

        public override void OnGameStart() => Active = false;
    }
}
