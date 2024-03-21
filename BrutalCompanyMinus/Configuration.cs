using BepInEx;
using BepInEx.Configuration;
using BrutalCompanyMinus.Minus;
using BrutalCompanyMinus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using static BrutalCompanyMinus.Minus.MEvent;
using System.Reflection.Emit;
using static UnityEngine.EventSystems.EventTrigger;
using MonoMod.Utils;
using System.Diagnostics;
using HarmonyLib;
using System.Numerics;
using UnityEngine;
using System.Collections.Concurrent;
using System.Globalization;
using BrutalCompanyMinus.Minus.Events;

namespace BrutalCompanyMinus
{
    [HarmonyPatch]
    internal class Configuration
    {

        public static ConfigFile uiConfig, eventConfig, weatherConfig, customAssetsConfig, difficultyConfig;

        public static List<ConfigFile> levelConfigs = new List<ConfigFile>();

        public static List<ConfigEntry<int>> eventWeights = new List<ConfigEntry<int>>();
        public static List<ConfigEntry<string>>
            eventDescriptions = new List<ConfigEntry<string>>(),
            eventColorHexes = new List<ConfigEntry<string>>();
        public static List<ConfigEntry<MEvent.EventType>> eventTypes = new List<ConfigEntry<MEvent.EventType>>();
        public static List<Dictionary<ScaleType, Scale>> eventScales = new List<Dictionary<ScaleType, Scale>>();
        public static List<ConfigEntry<bool>> eventEnables = new List<ConfigEntry<bool>>();

        public static ConfigEntry<bool> useCustomWeights, showEventsInChat;
        public static ConfigEntry<int> eventsToSpawn, maxEventsToSpawn;
        public static ConfigEntry<float> goodEventIncrementMultiplier, badEventIncrementMultiplier;
        public static float[] weightsForExtraEvents;

        public static ConfigEntry<bool> useWeatherMultipliers, randomizeWeatherMultipliers, enableTerminalText;

        public static Scale[] eventTypeScales = new Scale[6];
        public static ConfigEntry<float> weatherRandomRandomMinInclusive, weatherRandomRandomMaxInclusive;

        public static Weather noneMultiplier, dustCloudMultiplier, rainyMultiplier, stormyMultiplier, foggyMultiplier, floodedMultiplier, eclipsedMultiplier;

        public static ConfigEntry<string> UIKey;
        public static ConfigEntry<bool> NormaliseScrapValueDisplay, EnableUI, ShowUILetterBox, ShowExtraProperties, PopUpUI, DisplayUIAfterShipLeaves;
        public static ConfigEntry<float> UITime;

        public static ConfigEntry<bool> customScrapWeights, customEnemyWeights, enableAllEnemies, enableAllScrap, enableCustomWeights;
        public static ConfigEntry<int> allEnemiesDefaultWeight, allScrapDefaultWeight;

        public static ConfigEntry<bool> enableQuotaChanges;
        public static ConfigEntry<int> deadLineDaysAmount, startingCredits, startingQuota, baseIncrease, increaseSteepness;
        public static Scale spawnChanceMultiplierScaling = new Scale(), insideEnemyMaxPowerCountScaling = new Scale(), outsideEnemyPowerCountScaling = new Scale(), enemyBonusHpScaling = new Scale(), spawnCapMultiplier = new Scale();
        public static ConfigEntry<bool> ignoreScaleCap;

        public static Dictionary<string, Dictionary<string, int>>  // Level name => Enemy/Scrap name => Rarity
            insideEnemyRarityList = new Dictionary<string, Dictionary<string, int>>(), 
            outsideEnemyRarityList = new Dictionary<string, Dictionary<string, int>>(),
            daytimeEnemyRarityList = new Dictionary<string, Dictionary<string, int>>(),
            scrapRarityList = new Dictionary<string, Dictionary<string, int>>();

        // Custom assets
        public static ConfigEntry<int> nutSlayerLives, nutSlayerHp;
        public static ConfigEntry<float> nutSlayerMovementSpeed;
        public static ConfigEntry<bool> nutSlayerImmortal;

        public static ConfigEntry<int>
            slayerShotgunMinValue, slayerShotgunMaxValue;

        public static CultureInfo en = new CultureInfo("en-US"); // This is important, no touchy

