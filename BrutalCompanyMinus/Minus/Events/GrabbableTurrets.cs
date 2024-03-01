using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class GrabbableTurrets : MEvent
    {
        public static bool Active = false;
        public override string Name() => nameof(GrabbableTurrets);

        public static GrabbableTurrets Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Description = "Some turrets have turned into scrap...";
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.Rarity, new Scale(0.4f, 0.0084f, 0.4f, 0.9f));
        }

        public override void Execute() => Active = true;

        public override void OnShipLeave() => Active = false;

        public override void OnGameStart() => Active = false;
    }
}
