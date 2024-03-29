﻿using BrutalCompanyMinus;
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
    internal class Warzone : MEvent
    {

        public static bool Active = false;

        public override string Name() => nameof(Warzone);

        public static Warzone Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Description = "Landmines? Turrets? all of it";
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(LeaflessBrownTrees), nameof(LeaflessTrees) };
            EventsToSpawnWith = new List<string>() { nameof(Turrets), nameof(Landmines), nameof(OutsideTurrets), nameof(OutsideLandmines), nameof(Trees) };
        }

        public override void Execute() => Active = true;

        public override void OnShipLeave()
        {
            Active = false;
            Handlers.DDay.DestroyInstance();
        }

        public override void OnGameStart() => Active = false;
    }
}
