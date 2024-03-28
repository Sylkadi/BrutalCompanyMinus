using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class ScarceOutsideScrap : MEvent
    {
        public override string Name() => nameof(ScarceOutsideScrap);

        public static ScarceOutsideScrap Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Description = "There is a scarce amount of scrap outside.";
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.MinItemAmount, new Scale(3.0f, 0.084f, 3.0f, 8.0f));
            ScaleList.Add(ScaleType.MaxItemAmount, new Scale(5.0f, 0.117f, 5.0f, 12.0f));
        }

        public override void Execute() => Manager.Spawn.OutsideScrap(UnityEngine.Random.Range(Get(ScaleType.MinItemAmount), Get(ScaleType.MaxItemAmount) + 1));
    }
}
