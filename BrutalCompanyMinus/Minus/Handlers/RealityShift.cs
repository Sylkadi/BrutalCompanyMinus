using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.HID;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    internal class RealityShift
    {

        public static List<GameObject> shiftList = new List<GameObject>();
        public static List<int> shiftListValues = new List<int>();

        public static int normalScrapWeight = 80, grabbableLandmineWeight = 10, grabbableTurretWeight = 10;

        public static List<int> ShiftableObjects = new List<int>();


        public static bool invalidateGrab = false;
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject")]
        public static void OnBeginGrabObject(ref PlayerControllerB __instance)
        {
            if (!Events.RealityShift.Active || GameNetworkManager.Instance.localPlayerController == null) return;

            Ray interactRay = new Ray(__instance.gameplayCamera.transform.position, __instance.gameplayCamera.transform.forward);
            RaycastHit hit;
            if (!Physics.Raycast(interactRay, out hit, __instance.grabDistance, 832) || hit.collider.gameObject.layer == 8 || !(hit.collider.tag == "PhysicsProp") || __instance.twoHanded || __instance.sinkingValue > 0.73f)
            {
                return;
            }
            GrabbableObject grabbableObject = hit.collider.gameObject.GetComponent<GrabbableObject>();
            if (grabbableObject != null && grabbableObject.NetworkObject != null)
            {
                foreach(int objectID in ShiftableObjects)
                {
                    if(objectID == grabbableObject.gameObject.GetInstanceID())
                    {
                        invalidateGrab = true;
                        Net.Instance.ShiftServerRpc(grabbableObject.NetworkObject);
                        break;
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        [HarmonyPatch(typeof(RoundManager), "waitForScrapToSpawnToSync")]
        public static void OnwaitForScrapToSpawnToSync(ref NetworkObjectReference[] spawnedScrap) 
        {
            if (!Events.RealityShift.Active) return;

            shiftList.Clear();
            shiftListValues.Clear();

            Net.Instance.GenerateShiftableObjectsListServerRpc(spawnedScrap);

            List<int> weights = new List<int>();
            foreach (SpawnableItemWithRarity item in RoundManager.Instance.currentLevel.spawnableScrap)
            {
                if (item != null)
                {
                    weights.Add(item.rarity);
                }
                else
                {
                    weights.Add(0);
                }
            }

            for (int i = 0; i < ShiftableObjects.Count; i++)
            {
                int seed = StartOfRound.Instance.randomMapSeed + i;
                System.Random rng = new System.Random(seed);
                UnityEngine.Random.InitState(seed);
                Item spawnableItem = RoundManager.Instance.currentLevel.spawnableScrap[RoundManager.Instance.GetRandomWeightedIndexList(weights, rng)].spawnableItem;
                int index = RoundManager.Instance.GetRandomWeightedIndex(new int[3] { normalScrapWeight, grabbableLandmineWeight, grabbableTurretWeight }, rng);
                if (spawnableItem.spawnPrefab == null || spawnableItem.spawnPrefab.GetComponent<GrabbableObject>() == null) index = 1;
                if (index == 1) // Grabbable Landmine
                {
                    spawnableItem = Assets.grabbableLandmine;
                }
                else if (index == 2) // Grabbable Turret
                {
                    spawnableItem = Assets.grabbableTurret;
                }

                shiftList.Add(spawnableItem.spawnPrefab);
                shiftListValues.Add((int)(UnityEngine.Random.Range(spawnableItem.minValue, spawnableItem.maxValue + 1) * RoundManager.Instance.scrapValueMultiplier));
            }
        }
    }

    internal class GrabObjectTranspiler 
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject")]
        static IEnumerable<CodeInstruction> OnBeginGrabIL(IEnumerable<CodeInstruction> instructions, ILGenerator il) // return if grab invalidated.
        {
            var code = new List<CodeInstruction>(instructions);

            int index = -1;
            object returnOperand = null;
            

            for (int i = 0; i < code.Count; i++)
            {
                if (index == -1 && code[i].opcode == OpCodes.Ldfld && code[i + 1].opcode == OpCodes.Callvirt && code[i + 2].opcode == OpCodes.Callvirt && code[i + 3].opcode == OpCodes.Newobj && code[i + 4].opcode == OpCodes.Stfld)
                {
                    index = i + 5;
                }

                if (code[i].opcode == OpCodes.Brfalse)
                {
                    returnOperand = code[i].operand;
                    break;
                }
            }

            if(index != -1 && returnOperand != null)
            {
                code.Insert(index, new CodeInstruction(OpCodes.Brtrue, returnOperand));
                code.Insert(index, new CodeInstruction(Transpilers.EmitDelegate<Func<bool>>(() =>
                {
                    if (RealityShift.invalidateGrab)
                    {
                        RealityShift.invalidateGrab = false;
                        return true;
                    }
                    return false;
                })));
            } else
            {
                Log.LogError("Failed to patch BeginGrabObject()");
            }

            Log.LogInfo("Patched Section");
            for(int i = 0; i < index + 4; i++)
            {
                string labels = "", arrow = "";
                foreach (Label l in code[i].labels) labels += code[i].labels;
                if (i == index || i == index + 1) arrow = "-> ";
                Log.LogInfo($"{arrow}{i}: {code[i].opcode}, {code[i].operand}, {labels}");
            }
            
            return code.AsEnumerable();
        }
    }
}
