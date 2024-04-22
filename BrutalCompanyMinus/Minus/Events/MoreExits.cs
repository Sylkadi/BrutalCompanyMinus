using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class MoreExits : MEvent
    {
        public override string Name() => nameof(MoreExits);

        public static MoreExits Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 2;
            Descriptions = new List<string>() { "More entrances!", "More exits!", "Where does this lead?" };
            ColorHex = "#008000";
            Type = EventType.Good;

            ScaleList.Add(ScaleType.MinAmount, new Scale(2.0f, 0.03f, 2.0f, 5.0f));
            ScaleList.Add(ScaleType.MaxAmount, new Scale(3.0f, 0.05f, 3.0f, 8.0f));
        }

        public override void Execute() => MonoBehaviours.Passage.SpawnBunkerPassage(UnityEngine.Random.Range(Get(ScaleType.MinAmount), Get(ScaleType.MaxAmount) + 1));
    }
}
