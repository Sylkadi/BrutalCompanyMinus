using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class GrabbableLandmines : MEvent
    {
        public override string Name() => nameof(GrabbableLandmines);

        public override void Initalize()
        {
            Weight = 3;
            Description = "Some mines have turned into scrap...";
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.EnemyRarity, new Scale(0.50f, 0.0f));
        }

        public override void Execute() => Manager.grabbableLandmines = true;
    }
}
