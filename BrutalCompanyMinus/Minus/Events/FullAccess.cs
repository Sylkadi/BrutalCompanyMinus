using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using System.Collections;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class FullAccess : MEvent
    {
        public override string Name() => nameof(FullAccess);

        public static FullAccess Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 2;
            Description = "Everything is open!";
            ColorHex = "#008000";
            Type = EventType.Good;

            EventsToRemove = new List<string>() { nameof(FacilityGhost) };
        }


        public override void Execute() => Net.Instance.StartCoroutine(UnlockAll());

        public IEnumerator UnlockAll()
        {
            yield return new WaitForSeconds(11.5f);
            Net.Instance.UnlockAndOpenAllDoorsServerRpc();
        }
    }
}
