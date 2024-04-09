using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoThumpers : MEvent
    {
        public override string Name() => nameof(NoThumpers);

        public static NoThumpers Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "No crawlers", "No drifing", "No more running", "No more sharks", "No legless" };
            ColorHex = "#008000";
            Type = EventType.Remove;

            EventsToRemove = new List<string>() { nameof(Thumpers), nameof(Hell), nameof(NutSlayer) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.Thumper);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.Thumper);
    }
}
