using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class GrabbableLandmines : MEvent
    {
        public static bool Active = false;
        public static bool LandmineDisabled = false;
        public override string Name() => nameof(GrabbableLandmines);

        public static GrabbableLandmines Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 3;
            Descriptions = new List<string>() { "Some mines have turned into scrap...", "This was a wonderful idea", "Beep, Beep, Beep.", "You can now sell some of the landmines." };
            ColorHex = "#FF0000";
            Type = EventType.Bad;

            ScaleList.Add(ScaleType.Rarity, new Scale(0.4f, 0.0084f, 0.4f, 0.9f));
        }

        public override bool AddEventIfOnly() => RoundManager.Instance.currentLevel.spawnableMapObjects.ToList().Exists(x => x.prefabToSpawn.name == Assets.ObjectNameList[Assets.ObjectName.Landmine]);

        public override void Execute() {
            Active = true;
            LandmineDisabled = false;
        } 

        public override void OnShipLeave() {
            Active = false;
            LandmineDisabled = true;
        } 

        public override void OnGameStart()
        {
            Active = false;
            LandmineDisabled = false;
        }
    }
}