        public static void Initalize()
        {
            // Event settings
            useCustomWeights = difficultyConfig.Bind("_Event Settings", "Use custom weights?", false, "'false'= Use eventType weights to set all the weights     'true'= Use custom set weights");
            eventsToSpawn = difficultyConfig.Bind("_Event Settings", "Event count", 2);
            weightsForExtraEvents = ParseValuesFromString(difficultyConfig.Bind("_Event Settings", "Weights for extra events.", "40, 40, 15, 5", "Weights for extra events, can be expanded. (40, 40, 15, 5) is equivalent to (+0, +1, +2, +3) events").Value);
            showEventsInChat = difficultyConfig.Bind("_Event Settings", "Will Minus display events in chat?", false);

            // eventType weights
            eventTypeScales = new Scale[6]
            {
                getScale(difficultyConfig.Bind("_EventType Weights", "VeryBad event weight scale", "10, 0.34, 10, 30", "Format: BaseScale, IncrementScale, MinCap, MaxCap,   Forumla: BaseScale + (IncrementScale * DaysPassed)").Value),
                getScale(difficultyConfig.Bind("_EventType Weights", "Bad event weight scale", "40, -0.34, 20, 40", "Format: BaseScale, IncrementScale, MinCap, MaxCap,   Forumla: BaseScale + (IncrementScale * DaysPassed)").Value),
                getScale(difficultyConfig.Bind("_EventType Weights", "Neutral event weight scale", "15, 0, 15, 15", "Format: BaseScale, IncrementScale, MinCap, MaxCap,   Forumla: BaseScale + (IncrementScale * DaysPassed)").Value),
                getScale(difficultyConfig.Bind("_EventType Weights", "Good event weight scale", "18, 0, 18, 18", "Format: BaseScale, IncrementScale, MinCap, MaxCap,   Forumla: BaseScale + (IncrementScale * DaysPassed)").Value),
                getScale(difficultyConfig.Bind("_EventType Weights", "VeryGood event weight scale", "5, 0, 5, 5", "Format: BaseScale, IncrementScale, MinCap, MaxCap,   Forumla: BaseScale + (IncrementScale * DaysPassed)").Value),
                getScale(difficultyConfig.Bind("_EventType Weights", "Remove event weight scale", "12, 0, 12, 12", "These events remove something   Format: BaseScale, IncrementScale, MinCap, MaxCap,   Forumla: BaseScale + (IncrementScale * DaysPassed)").Value)
            };

            // Difficulty scaling
            ignoreScaleCap = difficultyConfig.Bind("Difficulty Scaling", "Ignore minCap, maxCap", false, "Ignore caps that limit scaling.");
            spawnChanceMultiplierScaling = getScale(difficultyConfig.Bind("Difficulty Scaling", "Spawn chance multiplier scale", "1.0, 0.017, 1.0, 2.0", "This will multiply the spawn chance by this,   Format: BaseScale, IncrementScale, MinCap, MaxCap,   Forumla: BaseScale + (IncrementScale * DaysPassed)").Value);
            spawnCapMultiplier = getScale(difficultyConfig.Bind("Difficulty Scaling", "Spawn cap multipler scale", "1.0, 0.017, 1.0, 2.0", "This will multiply outside and inside power counts by this,   Format: BaseScale, IncrementScale, MinCap, MaxCap,   Forumla: BaseScale + (IncrementScale * DaysPassed)").Value);
            insideEnemyMaxPowerCountScaling = getScale(difficultyConfig.Bind("Difficulty Scaling", "Additional Inside Max Enemy Power Count", "0, 0, 0, 0", "Added max enemy power count for inside enemies.,   Format: BaseScale, IncrementScale, MinCap, MaxCap,   Forumla: BaseScale + (IncrementScale * DaysPassed)").Value);
            outsideEnemyPowerCountScaling = getScale(difficultyConfig.Bind("Difficulty Scaling", "Additional Outside Max Enemy Power Count", "0, 0, 0, 0", "Added max enemy power count for outside enemies.,   Format: BaseScale, IncrementScale, MinCap, MaxCap,   Forumla: BaseScale + (IncrementScale * DaysPassed)").Value);
            enemyBonusHpScaling = getScale(difficultyConfig.Bind("Difficulty Scaling", "Additional hp", "0, 0.084, 0, 5", "Added hp to all enemies,   Format: BaseScale, IncrementScale, MinCap, MaxCap,   Forumla: BaseScale + (IncrementScale * DaysPassed)").Value);
            goodEventIncrementMultiplier = difficultyConfig.Bind("Difficulty Scaling", "Global multiplier for increment value on good and veryGood eventTypes.", 1.0f);
            badEventIncrementMultiplier = difficultyConfig.Bind("Difficulty Scaling", "Global multiplier for increment value on bad and veryBad eventTypes.", 1.0f);

            // Level Enemy/Scrap settings
            customScrapWeights = customAssetsConfig.Bind("_Custom enemy and scrap weights", "Generate and use scrap weights?", false, "This will generate customizable scrap weights for each level (This can become slow if you have alot of modded scraps)");
            customEnemyWeights = customAssetsConfig.Bind("_Custom enemy and scrap weights", "Generate and use enemy weights?", false, "This will generate customizable enemy weights for each level");
            enableAllEnemies = customAssetsConfig.Bind("_Custom enemy and scrap weights", "Enable all enemies on all moons", false, "This will enable all insideEnemies to spawn inside.., you need to have generate and use enemy weights enabled.");
            enableAllScrap = customAssetsConfig.Bind("_Custom enemy and scrap weights", "Enable all scrap on all moons", false, "This will enable for all scraps to spawn on all moons, you need to have generate and use scrap weights enabled.");
            enableCustomWeights = customAssetsConfig.Bind("_Custom enemy and scrap weights", "_Enable?", false);
            allEnemiesDefaultWeight = customAssetsConfig.Bind("_Custom enemy and scrap weights", "All enemies on all moons weight", 2, "If there is any enemy with weight 0, it will be set to this weight enabling them to spawn.");
            allEnemiesDefaultWeight = customAssetsConfig.Bind("_Custom enemy and scrap weights", "All scrap on all moons weight", 2, "If there is any scrap with weight 0, it will be set to this weight enabling them to spawn.");

            // Custom scrap settings
            nutSlayerLives = customAssetsConfig.Bind("NutSlayer", "Lives", 5, "If hp reaches zero or below, decrement lives and reset hp until 0 lives.");
            nutSlayerHp = customAssetsConfig.Bind("NutSlayer", "Hp", 6);
            nutSlayerMovementSpeed = customAssetsConfig.Bind("NutSlayer", "Speed", 8.0f);
            nutSlayerImmortal = customAssetsConfig.Bind("NutSlayer", "Immortal?", true);
            Assets.grabbableTurret.minValue = customAssetsConfig.Bind("Grabbable Landmine", "Min value", 50).Value;
            Assets.grabbableTurret.maxValue = customAssetsConfig.Bind("Grabbable Landmine", "Max value", 75).Value;
            Assets.grabbableLandmine.minValue = customAssetsConfig.Bind("Grabbable Turret", "Min value", 100).Value;
            Assets.grabbableLandmine.maxValue = customAssetsConfig.Bind("Grabbable Turret", "Max value", 150).Value;
            slayerShotgunMinValue = customAssetsConfig.Bind("Slayer Shotgun", "Min value", 200);
            slayerShotgunMaxValue = customAssetsConfig.Bind("Slayer Shotgun", "Max value", 300);

            // Weather settings
            useWeatherMultipliers = weatherConfig.Bind("_Weather Settings", "Enable weather multipliers?", true, "'false'= Disable all weather multipliers     'true'= Enable weather multipliers");
            randomizeWeatherMultipliers = weatherConfig.Bind("_Weather Settings", "Weather multiplier randomness?", false, "'false'= disable     'true'= enable");
            enableTerminalText = weatherConfig.Bind("_Weather Settings", "Enable terminal text?", true);

            // Weather Random settings
            weatherRandomRandomMinInclusive = weatherConfig.Bind("_Weather Random Multipliers", "Min Inclusive", 0.9f, "Lower bound of random value");
            weatherRandomRandomMaxInclusive = weatherConfig.Bind("_Weather Random Multipliers", "Max Inclusive", 1.2f, "Upper bound of random value");

            // Weather multipliers settings
            Weather createWeatherSettings(Weather weather)
            {
                string configHeader = "_(" + weather.weatherType.ToString() + ") Weather multipliers";

                float valueMultiplierSetting = weatherConfig.Bind(configHeader, "Value Multiplier", weather.scrapValueMultiplier, "Multiply Scrap value for " + weather.weatherType.ToString()).Value;
                float amountMultiplierSetting = weatherConfig.Bind(configHeader, "Amount Multiplier", weather.scrapAmountMultiplier, "Multiply Scrap amount for " + weather.weatherType.ToString()).Value;
                float sizeMultiplerSetting = weatherConfig.Bind(configHeader, "Factory Size Multiplier", weather.factorySizeMultiplier, "Multiply Factory size for " + weather.weatherType.ToString()).Value;

                return new Weather(weather.weatherType, valueMultiplierSetting, amountMultiplierSetting, sizeMultiplerSetting);
            }

            noneMultiplier = createWeatherSettings(new Weather(LevelWeatherType.None, 1.00f, 1.00f, 1.00f));
            dustCloudMultiplier = createWeatherSettings(new Weather(LevelWeatherType.DustClouds, 1.10f, 1.05f, 1.00f));
            rainyMultiplier = createWeatherSettings(new Weather(LevelWeatherType.Rainy, 1.10f, 1.05f, 1.00f));
            stormyMultiplier = createWeatherSettings(new Weather(LevelWeatherType.Stormy, 1.4f, 1.2f, 1.00f));
            foggyMultiplier = createWeatherSettings(new Weather(LevelWeatherType.Foggy, 1.2f, 1.10f, 1.00f));
            floodedMultiplier = createWeatherSettings(new Weather(LevelWeatherType.Flooded, 1.3f, 1.15f, 1.00f));
            eclipsedMultiplier = createWeatherSettings(new Weather(LevelWeatherType.Eclipsed, 1.5f, 1.25f, 1.00f));

            // UI Settings
            UIKey = uiConfig.Bind("UI Options", "Toggle UI Key", "K");
            NormaliseScrapValueDisplay = uiConfig.Bind("UI Options", "Normlize scrap value display number?", true, "In game default value is 0.4, having this set to true will multiply the 'displayed value' by 2.5 so it looks normal.");
            EnableUI = uiConfig.Bind("UI Options", "Enable UI?", true);
            ShowUILetterBox = uiConfig.Bind("UI Options", "Display UI Letter Box?", true);
            ShowExtraProperties = uiConfig.Bind("UI Options", "Display extra properties", true, "Display extra properties on UI such as scrap value and amount multipliers.");
            PopUpUI = uiConfig.Bind("UI Options", "PopUp UI?", true, "Will the UI popup whenever you start the day?");
            UITime = uiConfig.Bind("UI Options", "PopUp UI time.", 45.0f, "UI popup time");
            DisplayUIAfterShipLeaves = uiConfig.Bind("UI Options", "Display UI after ship leaves?", false, "Will only display event's after ship has left.");

            // Event settings
            foreach (MEvent e in EventManager.events)
            {
                eventWeights.Add(eventConfig.Bind(e.Name(), "Custom Weight", e.Weight, "If you want to use custom weights change 'Use custom weights'? setting in '__Event Settings' to true."));
                eventDescriptions.Add(eventConfig.Bind(e.Name(), "Description", e.Description));
                eventColorHexes.Add(eventConfig.Bind(e.Name(), "Color Hex", e.ColorHex));
                eventTypes.Add(eventConfig.Bind(e.Name(), "Event Type", e.Type));
                eventEnables.Add(eventConfig.Bind(e.Name(), "Event Enabled?", e.Enabled, "Setting this to false will stop the event from occuring.")); // Normal event

                // Make scale list
                Dictionary<ScaleType, Scale> scales = new Dictionary<ScaleType, Scale>();
                foreach (KeyValuePair<ScaleType, Scale> obj in e.ScaleList)
                {
                    scales.Add(obj.Key, getScale(eventConfig.Bind(e.Name(), obj.Key.ToString(), $"{obj.Value.Base.ToString(en)}, {obj.Value.Increment.ToString(en)}, {obj.Value.MinCap.ToString(en)}, {obj.Value.MaxCap.ToString(en)}", ScaleInfoList[obj.Key] + "   Format: BaseScale, IncrementScale, MinCap, MaxCap,   Forumla: BaseScale + (IncrementScale * DaysPassed)").Value));
                }
                eventScales.Add(scales);
            }

            // Specific event settings
            Minus.Handlers.FacilityGhost.actionTimeCooldown = eventConfig.Bind(nameof(FacilityGhost), "Normal Action Time Interval", 15.0f, "How often does it take for the ghost to make a decision?").Value;
            Minus.Handlers.FacilityGhost.ghostCrazyActionInterval = eventConfig.Bind(nameof(FacilityGhost), "Crazy Action Time Interval", 0.1f, "How often does it take for the ghost to make a decision while going crazy?").Value;
            Minus.Handlers.FacilityGhost.ghostCrazyPeriod = eventConfig.Bind(nameof(FacilityGhost), "Crazy Period", 3.0f, "How long will the ghost go crazy for?").Value;
            Minus.Handlers.FacilityGhost.crazyGhostChance = eventConfig.Bind(nameof(FacilityGhost), "Crazy Chance", 0.1f, "Whenever the ghost makes a decision, what is the chance that it will go crazy?").Value;
            Minus.Handlers.FacilityGhost.DoNothingWeight = eventConfig.Bind(nameof(FacilityGhost), "Do Nothing Weight", 50, "Whenever the ghost makes a decision, what is the weight to do nothing?").Value;
            Minus.Handlers.FacilityGhost.OpenCloseBigDoorsWeight = eventConfig.Bind(nameof(FacilityGhost), "Open and close big doors weight", 20, "Whenever the ghost makes a decision, what is the weight for ghost to open and close big doors?").Value;
            Minus.Handlers.FacilityGhost.MessWithLightsWeight = eventConfig.Bind(nameof(FacilityGhost), "Mess with lights weight", 16, "Whenever the ghost makes a decision, what is the weight to mess with lights?").Value;
            Minus.Handlers.FacilityGhost.MessWithBreakerWeight = eventConfig.Bind(nameof(FacilityGhost), "Mess with breaker weight", 4, "Whenever the ghost makes a decision, what is the weight to mess with the breaker?").Value;
            Minus.Handlers.FacilityGhost.OpenCloseDoorsWeight = eventConfig.Bind(nameof(FacilityGhost), "Open and close normal doors weight", 9, "Whenever the ghost makes a decision, what is the weight to attempt to open and close normal doors.").Value;
            Minus.Handlers.FacilityGhost.lockUnlockDoorsWeight = eventConfig.Bind(nameof(FacilityGhost), "Lock and unlock normal doors weight", 3, "Whenever the ghost makes a decision, what is the weight to attempt to lock and unlock normal doors.").Value;
            Minus.Handlers.FacilityGhost.chanceToOpenCloseDoor = eventConfig.Bind(nameof(FacilityGhost), "Chance to open and close normal doors", 0.3f, "Whenever the ghosts decides to open and close doors, what is the chance for each individual door that it will do that.").Value;
            Minus.Handlers.FacilityGhost.chanceToLockUnlockDoor = eventConfig.Bind(nameof(FacilityGhost), "Chance to lock and unlock normal doors", 0.1f, "Whenever the ghosts decides to lock and unlock doors, what is the chance for each individual door that it will do that.").Value;

            Minus.Handlers.RealityShift.normalScrapWeight = eventConfig.Bind(nameof(RealityShift), "Normal shift weight", 85, "Weight for transforming scrap into some other scrap?").Value;
            Minus.Handlers.RealityShift.grabbableLandmineWeight = eventConfig.Bind(nameof(RealityShift), "Grabbable landmine shift weight", 15, "Weight for transforming scrap into a grabbable landmine?").Value;
            Minus.Handlers.RealityShift.grabbableTurretWeight = eventConfig.Bind(nameof(RealityShift), "Grabbable turret shift weight", 0, "Weight for transforming scrap into a grabbable turret?").Value;

            Minus.Handlers.DDay.bombardmentInterval = eventConfig.Bind(nameof(DDay), "Bombardment interval", 100, "The time it takes before each bombardment event.").Value;
            Minus.Handlers.DDay.bombardmentTime = eventConfig.Bind(nameof(DDay), "Bombardment time", 15, "When a bombardment event occurs, how long will it last?").Value;
            Minus.Handlers.DDay.fireInterval = eventConfig.Bind(nameof(DDay), "Fire interval", 1, "During a bombardment event how often will it fire?").Value;
            Minus.Handlers.DDay.fireAmount = eventConfig.Bind(nameof(DDay), "Fire amount", 8, "For every fire interval, how many shot's will it take? This will get scaled higher on bigger maps.").Value;
            Minus.Handlers.DDay.displayWarning = eventConfig.Bind(nameof(DDay), "Display warning?", true, "Display warning message before bombardment?").Value;
            Minus.Handlers.DDay.volume = eventConfig.Bind(nameof(DDay), "Siren Volume?", 0.3f, "Volume of the siren? between 0.0 and 1.0").Value;
            Minus.Handlers.ArtilleryShell.speed = eventConfig.Bind(nameof(DDay), "Artillery shell speed", 100.0f, "How fast does the artillery shell travel?").Value;
        }

