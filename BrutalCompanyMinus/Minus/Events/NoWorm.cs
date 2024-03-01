using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoWorm : MEvent
    {
        public override string Name() => nameof(NoWorm);

        public static NoWorm Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Description = "No worms";
            ColorHex = "#008000";
            Type = EventType.Remove;
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyNameList[Assets.EnemyName.EarthLeviathan]);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyNameList[Assets.EnemyName.EarthLeviathan]);
    }
}
