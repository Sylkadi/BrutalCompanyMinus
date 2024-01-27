using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoLizards : MEvent
    {
        public override string Name() => nameof(NoLizards);

        public override void Initalize()
        {
            Weight = 1;
            Description = "No lizards";
            ColorHex = "#008000";
            Type = EventType.Remove;
            EventsToRemove = new List<string>() { nameof(Lizard) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyNameList[Assets.EnemyName.SporeLizard]);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyNameList[Assets.EnemyName.SporeLizard]);
    }
}
