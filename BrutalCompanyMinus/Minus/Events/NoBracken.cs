using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoBracken : MEvent
    {
        public override string Name() => nameof(NoBracken);

        public static NoBracken Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "No stalkers", "No chiropractor's", "No more disappearing." };
            ColorHex = "#008000";
            Type = EventType.Remove;

            EventsToRemove = new List<string>() { nameof(Bracken), nameof(Hell) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.Bracken);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.Bracken);
    }
}