        private static bool bindedLevelConfigurations = false;
        internal static void GenerateLevelConfigurations(StartOfRound instance)
        {
            if (bindedLevelConfigurations || !enableCustomWeights.Value) return;
            if (!customEnemyWeights.Value && !customScrapWeights.Value) return;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Create scrap list
            Log.LogInfo("Generating Enemy + Scrap rarity config");
            List<string> scrapNameList = new List<string>();
            foreach (KeyValuePair<string, Item> item in Assets.ItemList)
            {
                if (item.Value.isScrap) scrapNameList.Add(item.Key);
            }

            int levelCount = instance.levels.Length;

            Dictionary<string, int>[]
                insideEnemyList = new Dictionary<string, int>[levelCount],
                outsideEnemyList = new Dictionary<string, int>[levelCount],
                daytimeEnemyList = new Dictionary<string, int>[levelCount],
                scrapList = new Dictionary<string, int>[levelCount];

            // Multi thread cause this is fucking slow otherwise
            Parallel.For(0, levelCount, i =>
            {
                if (instance.levels[i] == null)
                {
                    Log.LogError("Null entry in levels list");
                    return;
                }
                Log.LogInfo(string.Format("Generating and binding Enemy + Scrap rarity config for {0}", instance.levels[i].name));

                // Create configFile for particular moon
                ConfigFile levelConfig = new ConfigFile(string.Format("{0}\\BrutalCompanyMinus\\Levels\\{1}_Weights.cfg", Paths.ConfigPath, instance.levels[i].name), true);
                
                // Initalize Lists
                insideEnemyList[i] = new Dictionary<string, int>();
                outsideEnemyList[i] = new Dictionary<string, int>();
                daytimeEnemyList[i] = new Dictionary<string, int>();
                scrapList[i] = new Dictionary<string, int>();

                // Assign rarities to lists
                // Enemies
                if (customEnemyWeights.Value)
                {
                    // Add all enemies with rarity 0
                    foreach (KeyValuePair<string, EnemyType> enemy in Assets.EnemyList)
                    {
                        insideEnemyList[i].Add(enemy.Key, 0);
                        outsideEnemyList[i].Add(enemy.Key, 0);
                        daytimeEnemyList[i].Add(enemy.Key, 0);
                    }

                    // Inside enemies
                    foreach (SpawnableEnemyWithRarity enemy in instance.levels[i].Enemies)
                    {
                        if (enemy == null || enemy.enemyType == null)
                        {
                            Log.LogError(string.Format("Null entry on {0} in level.Enemies", instance.levels[i].name));
                            continue; // Skip entry
                        }
                        insideEnemyList[i][enemy.enemyType.name] = enemy.rarity;
                    }
                    foreach (KeyValuePair<string, int> enemy in insideEnemyList[i].ToList())
                    {
                        insideEnemyList[i][enemy.Key] = levelConfig.Bind("_Inside Enemies", enemy.Key, enemy.Value).Value;
                    }

                    // Outside enemies
                    foreach (SpawnableEnemyWithRarity enemy in instance.levels[i].OutsideEnemies)
                    {
                        if (enemy == null || enemy.enemyType == null)
                        {
                            Log.LogError(string.Format("Null entry on {0} in level.OutsideEnemies", instance.levels[i].name));
                            continue; // Skip entry
                        }
                        outsideEnemyList[i][enemy.enemyType.name] = enemy.rarity;
                    }
                    foreach (KeyValuePair<string, int> enemy in outsideEnemyList[i].ToList())
                    {
                        outsideEnemyList[i][enemy.Key] = levelConfig.Bind("_Outside Enemies", enemy.Key, enemy.Value).Value;
                    }

                    // Daytime enemies
                    foreach (SpawnableEnemyWithRarity enemy in instance.levels[i].DaytimeEnemies)
                    {
                        if (enemy == null || enemy.enemyType == null)
                        {
                            Log.LogError(string.Format("Null entry on {0} in level.DaytimeEnemies", instance.levels[i].name));
                            continue; // Skip entry
                        }
                        daytimeEnemyList[i][enemy.enemyType.name] = enemy.rarity;
                    }
                    foreach (KeyValuePair<string, int> enemy in daytimeEnemyList[i].ToList())
                    {
                        daytimeEnemyList[i][enemy.Key] = levelConfig.Bind("Daytime Enemies", enemy.Key, enemy.Value).Value;
                    }
                }

                // Scrap
                if(customScrapWeights.Value)
                {
                    // Add all scrap with rarity 0
                    foreach (string scrapName in scrapNameList)
                    {
                        scrapList[i].Add(scrapName, 0);
                    }

                    foreach (SpawnableItemWithRarity scrap in instance.levels[i].spawnableScrap)
                    {
                        if (scrap == null || scrap.spawnableItem == null)
                        {
                            Log.LogError(string.Format("Null entry on {0} in level.spawnableScrap", instance.levels[i].name));
                            continue; // Skip Entry
                        }
                        scrapList[i][scrap.spawnableItem.name] = scrap.rarity;
                    }
                    foreach (KeyValuePair<string, int> scrap in scrapList[i].ToList())
                    {
                        scrapList[i][scrap.Key] = levelConfig.Bind("Scrap", scrap.Key, scrap.Value).Value;
                    }
                }
            });

            for(int i = 0; i < levelCount; i++) // This is done for thread safety
            {
                string levelName = instance.levels[i].name;
                insideEnemyRarityList.Add(levelName, insideEnemyList[i]);
                outsideEnemyRarityList.Add(levelName, outsideEnemyList[i]);
                daytimeEnemyRarityList.Add(levelName, daytimeEnemyList[i]);
                scrapRarityList.Add(levelName, scrapList[i]);
            }

            stopWatch.Stop();
            Log.LogInfo(string.Format("Took {0}ms", stopWatch.ElapsedMilliseconds));


            
            bindedLevelConfigurations = true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TimeOfDay), "Awake")]
        private static void OnTimeDayStart(ref TimeOfDay __instance)
        {
            enableQuotaChanges = difficultyConfig.Bind("Quota Settings", "_Enable Quota Changes", true);
            if(enableQuotaChanges.Value)
            {
                __instance.quotaVariables.deadlineDaysAmount = difficultyConfig.Bind("Quota Settings", "Deadline Days Amount", __instance.quotaVariables.deadlineDaysAmount).Value;
                __instance.quotaVariables.startingCredits = difficultyConfig.Bind("Quota Settings", "Starting Credits", __instance.quotaVariables.startingCredits).Value;
                __instance.quotaVariables.startingQuota = difficultyConfig.Bind("Quota Settings", "Starting Quota", __instance.quotaVariables.startingQuota).Value;
                __instance.quotaVariables.baseIncrease = difficultyConfig.Bind("Quota Settings", "Base Increase", __instance.quotaVariables.baseIncrease).Value;
                __instance.quotaVariables.increaseSteepness = difficultyConfig.Bind("Quota Settings", "Increase Steepness", __instance.quotaVariables.increaseSteepness).Value;
            }
        }

        private static Scale getScale(string from)
        {
            float[] values = ParseValuesFromString(from);
            return new Scale(values[0], values[1], values[2], values[3]);
        }

        private static float[] ParseValuesFromString(string from)
        {
            return from.Split(',').Select(x => float.Parse(x, en)).ToArray();
        }
    }
}
