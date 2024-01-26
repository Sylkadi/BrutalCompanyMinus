using HarmonyLib;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    internal class DoorGlitch
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RoundManager), "Update")]
        static void OnUpdate()
        {
            if (Manager.DoorGlitchActive)
            {
                TerminalAccessibleObject[] doors = UnityEngine.Object.FindObjectsOfType<TerminalAccessibleObject>();
                if (doors.Length > 0 && UnityEngine.Random.Range(0, (int)(15 / Time.deltaTime)) == 1)
                {
                    foreach (TerminalAccessibleObject door in doors)
                    {
                        if (UnityEngine.Random.Range(0, 3) == 1)
                        {
                            door.SetDoorOpenServerRpc(true);
                        }
                        else
                        {
                            door.SetDoorOpenServerRpc(false);
                        }
                    }
                }
            }
        }
    }
}
