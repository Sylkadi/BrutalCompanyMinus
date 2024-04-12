using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BepInEx.Bootstrap;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using static BrutalCompanyMinus.Minus.EventManager;
using BrutalCompanyMinus.Minus.Events;
using BrutalCompanyMinus.Minus;

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
            theGiantSpecimensPresent = false,
            mimicsPresent = false,
            footballPresent = false;

        internal static FieldInfo peeperSpawnChance = null;
        internal static NetworkVariable<int>[] mimicNetworkSpawnChances = null;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PreInitSceneScript), "Awake")]
        private static void OnGameLoad()
        {
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
                        NoPeepers.oldSpawnChance = (float)peeperSpawnChance.GetValue(null);
                        peepersPresent = true;
                        moddedEvents.Add(new Peepers());
                        moddedEvents.Add(new NoPeepers());
                    }
                }
            }

            Assembly mimicsAssembly = GetAssembly("x753.Mimics");
            if(mimicsAssembly != null)
            {
                Log.LogInfo("Found mimicsMod, Will attempt to grab spawn rate network variables");

                Type mimicNetworker = mimicsAssembly.GetType("Mimics.MimicNetworker");
                Type mimic = mimicsAssembly.GetType("Mimics.Mimics");
                if (mimicNetworker != null && mimic != null)
                {
                    mimicNetworkSpawnChances = new NetworkVariable<int>[6];

                    for(int i = 0; i < 5; i++)
                    {
                        mimicNetworkSpawnChances[i] = (NetworkVariable<int>)mimicNetworker.GetField("SpawnWeight" + i, BindingFlags.Static | BindingFlags.Public).GetValue(null);
                    }
                    mimicNetworkSpawnChances[5] = (NetworkVariable<int>)mimicNetworker.GetField("SpawnWeightMax", BindingFlags.Static | BindingFlags.Public).GetValue(null);

                    bool isNull = false;
                    foreach(NetworkVariable<int> variable in mimicNetworkSpawnChances)
                    {
                        if(variable == null)
                        {
                            isNull = true;
                            break;
                        }
                    }
                    FieldInfo spawnRates = mimic.GetField("SpawnRates", BindingFlags.Static | BindingFlags.Public);

                    if(spawnRates != null && !isNull)
                    {
                        Log.LogInfo("Found spawn rate network variables, Mimics and noMimics events will now appear.");
                        Minus.Handlers.Mimics.originalSpawnRateValues = (int[])spawnRates.GetValue(null);
                        mimicsPresent = true;
                        moddedEvents.Add(new Mimics());
                        moddedEvents.Add(new NoMimics());
                    }
                }
            }

            lethalEscapePresent = IsModPresent("xCeezy.LethalEscape", "Will prevent SafeOutside event from occuring.");
            lethalThingsPresent = IsModPresent("evaisa.lethalthings", "Roomba and TeleporterTraps event will now occur.", new Roomba(), new TeleporterTraps());
            diversityPresent = IsModPresent("Chaos.Diversity", "Walker event will now occur.", new Walkers());
            scopophobiaPresent = IsModPresent("Scopophobia", "Shy Guy and NoShyGuy event will now occur.", new ShyGuy(), new NoShyGuy());
            lcOfficePresent = IsModPresent("Piggy.LCOffice", "Shrimp event will now occur.", new Shrimp());
            herobrinePresent = IsModPresent("Kittenji.HerobrineMod", "Herobrine event will now occur.", new Herobrine());
            sirenheadPresent = IsModPresent("Ccode.SirenHead", "SirenHead event will now occur.", new SirenHead());
            rollinggiantPresent = IsModPresent("nomnomab.rollinggiant", "RollingGiant and NoRollingGiant event will now occur.", new RollingGiants());
            theFiendPresent = IsModPresent("com.RuthlessCompany", "TheFiend and NoFiend event will now occur.", new TheFiend(), new NoFiend());
            immortalSnailPresent = IsModPresent("ImmortalSnail", "ImmortalSnail and NoImmortalSnail event will now occur.", new ImmortalSnail(), new NoImmortalSnails());
            lockerPresent = IsModPresent("com.zealsprince.locker", "Locker and NoLocker event will now occur.", new Lockers(), new NoLockers());
            theGiantSpecimensPresent = IsModPresent("TheGiantSpecimens", "GiantShowdown event will now occur.", new GiantShowdown());
            footballPresent = IsModPresent("Kittenji.FootballEntity", "Football event will now occur.", new Football());
        }

        private static Assembly GetAssembly(string name)
        {
            if(Chainloader.PluginInfos.ContainsKey(name))
            {
                return Chainloader.PluginInfos[name].Instance.GetType().Assembly;
            }
            return null;
        }

        private static bool IsModPresent(string name, string logMessage, params MEvent[] associatedEvents)
        {
            bool isPresent = Chainloader.PluginInfos.ContainsKey(name);
            if (isPresent)
            {
                moddedEvents.AddRange(associatedEvents);
                Log.LogInfo($"{name} is present. {logMessage}");
            }
            return isPresent;
        }
    }
}
