using BrutalCompanyMinus.Minus.Handlers;
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

        public static Bonus Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 2;
            Descriptions = new List<string>() { "Corporate is feeling good today.", "The company is giving you credits for existing", "■ ■ ■", "It's never enough." };
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.MinCash, new Scale(75.0f, 2.5f, 75.0f, 225.0f));
            ScaleList.Add(ScaleType.MaxCash, new Scale(125.0f, 4.167f, 125.0f, 375.0f));
        }

        public override void Execute() => Manager.PayCredits(UnityEngine.Random.Range(Get(ScaleType.MinCash), Get(ScaleType.MaxCash) + 1));
    }
}
