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
    internal class LeaflessBrownTrees : MEvent
    {
        public override string Name() => nameof(LeaflessBrownTrees);

        public override void Initalize()
        {
            Weight = 8;
            Description = "These trees look spooky";
            ColorHex = "#FFFFFF";
            Type = EventType.Neutral;

            EventsToRemove = new List<string>() { nameof(Trees), nameof(LeaflessTrees) };

            ScaleList.Add(ScaleType.MinDensity, new Scale(0.03f, 0.0f, 0.03f, 0.03f));
            ScaleList.Add(ScaleType.MaxDensity, new Scale(0.05f, 0.0f, 0.05f, 0.05f));
        }

        public override void Execute()
        {
            Net.Instance.outsideObjectsToSpawn.Add(new OutsideObjectsToSpawn(UnityEngine.Random.Range(Getf(ScaleType.MinDensity), Getf(ScaleType.MaxDensity)), (int)Assets.ObjectName.TreeLeaflessBrown1));
        }
    }
}
