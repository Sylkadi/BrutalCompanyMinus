using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoButlers : MEvent
    {
        public override string Name() => nameof(NoButlers);

        public static NoButlers Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "No suits", "No bubblewrap", "No popping", "This facility is missing knives.", "This facility is dirty." };
            ColorHex = "#008000";
            Type = EventType.Remove;

            EventsToRemove = new List<string>() { nameof(Butlers), nameof(Hell) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.Butler);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.Butler);
    }
}
