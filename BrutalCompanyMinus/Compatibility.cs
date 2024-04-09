using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BepInEx.Bootstrap;
using HarmonyLib;
using UnityEngine;

namespace BrutalCompanyMinus
{
    [HarmonyPatch]
    internal class Compatibility
    {
        internal static bool yippeeModCompatibilityMode = false;
        internal static AudioClip[] yippeeNewSFX = null;

        internal static bool
            lethalEscapePresent = false,
            lethalThingsPresent = false,
            diversityPresent = false,
            scopophobiaPresent = false,
            lcOfficePresent = false,
            herobrinePresent = false,
            peepersPresent = false,
            sirenheadPresent = false,
            rollinggiantPresent = false,
            theFiendPresent = false,
            immortalSnailPresent = false,
            lockerPresent = false,
            theGiantSpecimensPresent = false;

        internal static FieldInfo peeperSpawnChance = null;

        internal static bool IsVersion50 = false;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PreInitSceneScript), "Awake")]
        private static void OnGameLoad()
        {
            foreach(string x in Chainloader.PluginInfos.Keys)
            {
                Log.LogFatal(x);
            }

            Assembly yippeeAssembly = GetAssembly("sunnobunno.YippeeMod");
            if(yippeeAssembly != null)
            {
                Log.LogInfo("Found YippeeMod, Will attempt to replace kamikazie bug SFX");

                Type type = yippeeAssembly.GetType("YippeeMod.YippeeModBase");
                if(type != null)
                {
                    FieldInfo localField = type.GetField("newSFX", BindingFlags.Static | BindingFlags.NonPublic);
                    if(localField != null)
                    {
                        yippeeNewSFX = (AudioClip[])localField.GetValue(null);
                        if(yippeeNewSFX != null)
                        {
                            Log.LogInfo("YippeeMod compatibility succeeded.");
                            yippeeModCompatibilityMode = true;
                        }
                    }
                }
            }

            Assembly peepersAssembly = GetAssembly("x753.Peepers");
            if(peepersAssembly != null)
            {
                Log.LogInfo("Found PeepersMod, Will attempt to get spawnChance field.");

                Type type = peepersAssembly.GetType("LCPeeper.Peeper");
                if(type != null)
                {
                    peeperSpawnChance = type.GetField("PeeperSpawnChance", BindingFlags.Static | BindingFlags.Public);
                    if(peeperSpawnChance != null)
                    {
                        Log.LogInfo("Found spawnChance Field, Peepers and NoPeepers event's will now occur");
                        Minus.Events.NoPeepers.oldSpawnChance = (float)peeperSpawnChance.GetValue(null);
                        peepersPresent = true;
                    }
                }
            }

            lethalEscapePresent = IsModPresent("xCeezy.LethalEscape", "Will prevent SafeOutside event from occuring.");
            lethalThingsPresent = IsModPresent("evaisa.lethalthings", "Roomba and NoRoomba event will now occur.");
            diversityPresent = IsModPresent("Chaos.Diversity", "Walker event will now occur.");
            scopophobiaPresent = IsModPresent("Scopophobia", "Shy Guy and NoShyGuy event will now occur.");
            lcOfficePresent = IsModPresent("Piggy.LCOffice", "Shrimp event will now occur.");
            herobrinePresent = IsModPresent("Kittenji.HerobrineMod", "Herobrine event will now occur.");
            sirenheadPresent = IsModPresent("Ccode.SirenHead", "SirenHead and NoSirenHead event will now occur.");
            rollinggiantPresent = IsModPresent("nomnomab.rollinggiant", "RollingGiant and NoRollingGiant event will now occur.");
            theFiendPresent = IsModPresent("com.RuthlessCompany", "TheFiend and NoFiend event will now occur.");
            immortalSnailPresent = IsModPresent("ImmortalSnail", "ImmortalSnail and NoImmortalSnail event will now occur.");
            lockerPresent = IsModPresent("com.zealsprince.locker", "Locker and NoLocker event will now occur.");
            theGiantSpecimensPresent = IsModPresent("TheGiantSpecimens", "asdasd");
        }

        private static Assembly GetAssembly(string name)
        {
            if(Chainloader.PluginInfos.ContainsKey(name))
            {
                return Chainloader.PluginInfos[name].Instance.GetType().Assembly;
            }
            return null;
        }

        private static bool IsModPresent(string name, string logMessage)
        {
            bool isPresent = Chainloader.PluginInfos.ContainsKey(name);
            if (isPresent) Log.LogInfo($"{name} is present. {logMessage}");
            return isPresent;
        }
    }
}
