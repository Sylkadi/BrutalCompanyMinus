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

        public static NoSpiders Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "No 8 legged creatures", "No arachnophobia", "You don't need to bring a hoover inside the facility." };
            ColorHex = "#008000";
            Type = EventType.Remove;

            EventsToRemove = new List<string>() { nameof(Spiders), nameof(Arachnophobia), nameof(Hell), nameof(NutSlayer) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.BunkerSpider);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.BunkerSpider);
    }
}
