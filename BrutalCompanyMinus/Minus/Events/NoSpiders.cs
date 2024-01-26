using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoSpiders : MEvent
    {
        public override string Name() => nameof(NoSpiders);

        public override void Initalize()
        {
            Weight = 1;
            Description = "No 8 legged fucks";
            ColorHex = "#008000";
            Type = EventType.Remove;

            EventsToRemove = new List<string>() { nameof(Spiders), nameof(Arachnophobia) };
        }

        public override void Execute() => Manager.RemoveSpawn("SandSpider");
    }
}
