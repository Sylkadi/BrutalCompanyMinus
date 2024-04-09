using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoOldBird : MEvent
    {
        public override string Name() => nameof(NoOldBird);

        public static NoOldBird Instance;

        public override void Initalize()
        {
            Instance = this;

            EventsToRemove = new List<string>() { nameof(OldBirds), nameof(Hell) };

            Weight = 1;
            Descriptions = new List<string>() { "No robots", "No deranged children", "No more giant killers" };
            ColorHex = "#008000";
            Type = EventType.Remove;
        }

        public override bool AddEventIfOnly() => Compatibility.IsVersion50 && Manager.SpawnExists(Assets.EnemyName.OldBird);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.OldBird);
    }
}
