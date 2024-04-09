using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoSnareFleas : MEvent
    {
        public override string Name() => nameof(NoSnareFleas);

        public static NoSnareFleas Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "No Ceiling campers!", "Your head is safe, I hope...", "No fine dining" };
            ColorHex = "#008000";
            Type = EventType.Remove;

            EventsToRemove = new List<string>() { nameof(SnareFleas), nameof(Worms) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.SnareFlea);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.SnareFlea);
    }
}
