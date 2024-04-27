using System;
using HarmonyLib;
using BepInEx;
using UnityEngine;
using System.Reflection;
using BrutalCompanyMinus.Minus;
using System.Collections.Generic;
using BrutalCompanyMinus.Minus.Handlers;
using UnityEngine.Diagnostics;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine.InputSystem.HID;
using Discord;
using System.Diagnostics;
using BepInEx.Configuration;
using System.Globalization;
using static BrutalCompanyMinus.Configuration;

namespace BrutalCompanyMinus
{
    [HarmonyPatch]
    [BepInPlugin(GUID, NAME, VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        private const string GUID = "Drinkable.BrutalCompanyMinus";
        private const string NAME = "BrutalCompanyMinus";
        private const string VERSION = "0.13.2";
        private static readonly Harmony harmony = new Harmony(GUID);

        void Awake()
        {
            // Logger
            Log.Initalize(Logger);

            // Required for netweaving
            var EventTypes = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var EventType in EventTypes)
            {
                var methods = EventType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
            
            // Load assets
            Assets.Load();

            // Create config files
            uiConfig = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus\\UI_Settings.cfg", true);
            difficultyConfig = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus\\Difficulty_Settings.cfg", true);
            eventConfig = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus\\VanillaEvents.cfg", true);
            weatherConfig = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus\\Weather_Settings.cfg", true);
            customAssetsConfig = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus\\Enemy_Scrap_Weights_Settings.cfg", true);
            moddedEventConfig = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus\\ModdedEvents.cfg", true);
            customEventConfig = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus\\CustomEvents.cfg", true);
            allEnemiesConfig = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus\\AllEnemies.cfg", true);

            // Patch all
            harmony.PatchAll();
            harmony.PatchAll(typeof(GrabObjectTranspiler));

            Log.LogInfo(NAME + " " + VERSION + " " + "is done patching.");
        }
    }
}
