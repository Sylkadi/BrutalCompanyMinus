using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Bonus : MEvent
    {
        public override string Name() => nameof(Bonus);

        public override void Initalize()
        {
            Weight = 2;
            Description = "Corporate is feeling good today.";
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.MinCash, new Scale(80.0f, 0.8f));
            ScaleList.Add(ScaleType.MaxCash, new Scale(120.0f, 1.5f));
        }

        public override void Execute()
        {
            Manager.paycut += UnityEngine.Random.Range(Get(ScaleType.MinCash), Get(ScaleType.MaxCash) + 1);
        }
    }
}
