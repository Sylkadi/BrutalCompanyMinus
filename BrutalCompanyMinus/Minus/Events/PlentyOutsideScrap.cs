using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class PlentyOutsideScrap : MEvent
    {
        public override string Name() => nameof(PlentyOutsideScrap);

        public static PlentyOutsideScrap Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "There is some scrap to be found outside.", "This facility lacks proper waste disposal", "Yay, scrap outside" };
            ColorHex = "#00FF00";
            Type = EventType.VeryGood;

            EventsToRemove = new List<string>() { nameof(ScarceOutsideScrap) };

            ScaleList.Add(ScaleType.MinItemAmount, new Scale(9.0f, 0.09f, 9.0f, 18.0f));
            ScaleList.Add(ScaleType.MaxItemAmount, new Scale(12.0f, 0.12f, 12.0f, 24.0f));
        }

        public override void Execute() => Manager.Spawn.OutsideScrap(UnityEngine.Random.Range(Get(ScaleType.MinItemAmount), Get(ScaleType.MaxItemAmount) + 1));
    }
}
