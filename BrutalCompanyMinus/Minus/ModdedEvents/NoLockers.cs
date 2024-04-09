using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoLockers : MEvent
    {
        public override string Name() => nameof(NoLockers);

        public static NoLockers Instance;

        public override void Initalize()
        {
            Instance = this;

            EventsToRemove = new List<string>() { nameof(Hell), nameof(Lockers) };

            Weight = 1;
            Descriptions = new List<string>() { "No iron maidens", "No lockers", "Your chances of surviving has slightly increased." };
            ColorHex = "#008000";
            Type = EventType.Remove;
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists("LockerEnemy") && Compatibility.lockerPresent;

        public override void Execute() => Manager.RemoveSpawn("LockerEnemy");
    }
}
