using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoCoilhead : MEvent
    {
        public override string Name() => nameof(NoCoilhead);

        public static NoCoilhead Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Description = "Nothing to stare at today";
            ColorHex = "#008000";
            Type = EventType.Remove;

            EventsToRemove = new List<string>() { nameof(Coilhead), nameof(AntiCoilhead), nameof(Hell) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.CoilHead);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.CoilHead);
    }
}
