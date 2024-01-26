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
    internal class LeaflessTrees : MEvent
    {
        public override string Name() => nameof(LeaflessTrees);

        public override void Initalize()
        {
            Weight = 8;
            Description = "These trees look dead";
            ColorHex = "#FFFFFF";
            Type = EventType.Neutral;

            EventsToRemove = new List<string>() { nameof(Trees), nameof(LeaflessBrownTrees) };

            ScaleList.Add(ScaleType.MinDensity, new Scale(0.03f, 0.0f));
            ScaleList.Add(ScaleType.MaxDensity, new Scale(0.05f, 0.0f));
        }

        public override void Execute()
        {
            Net.Instance.outsideObjectsToSpawn.Add(new OutsideObjectsToSpawn(UnityEngine.Random.Range(Getf(ScaleType.MinDensity) * 0.5f, Getf(ScaleType.MaxDensity) * 0.5f), (int)Assets.ObjectName.TreeLeafless2));
            Net.Instance.outsideObjectsToSpawn.Add(new OutsideObjectsToSpawn(UnityEngine.Random.Range(Getf(ScaleType.MinDensity) * 0.5f, Getf(ScaleType.MaxDensity) * 0.5f), (int)Assets.ObjectName.TreeLeafless3));
        }
    }
}
