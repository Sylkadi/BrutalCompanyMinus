using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoHoardingBugs : MEvent
    {
        public override string Name() => nameof(NoHoardingBugs);

        public static NoHoardingBugs Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Description = "No critters";
            ColorHex = "#008000";
            Type = EventType.Remove;

            EventsToRemove = new List<string>() { nameof(HoardingBugs), nameof(BugHorde), nameof(KamikazieBugs), nameof(Hell) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyNameList[Assets.EnemyName.HoardingBug]);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyNameList[Assets.EnemyName.HoardingBug]);
    }
}
