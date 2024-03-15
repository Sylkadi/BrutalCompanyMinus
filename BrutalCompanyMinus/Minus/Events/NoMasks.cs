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
    internal class NoMasks : MEvent
    {
        public static bool removedComedy, removedTragedy;
        public static SpawnableItemWithRarity comedyReference, tragedyRefernece;

        public override string Name() => nameof(NoMasks);

        public static NoMasks Instance;

        public override void Initalize()
        {
            Instance = this;

            EventsToRemove = new List<string>() { nameof(Hell) };

            Weight = 1;
            Description = "No masks";
            ColorHex = "#008000";
            Type = EventType.Remove;
        }

        public override bool AddEventIfOnly()
        {
            if (Manager.SpawnExists(Assets.EnemyNameList[Assets.EnemyName.Masked])) return true;
            if (RoundManager.Instance.currentLevel.spawnableScrap.Exists(x => x.spawnableItem.name == Assets.ItemNameList[Assets.ItemName.Comedy])) return true;
            if (RoundManager.Instance.currentLevel.spawnableScrap.Exists(x => x.spawnableItem.name == Assets.ItemNameList[Assets.ItemName.Tragedy])) return true;
            return false;
        }

        public override void Execute()
        {
            // Remove masked scrap
            int index = RoundManager.Instance.currentLevel.spawnableScrap.FindIndex(s => s.spawnableItem.name == Assets.ItemNameList[Assets.ItemName.Comedy]);
            if (index != -1)
            {
                RoundManager.Instance.currentLevel.spawnableScrap.RemoveAt(index);
                removedComedy = true;
            }

            index = RoundManager.Instance.currentLevel.spawnableScrap.FindIndex(s => s.spawnableItem.name == Assets.ItemNameList[Assets.ItemName.Tragedy]);
            if (index != -1) 
            { 
                RoundManager.Instance.currentLevel.spawnableScrap.RemoveAt(index);
                removedTragedy = true;
            }

            // Remove masked enemy
            Manager.RemoveSpawn(Assets.EnemyNameList[Assets.EnemyName.Masked]);
        }

        public override void OnShipLeave()
        {
            if(removedComedy)
            {
                RoundManager.Instance.currentLevel.spawnableScrap.Add(comedyReference);
            }
            if(removedTragedy)
            {
                RoundManager.Instance.currentLevel.spawnableScrap.Add(tragedyRefernece);
            }

            removedComedy = false; removedTragedy = false;
        }

        public override void OnGameStart()
        {
            removedComedy = false;
            removedTragedy = false;
        }
    }
}
