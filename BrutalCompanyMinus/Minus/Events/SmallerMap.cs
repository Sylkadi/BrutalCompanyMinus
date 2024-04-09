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

        public static SmallerMap Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 2;
            Descriptions = new List<string>() { "This facility is smaller.", "Less time running around", "This facility is more compact" };
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.FactorySize, new Scale(0.75f, 0.0f, 0.75f, 0.75f));
        }

        public override void Execute() => Manager.currentLevel.factorySizeMultiplier *= Getf(ScaleType.FactorySize);
    }
}
