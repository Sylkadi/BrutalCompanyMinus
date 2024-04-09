using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class SafeOutside : MEvent
    {
        public override string Name() => nameof(SafeOutside);

        public static SafeOutside Instance;

        public static bool Active = false;

        public override void Initalize()
        {
            Instance = this;

            EventsToRemove = new List<string>() { nameof(NoOldBird), nameof(NoDogs), nameof(NoGiants), nameof(NoBaboons), nameof(NoWorm), nameof(NoMasks), nameof(Warzone), nameof(OutsideTurrets), nameof(OutsideLandmines), nameof(Masked), nameof(AllWeather) };

            Weight = 1;
            Descriptions = new List<string>() { "Outside is safe!", "It's unusally quiet outside", "You might find bees outside but that is about it", "You can hear your own footstep's echo as you walk outside." };
            ColorHex = "#00FF00";
            Type = EventType.VeryGood;
        }

        public override bool AddEventIfOnly() => !Compatibility.lethalEscapePresent;

        public override void Execute()
        {
            Active = true;
            Manager.RemoveSpawn(Assets.EnemyName.Masked);
        }

        public override void OnShipLeave() => Active = false;

        public override void OnGameStart() => Active = false;
    }
}
