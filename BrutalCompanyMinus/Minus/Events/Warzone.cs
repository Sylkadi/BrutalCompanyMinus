using BrutalCompanyMinus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Warzone : MEvent
    {
        public override string Name() => nameof(Warzone);

        public override void Initalize()
        {
            Weight = 1;
            Description = "Landmines? Turrets? all of it";
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(LeaflessBrownTrees), nameof(LeaflessTrees) };
            EventsToSpawnWith = new List<string>() { nameof(Turrets), nameof(Landmines), nameof(OutsideTurrets), nameof(OutsideLandmines), nameof(Trees) };
        }
    }
}
