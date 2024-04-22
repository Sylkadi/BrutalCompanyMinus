using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.XR.OpenVR;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Bounty : MEvent
    {
        public static bool Active = false;
        public override string Name() => nameof(Bounty);

        public static Bounty Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 2;
            Descriptions = new List<string>() { "The company is now paying for kills", "RIP AND TEAR", "Extermination time", "Monsters roam free, and the price on their heads is mediocre", "The hunt is on!" };
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.MinValue, new Scale(20.0f, 0.4f, 20.0f, 60.0f));
            ScaleList.Add(ScaleType.MaxValue, new Scale(30.0f, 0.6f, 30.0f, 90.0f));
        }

        public override void Execute()
        {
            Handlers.Bounty.enemyObjectIDs.Clear();
            Active = true;
        }

        public override void OnShipLeave() => Active = false;

        public override void OnGameStart() => Active = false;
    }
}
