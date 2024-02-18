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

namespace BrutalCompanyMinus
{
    [HarmonyPatch]
    internal class Configuration
    {

        public static ConfigFile generalConfig, eventConfig, weatherMultipliersConfig, customAssetsConfig;

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
        public static ConfigEntry<float> goodEventIncrementMultiplier, badEventIncrementMultiplier, chanceForExtraEvent;

        public static ConfigEntry<bool> useWeatherMultipliers, randomizeWeatherMultipliers, enableTerminalText;

        public static ConfigEntry<int> veryGoodWeight, goodWeight, neutralWeight, badWeight, veryBadWeight, removeEnemyWeight;
        public static ConfigEntry<float> weatherRandomRandomMinInclusive, weatherRandomRandomMaxInclusive;

        public static Weather noneMultiplier, dustCloudMultiplier, rainyMultiplier, stormyMultiplier, foggyMultiplier, floodedMultiplier, eclipsedMultiplier;

        public static ConfigEntry<string> UIKey;
        public static ConfigEntry<bool> NormaliseScrapValueDisplay, EnableUI, ShowUILetterBox, ShowExtraProperties, PopUpUI;

        public static ConfigEntry<bool> customScrapWeights, customEnemyWeights, enableAllEnemies, enableAllScrap, enableCustomWeights;
        public static ConfigEntry<int> allEnemiesDefaultWeight, allScrapDefaultWeight;

        public static ConfigEntry<bool> enableQuotaChanges;
        public static ConfigEntry<int> deadLineDaysAmount, startingCredits, startingQuota, baseIncrease, increaseSteepness;

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
        
