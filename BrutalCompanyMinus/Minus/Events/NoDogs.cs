using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoDogs : MEvent
    {
        public override string Name() => nameof(NoDogs);

        public static NoDogs Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "No Barking", "You can now party without uninvited guests", "No more doggos" };
            ColorHex = "#008000";
            Type = EventType.Remove;

            EventsToRemove = new List<string>() { nameof(Dogs), nameof(Hell), nameof(Shrimp) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.EyelessDog) || Manager.SpawnExists("ShrimpEnemy") || Manager.SpawnExists("Football");

        public override void Execute()
        {
            Manager.RemoveSpawn(Assets.EnemyName.EyelessDog);
            Manager.RemoveSpawn("ShrimpEnemy");
            Manager.RemoveSpawn("Football");
        }
    }
}
