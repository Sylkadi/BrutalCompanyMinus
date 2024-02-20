using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class BigBonus : MEvent
    {
        public override string Name() => nameof(BigBonus);

        public override void Initalize()
        {
            Weight = 1;
            Description = "Corporate is very pleased";
            ColorHex = "#00FF00";
            Type = EventType.VeryGood;

            EventsToRemove = new List<string>() { nameof(Bonus) };

            ScaleList.Add(ScaleType.MinCash, new Scale(150.0f, 5.84f, 150.0f, 400.0f));
            ScaleList.Add(ScaleType.MaxCash, new Scale(200.0f, 6.7f, 200.0f, 600.0f));
        }

        public override void Execute() => Manager.PayCredits(UnityEngine.Random.Range(Get(ScaleType.MinCash), Get(ScaleType.MaxCash) + 1));
    }
}
