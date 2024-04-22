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
    internal class EarlyShip : MEvent
    {
        public override string Name() => nameof(EarlyShip);

        public static EarlyShip Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "The ship has arrived a little early.", "Warp drive activated!", "The early bird catches the worm." };
            ColorHex = "#008000";
            Type = EventType.Good;

            EventsToRemove = new List<string>() { nameof(LateShip) };

            ScaleList.Add(ScaleType.MinAmount, new Scale(-45.0f, -0.55f, -100.0f, -45.0f));
            ScaleList.Add(ScaleType.MaxAmount, new Scale(-60.0f, -0.55f, -100.0f, -60.0f));
        }

        public override void Execute() => Manager.AddTime(UnityEngine.Random.Range(Getf(ScaleType.MinAmount), Getf(ScaleType.MaxAmount)));
    }
}
