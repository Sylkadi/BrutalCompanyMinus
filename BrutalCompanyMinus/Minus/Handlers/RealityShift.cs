using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

        public static int normalScrapWeight = 85, grabbableLandmineWeight = 15, grabbableTurretWeight = 0;

        public static List<int> ShiftableObjects = new List<int>();

        public static List<NetworkObjectReference> shiftedObjects = new List<NetworkObjectReference>();

        public static MethodInfo grabObjectServerRpc = typeof(PlayerControllerB).GetMethod("GrabObjectServerRpc", BindingFlags.NonPublic | BindingFlags.Instance);
        public static MethodInfo firstEmptyItemSlot = typeof(PlayerControllerB).GetMethod("FirstEmptyItemSlot", BindingFlags.NonPublic| BindingFlags.Instance);
        public static MethodInfo setSpecialGrabAnimationBool = typeof(PlayerControllerB).GetMethod("SetSpecialGrabAnimationBool", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo grabObjectCoroutine = typeof(PlayerControllerB).GetField("grabObjectCoroutine", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo currentlyGrabbingObject = typeof(PlayerControllerB).GetField("currentlyGrabbingObject", BindingFlags.NonPublic | BindingFlags.Instance);

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
            if((int)firstEmptyItemSlot.Invoke(__instance, null) == -1)
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

                        __instance.StopCoroutine(GrabShiftedObject(__instance));
                        __instance.StartCoroutine(GrabShiftedObject(__instance));
                        break;
                    }
                }
            }
        }

        public static IEnumerator GrabShiftedObject(PlayerControllerB instance)
        {
            yield return new WaitUntil(() => shiftedObjects.Count > 0);

            NetworkObject networkObject = null;
            if (!shiftedObjects[0].TryGet(out networkObject))
            {
                Log.LogError("Null network object in GrabShiftedObject()");
                yield break;
            }

            GrabbableObject newObject = networkObject.GetComponent<GrabbableObject>();
            currentlyGrabbingObject.SetValue(instance, newObject);

            newObject.InteractItem();
            if(newObject.grabbable)
            {
                instance.playerBodyAnimator.SetBool("GrabInvalidated", value: false);
                instance.playerBodyAnimator.SetBool("GrabValidated", value: false);
                instance.playerBodyAnimator.SetBool("cancelHolding", value: false);
                instance.playerBodyAnimator.ResetTrigger("Throw");
                setSpecialGrabAnimationBool.Invoke(instance, new object[] { true, newObject } );
                instance.isGrabbingObjectAnimation = true;
                instance.cursorIcon.enabled = false;
                instance.cursorTip.text = "";
                instance.twoHanded = newObject.itemProperties.twoHanded;
                instance.carryWeight += Mathf.Clamp(newObject.itemProperties.weight - 1f, 0f, 10f);
                if (newObject.itemProperties.grabAnimationTime > 0f)
                {
                    instance.grabObjectAnimationTime = newObject.itemProperties.grabAnimationTime;
                }
                else
                {
                    instance.grabObjectAnimationTime = 0.4f;
                }

                if (grabObjectCoroutine.GetValue(instance) != null)
                {
                    instance.StopCoroutine("GrabObject");
                }
                grabObjectCoroutine.SetValue(instance, instance.StartCoroutine("GrabObject"));
            }

            grabObjectServerRpc.Invoke(instance, new object[] { shiftedObjects[0] });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "GrabObjectClientRpc")]
        public static void OnGrabObjectClientRpc()
        {

            
            shiftedObjects.Clear();
        }

        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
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
                shiftListValues.Add((int)(UnityEngine.Random.Range(spawnableItem.minValue, spawnableItem.maxValue + 1) * RoundManager.Instance.scrapValueMultiplier * Manager.scrapValueMultiplier));
            }
        }
    }

    internal class GrabObjectTranspiler 
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject")]
        static IEnumerable<CodeInstruction> OnBeginGrabIL(IEnumerable<CodeInstruction> instructions, ILGenerator il) // deny grab
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

            Log.LogInfo("Patched Section     PlayerControllerB.BeginGrabObject()");
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
