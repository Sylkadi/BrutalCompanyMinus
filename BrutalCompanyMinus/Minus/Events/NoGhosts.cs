﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoGhosts : MEvent
    {
        public override string Name() => nameof(NoGhosts);

        public static NoGhosts Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Description = "No ghosts";
            ColorHex = "#008000";
            Type = EventType.Remove;

            EventsToRemove = new List<string>() { nameof(LittleGirl), nameof(FacilityGhost) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyNameList[Assets.EnemyName.GhostGirl]);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyNameList[Assets.EnemyName.GhostGirl]);
    }
}