        public static void Initalize()
        {
            // Event settings
            useCustomWeights = generalConfig.Bind("_Event Settings", "Use custom weights?", false, "'false'= Use eventType weights to set all the weights     'true'= Use custom set weights");
            eventsToSpawn = generalConfig.Bind("_Event Settings", "Event count", 2);
            maxEventsToSpawn = generalConfig.Bind("_Event Settings", "Max event count", 4);
            chanceForExtraEvent = generalConfig.Bind("_Event Settings", "Chance for extra event", 0.5f, "Will roll this chance, if rolled true, then +1 event and roll again until failed or hit cap.");
            showEventsInChat = generalConfig.Bind("_Event Settings", "Will Minus display events in chat?", false);
            goodEventIncrementMultiplier = generalConfig.Bind("_Event Settings", "Global multiplier for increment value on good and veryGood eventTypes.", 1.0f);
            badEventIncrementMultiplier = generalConfig.Bind("_Event Settings", "Global multiplier for increment value on bad and veryBad eventTypes.", 1.0f);

            // eventType weights
            veryGoodWeight = generalConfig.Bind("EventType Weights", "VeryGood event weight", 6);
            goodWeight = generalConfig.Bind("EventType Weights", "Good event weight", 18);
            neutralWeight = generalConfig.Bind("EventType Weights", "Neutral event weight", 15);
            badWeight = generalConfig.Bind("EventType Weights", "Bad event weight", 33);
            veryBadWeight = generalConfig.Bind("EventType Weights", "VeryBad event weight", 13);
            removeEnemyWeight = generalConfig.Bind("EventType Weights", "Remove event weight", 15, "These events remove something");

            // Weather settings
            useWeatherMultipliers = generalConfig.Bind("Weather Settings", "Enable weather multipliers?", true, "'false'= Disable all weather multipliers     'true'= Enable weather multipliers");
            randomizeWeatherMultipliers = generalConfig.Bind("Weather Settings", "Weather multiplier randomness?", false, "'false'= disable     'true'= enable");
            enableTerminalText = generalConfig.Bind("Weather Settings", "Enable terminal text?", true);
            
            // Weather Random settings
            weatherRandomRandomMinInclusive = generalConfig.Bind("Weather Random Multipliers", "Min Inclusive", 0.9f, "Lower bound of random value");
            weatherRandomRandomMaxInclusive = generalConfig.Bind("Weather Random Multipliers", "Max Inclusive", 1.2f, "Upper bound of random value");

            // Level Enemy/Scrap settings
            customScrapWeights = generalConfig.Bind("Custom enemy and scrap weights", "Generate and use scrap weights?", false, "This will generate customizable scrap weights for each level (This can become slow if you have alot of modded scraps)");
            customEnemyWeights = generalConfig.Bind("Custom enemy and scrap weights", "Generate and use enemy weights?", true, "This will generate customizable enemy weights for each level");
            enableAllEnemies = generalConfig.Bind("Custom enemy and scrap weights", "Enable all enemies on all moons", false, "This will enable all insideEnemies to spawn inside.., you need to have generate and use enemy weights enabled.");
            enableAllScrap = generalConfig.Bind("Custom enemy and scrap weights", "Enable all scrap on all moons", false, "This will enable for all scraps to spawn on all moons, you need to have generate and use scrap weights enabled.");
            enableCustomWeights = generalConfig.Bind("Custom enemy and scrap weights", "_Enable?", true);
            allEnemiesDefaultWeight = generalConfig.Bind("Custom enemy and scrap weights", "All enemies on all moons weight", 2, "If there is any enemy with weight 0, it will be set to this weight enabling them to spawn.");
            allEnemiesDefaultWeight = generalConfig.Bind("Custom enemy and scrap weights", "All scrap on all moons weight", 2, "If there is any scrap with weight 0, it will be set to this weight enabling them to spawn.");

            // Custom scrap settings
            nutSlayerLives = customAssetsConfig.Bind("NutSlayer", "Lives", 6, "If hp reaches zero or below, decrement lives and reset hp until 0 lives.");
            nutSlayerHp = customAssetsConfig.Bind("NutSlayer", "Hp", 4);
            nutSlayerMovementSpeed = customAssetsConfig.Bind("NutSlayer", "Speed", 8.0f);
            nutSlayerImmortal = customAssetsConfig.Bind("NutSlayer", "Immortal", false);
            Assets.grabbableTurret.minValue = customAssetsConfig.Bind("Grabbable Landmine", "Min value", 50).Value;
            Assets.grabbableTurret.maxValue = customAssetsConfig.Bind("Grabbable Landmine", "Max value", 75).Value;
            Assets.grabbableLandmine.minValue = customAssetsConfig.Bind("Grabbable Turret", "Min value", 100).Value;
            Assets.grabbableLandmine.maxValue = customAssetsConfig.Bind("Grabbable Turret", "Max value", 150).Value;
            slayerShotgunMinValue = customAssetsConfig.Bind("Slayer Shotgun", "Min value", 200);
            slayerShotgunMaxValue = customAssetsConfig.Bind("Slayer Shotgun", "Max value", 300);

            // Weather multipliers settings
            Weather createWeatherSettings(Weather weather)
            {
                string configHeader = "_(" + weather.weatherType.ToString() + ") Weather multipliers";

                float valueMultiplierSetting = weatherMultipliersConfig.Bind(configHeader, "Value Multiplier", weather.scrapValueMultiplier, "Multiply Scrap value for " + weather.weatherType.ToString()).Value;
                float amountMultiplierSetting = weatherMultipliersConfig.Bind(configHeader, "Amount Multiplier", weather.scrapAmountMultiplier, "Multiply Scrap amount for " + weather.weatherType.ToString()).Value;
                float sizeMultiplerSetting = weatherMultipliersConfig.Bind(configHeader, "Factory Size Multiplier", weather.factorySizeMultiplier, "Multiply Factory size for " + weather.weatherType.ToString()).Value;

                return new Weather(weather.weatherType, valueMultiplierSetting, amountMultiplierSetting, sizeMultiplerSetting);
            }

            noneMultiplier = createWeatherSettings(new Weather(LevelWeatherType.None, 1.00f, 1.00f, 1.00f));
            dustCloudMultiplier = createWeatherSettings(new Weather(LevelWeatherType.DustClouds, 1.10f, 1.05f, 1.00f));
            rainyMultiplier = createWeatherSettings(new Weather(LevelWeatherType.Rainy, 1.10f, 1.05f, 1.00f));
            stormyMultiplier = createWeatherSettings(new Weather(LevelWeatherType.Stormy, 1.4f, 1.2f, 1.00f));
            foggyMultiplier = createWeatherSettings(new Weather(LevelWeatherType.Foggy, 1.2f, 1.10f, 1.00f));
            floodedMultiplier = createWeatherSettings(new Weather(LevelWeatherType.Flooded, 1.3f, 1.15f, 1.00f));
            eclipsedMultiplier = createWeatherSettings(new Weather(LevelWeatherType.Eclipsed, 1.5f, 1.25f, 1.00f));

            // UI Key
            UIKey = generalConfig.Bind("_UI Options", "Toggle UI Key", "K");
            NormaliseScrapValueDisplay = generalConfig.Bind("_UI Options", "Normlize scrap value display number?", true, "In game default value is 0.4, having this set to true will multiply the 'displayed value' by 2.5 so it looks normal.");
            EnableUI = generalConfig.Bind("_UI Options", "Enable UI?", true);
            ShowUILetterBox = generalConfig.Bind("_UI Options", "Display UI Letter Box?", true);
            ShowExtraProperties = generalConfig.Bind("_UI Options", "Display extra properties", true, "Display extra properties on UI such as scrap value and amount multipliers.");
            PopUpUI = generalConfig.Bind("_UI Options", "PopUp UI?", true, "Will the UI popup whenever you start the day?");

            // Event settings

            foreach (MEvent e in EventManager.events)
            {
                eventWeights.Add(eventConfig.Bind(e.Name(), "Custom Weight", e.Weight, "If you want to use custom weights change 'Use custom weights'? setting in '__Event Settings' to true."));
                eventDescriptions.Add(eventConfig.Bind(e.Name(), "Description", e.Description));
                eventColorHexes.Add(eventConfig.Bind(e.Name(), "Color Hex", e.ColorHex));
                eventTypes.Add(eventConfig.Bind(e.Name(), "Event Type", e.Type));
                eventEnables.Add(eventConfig.Bind(e.Name(), "Event Enabled?", e.Enabled, "Setting this to false will stop the event from occuring.")); // Normal event

                // Make scale list
                List<ScaleType> scaleTypes = new List<ScaleType>();
                List<ConfigEntry<float>> baseScales = new List<ConfigEntry<float>>();
                List<ConfigEntry<float>> incrementScales = new List<ConfigEntry<float>>();
                Dictionary<ScaleType, Scale> scales = new Dictionary<ScaleType, Scale>();
                foreach (KeyValuePair<ScaleType, Scale> obj in e.ScaleList)
                {
                    scaleTypes.Add(obj.Key);
                    baseScales.Add(eventConfig.Bind(e.Name(), obj.Key.ToString() + " Base Scale Value", obj.Value.Base, "Starting Value"));
                    incrementScales.Add(eventConfig.Bind(e.Name(), obj.Key.ToString() + " Increment Scale Value", obj.Value.Increment, "Formula: BaseScale + (IncrementScale * DaysPassed)"));
                }
                for (int i = 0; i != baseScales.Count; i++)
                {
                    scales.Add(scaleTypes[i], new Scale(baseScales[i].Value, incrementScales[i].Value));
                }
                eventScales.Add(scales);
            }
        }

