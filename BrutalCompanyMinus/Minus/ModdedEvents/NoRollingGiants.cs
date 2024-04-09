using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoRollingGiants : MEvent
    {
        public override string Name() => nameof(NoRollingGiants);

        public static NoRollingGiants Instance;

        public override void Initalize()
        {
            Instance = this;

            EventsToRemove = new List<string>() { nameof(Hell), nameof(RollingGiants) };

            Weight = 1;
            Descriptions = new List<string>() { "No rolling giants", "No more cowering in fear", "No weird thing???" };
            ColorHex = "#008000";
            Type = EventType.Remove;
        }

        public override bool AddEventIfOnly() => (Manager.SpawnExists("RollingGiant_EnemyType") || Manager.SpawnExists("RollingGiant_EnemyType_Outside") || Manager.SpawnExists("RollingGiant_EnemyType_Outside_Daytime")) && Compatibility.rollinggiantPresent;

        public override void Execute()
        {
            Manager.RemoveSpawn("RollingGiant_EnemyType");
            Manager.RemoveSpawn("RollingGiant_EnemyType_Outside");
            Manager.RemoveSpawn("RollingGiant_EnemyType_Outside_Daytime");
        }
    }
}
