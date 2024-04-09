using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoImmortalSnails : MEvent
    {
        public override string Name() => nameof(NoImmortalSnails);

        public static NoImmortalSnails Instance;

        public override void Initalize()
        {
            Instance = this;

            EventsToRemove = new List<string>() { nameof(Hell), nameof(RollingGiants) };

            Weight = 1;
            Descriptions = new List<string>() { "No slow moving things", "No immortal snails", "No thermonuclear bombs here..." };
            ColorHex = "#008000";
            Type = EventType.Remove;
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists("ImmortalSnail.EnemyType") && Compatibility.immortalSnailPresent;

        public override void Execute() => Manager.RemoveSpawn("ImmortalSnail.EnemyType");
    }
}
