using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using System.Collections;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    internal class FullAccess
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(RoundManager), "RefreshEnemiesList")]
        private static void OnRefreshEnemiesList()
        {
            if(Events.FullAccess.Active && NetworkManager.Singleton.IsServer)
            {
                RoundManager.Instance.StartCoroutine(OpenAll());
                Events.FullAccess.Active = false;
            }
        }

        private static IEnumerator OpenAll()
        {
            yield return new WaitForSeconds(8.0f);
            Net.Instance.UnlockAndOpenAllDoorsServerRpc();
        }
    }
}
