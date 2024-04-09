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

        public static Trees Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 8;
            Descriptions = new List<string>() { "Trees", "More trees", "The trees look like trees" };
            ColorHex = "#FFFFFF";
            Type = EventType.Neutral;

            EventsToRemove = new List<string>() { nameof(LeaflessBrownTrees), nameof(LeaflessTrees) };

            ScaleList.Add(ScaleType.MinDensity, new Scale(0.018f, 0.0f, 0.018f, 0.018f));
            ScaleList.Add(ScaleType.MaxDensity, new Scale(0.03f, 0.0f, 0.03f, 0.03f));
        }

        public override void Execute()
        {
            if (LeaflessTrees.Instance.Executed || LeaflessBrownTrees.Instance.Executed) return;

            Net.Instance.outsideObjectsToSpawn.Add(new OutsideObjectsToSpawn(UnityEngine.Random.Range(Getf(ScaleType.MinDensity), Getf(ScaleType.MaxDensity)), (int)Assets.ObjectName.Tree));
        }
    }
}
