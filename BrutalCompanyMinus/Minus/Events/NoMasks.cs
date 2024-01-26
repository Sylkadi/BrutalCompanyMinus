using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoMasks : MEvent
    {
        public override string Name() => nameof(NoMasks);

        public override void Initalize()
        {
            Weight = 1;
            Description = "No masks";
            ColorHex = "#008000";
            Type = EventType.Remove;
        }

        public override void Execute()
        {
            // Remove masked scrap
            int index = RoundManager.Instance.currentLevel.spawnableScrap.FindIndex(s => s.spawnableItem.name == Assets.GetItem(Assets.ItemName.Comedy).name);
            if (index != -1) RoundManager.Instance.currentLevel.spawnableScrap.RemoveAt(index);

            index = RoundManager.Instance.currentLevel.spawnableScrap.FindIndex(s => s.spawnableItem.name == Assets.GetItem(Assets.ItemName.Tragedy).name);
            if (index != -1) RoundManager.Instance.currentLevel.spawnableScrap.RemoveAt(index);

            // Remove masked enemy
            Manager.RemoveSpawn("MaskedPlayerEnemy");
        }
    }
}