        private static bool bindedLevelConfigurations = false;
        internal static void GenerateLevelConfigurations(StartOfRound instance)
        {
            if (bindedLevelConfigurations || !enableCustomWeights.Value) return;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Create scrap list
            Log.LogInfo("Generating Enemy + Scrap rarity config");
            List<string> scrapNameList = new List<string>();
            foreach (KeyValuePair<string, Item> item in Assets.ItemList)
            {
                if (item.Value.isScrap) scrapNameList.Add(item.Key);
            }

            // Multi thread cause this is fucking slow otherwise
            Parallel.ForEach(instance.levels, level =>
            {
                Log.LogInfo(string.Format("Generating and binding Enemy + Scrap rarity config for {0}", level.name));

                // Create configFile for particular moon
                ConfigFile levelConfig = new ConfigFile(string.Format("{0}\\BrutalCompanyMinus\\Levels\\{1}_Weights.cfg", Paths.ConfigPath, level.name), true);

                Dictionary<string, int>
                    insideEnemyList = new Dictionary<string, int>(),
                    outsideEnemyList = new Dictionary<string, int>(),
                    daytimeEnemyList = new Dictionary<string, int>(),
                    scrapList = new Dictionary<string, int>();

                // Assign rarities to lists
                // Enemies
                if(customEnemyWeights.Value)
                {
                    // Add all enemies with rarity 0
                    foreach (KeyValuePair<string, EnemyType> enemy in Assets.EnemyList)
                    {
                        insideEnemyList.Add(enemy.Key, 0);
                        outsideEnemyList.Add(enemy.Key, 0);
                        daytimeEnemyList.Add(enemy.Key, 0);
                    }

                    // Inside enemies
                    foreach (SpawnableEnemyWithRarity enemy in level.Enemies)
                    {
                        if (enemy.enemyType == null)
                        {
                            Log.LogError(string.Format("Null entry on {0} in level.Enemies", level.name));
                            continue; // Skip entry
                        }
                        insideEnemyList[enemy.enemyType.name] = enemy.rarity;
                    }
                    foreach (KeyValuePair<string, int> enemy in insideEnemyList.ToList())
                    {
                        insideEnemyList[enemy.Key] = levelConfig.Bind("_Inside Enemies", enemy.Key, enemy.Value).Value;
                    }

                    // Outside enemies
                    foreach (SpawnableEnemyWithRarity enemy in level.OutsideEnemies)
                    {
                        if (enemy.enemyType == null)
                        {
                            Log.LogError(string.Format("Null entry on {0} in level.OutsideEnemies", level.name));
                            continue; // Skip entry
                        }
                        outsideEnemyList[enemy.enemyType.name] = enemy.rarity;
                    }
                    foreach (KeyValuePair<string, int> enemy in outsideEnemyList.ToList())
                    {
                        outsideEnemyList[enemy.Key] = levelConfig.Bind("_Outside Enemies", enemy.Key, enemy.Value).Value;
                    }

                    // Daytime enemies
                    foreach (SpawnableEnemyWithRarity enemy in level.DaytimeEnemies)
                    {
                        if (enemy.enemyType == null)
                        {
                            Log.LogError(string.Format("Null entry on {0} in level.DaytimeEnemies", level.name));
                            continue; // Skip entry
                        }
                        daytimeEnemyList[enemy.enemyType.name] = enemy.rarity;
                    }
                    foreach (KeyValuePair<string, int> enemy in daytimeEnemyList.ToList())
                    {
                        daytimeEnemyList[enemy.Key] = levelConfig.Bind("Daytime Enemies", enemy.Key, enemy.Value).Value;
                    }
                }

                // Scrap
                if(customScrapWeights.Value)
                {
                    // Add all scrap with rarity 0
                    foreach (string scrapName in scrapNameList)
                    {
                        scrapList.Add(scrapName, 0);
                    }

                    foreach (SpawnableItemWithRarity scrap in level.spawnableScrap)
                    {
                        if (scrap.spawnableItem == null)
                        {
                            Log.LogError(string.Format("Null entry on {0} in level.spawnableScrap", level.name));
                            continue; // Skip Entry
                        }
                        scrapList[scrap.spawnableItem.name] = scrap.rarity;
                    }
                    foreach (KeyValuePair<string, int> scrap in scrapList.ToList())
                    {
                        scrapList[scrap.Key] = levelConfig.Bind("Scrap", scrap.Key, scrap.Value).Value;
                    }
                }

                outsideEnemyRarityList.Add(level.name, outsideEnemyList);
                insideEnemyRarityList.Add(level.name, insideEnemyList);
                daytimeEnemyRarityList.Add(level.name, daytimeEnemyList);
                scrapRarityList.Add(level.name, scrapList);
            });

            stopWatch.Stop();
            Log.LogInfo(string.Format("Took {0}ms", stopWatch.ElapsedMilliseconds));

            bindedLevelConfigurations = true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TimeOfDay), "Awake")]
        private static void OnTimeDayStart(ref TimeOfDay __instance)
        {
            enableQuotaChanges = generalConfig.Bind("Quota Settings", "_Enable Quota Changes", true);
            if(enableQuotaChanges.Value)
            {
                __instance.quotaVariables.deadlineDaysAmount = generalConfig.Bind("Quota Settings", "Deadline Days Amount", __instance.quotaVariables.deadlineDaysAmount).Value;
                __instance.quotaVariables.startingCredits = generalConfig.Bind("Quota Settings", "Starting Credits", __instance.quotaVariables.startingCredits).Value;
                __instance.quotaVariables.startingQuota = generalConfig.Bind("Quota Settings", "Starting Quota", __instance.quotaVariables.startingQuota).Value;
                __instance.quotaVariables.baseIncrease = generalConfig.Bind("Quota Settings", "Base Increase", __instance.quotaVariables.baseIncrease).Value;
                __instance.quotaVariables.increaseSteepness = generalConfig.Bind("Quota Settings", "Increase Steepness", __instance.quotaVariables.increaseSteepness).Value;
            }
        }
    }
}
