using BrutalCompanyMinus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class LateShip : MEvent
    {
        public override string Name() => nameof(LateShip);

        public static LateShip Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "The ship has arrived a little late.", "Warp drive failed!", "Behind schedule." };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.MinAmount, new Scale(60.0f, 0.917f, 60.0f, 115.0f));
            ScaleList.Add(ScaleType.MaxAmount, new Scale(80.0f, 1.0f, 80.0f, 140.0f));
        }

        public override void Execute() => Manager.AddTime(UnityEngine.Random.Range(Getf(ScaleType.MinAmount), Getf(ScaleType.MaxAmount)));
    }
}
