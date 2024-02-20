using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class SmallerMap : MEvent
    {
        public override string Name() => nameof(SmallerMap);

        public override void Initalize()
        {
            Weight = 2;
            Description = "This facility is smaller.";
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.FactorySize, new Scale(0.75f, 0.0f, 0.75f, 0.75f));
        }

        public override void Execute() => Manager.currentLevel.factorySizeMultiplier *= Getf(ScaleType.FactorySize);
    }
}
