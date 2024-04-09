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

        public static BigBonus Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Corporate is very pleased", "The company has sent you a stimulus check", "Yippeeee" };
            ColorHex = "#00FF00";
            Type = EventType.VeryGood;

            EventsToRemove = new List<string>() { nameof(Bonus) };

            ScaleList.Add(ScaleType.MinCash, new Scale(280.0f, 8.67f, 280.0f, 800.0f));
            ScaleList.Add(ScaleType.MaxCash, new Scale(350.0f, 13.34f, 350.0f, 1150.0f));
        }

        public override void Execute() => Manager.PayCredits(UnityEngine.Random.Range(Get(ScaleType.MinCash), Get(ScaleType.MaxCash) + 1));
    }
}
