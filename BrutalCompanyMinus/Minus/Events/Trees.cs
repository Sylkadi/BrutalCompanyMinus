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
    internal class Trees : MEvent
    {
        public override string Name() => nameof(Trees);

        public override void Initalize()
        {
            Weight = 8;
            Description = "Trees!!";
            ColorHex = "#FFFFFF";
            Type = EventType.Neutral;

            EventsToRemove = new List<string>() { nameof(LeaflessBrownTrees), nameof(LeaflessTrees) };

            ScaleList.Add(ScaleType.MinDensity, new Scale(0.03f, 0.0f));
            ScaleList.Add(ScaleType.MaxDensity, new Scale(0.05f, 0.0f));
        }

        public override void Execute()
        {
            Net.Instance.outsideObjectsToSpawn.Add(new OutsideObjectsToSpawn(UnityEngine.Random.Range(Getf(ScaleType.MinDensity), Getf(ScaleType.MaxDensity)), (int)Assets.ObjectName.Tree));
        }
    }
}
