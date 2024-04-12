using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoBirds : MEvent
    {
        public override string Name() => nameof(NoBirds);

        public static NoBirds Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "No birds", "No birbs", "No flower snakes!" };
            ColorHex = "#008000";
            Type = EventType.Remove;

            EventsToRemove = new List<string>() { nameof(Birds), nameof(Hell), nameof(FlowerSnake) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.Manticoil) || Manager.SpawnExists(Assets.EnemyName.FlowerSnake);

        public override void Execute()
        {
            Manager.RemoveSpawn(Assets.EnemyName.Manticoil);
            Manager.RemoveSpawn(Assets.EnemyName.FlowerSnake);
        }
    }
}
