using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class OldBirds : MEvent
    {
        public override string Name() => nameof(OldBirds);

        public static OldBirds Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Who put that thing in there??", "Mentally deranged toddlers", "Does the lighnting kill them?" };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            EventsToSpawnWith = new List<string>() { nameof(Landmines), nameof(OutsideLandmines) };

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.OldBird,
                new Scale(1.0f, 0.84f, 1.0f, 6.0f),
                new Scale(30.0f, 1.0f, 30.0f, 90.0f),
                new Scale(1.0f, 0.017f, 1.0f, 2.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(1.0f, 0.067f, 1.0f, 5.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
