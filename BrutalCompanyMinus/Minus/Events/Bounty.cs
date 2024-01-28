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
        public override string Name() => nameof(Bounty);

        public override void Initalize()
        {
            Weight = 2;
            Description = "The company is now paying for kills";
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.MinValue, new Scale(20.0f, 0.5f));
            ScaleList.Add(ScaleType.MaxValue, new Scale(40.0f, 0.5f));
        }

        public override void Execute() => Manager.BountyActive = true;
    }
}
