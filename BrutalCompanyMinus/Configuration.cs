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

namespace BrutalCompanyMinus
{
    internal class Configuration
    {

        public static ConfigFile generalConfig, eventConfig;

        public static List<ConfigFile> levelConfigs = new List<ConfigFile>();

        public static List<ConfigEntry<int>> eventWeights = new List<ConfigEntry<int>>();
        public static List<ConfigEntry<string>>
            eventDescriptions = new List<ConfigEntry<string>>(),
            eventColorHexes = new List<ConfigEntry<string>>();
        public static List<ConfigEntry<MEvent.EventType>> eventTypes = new List<ConfigEntry<MEvent.EventType>>();
        public static List<Dictionary<ScaleType, Scale>> eventScales = new List<Dictionary<ScaleType, Scale>>();
        public static List<ConfigEntry<bool>> eventEnables = new List<ConfigEntry<bool>>();

        public static ConfigEntry<bool> useCustomWeights, showEventsInChat;
        public static ConfigEntry<int> eventsToSpawn;
        public static ConfigEntry<float> goodEventIncrementMultiplier, badEventIncrementMultiplier;

        public static ConfigEntry<bool> useWeatherMultipliers, randomizeWeatherMultipliers, enableTerminalText;

        public static ConfigEntry<int> veryGoodWeight, goodWeight, neutralWeight, badWeight, veryBadWeight, removeEnemyWeight;
        public static ConfigEntry<float> weatherRandomRandomMinInclusive, weatherRandomRandomMaxInclusive;

        public static Weather noneMultiplier, dustCloudMultiplier, rainyMultiplier, stormyMultiplier, foggyMultiplier, floodedMultiplier, eclipsedMultiplier;

        public static ConfigEntry<string> UIKey;
        public static ConfigEntry<bool> NormaliseScrapValueDisplay, EnableUI, ShowUILetterBox, ShowExtraProperties, PopUpUI;

        public static Dictionary<string, Dictionary<string, int>>  // Level name => Enemy/Scrap name => Rarity
            insideEnemyRarityList = new Dictionary<string, Dictionary<string, int>>(), 
            outsideEnemyRarityList = new Dictionary<string, Dictionary<string, int>>(),
            daytimeEnemyRarityList = new Dictionary<string, Dictionary<string, int>>(),
            scrapRarityList = new Dictionary<string, Dictionary<string, int>>();
        
        public static void Initalize()
        {
            // Create General Config
            generalConfig = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus\\General_Settings.cfg", true);

            // Event settings
            useCustomWeights = generalConfig.Bind("__Event Settings", "Use custom weights?", false, "'false'= Use eventType weights to set all the weights     'true'= Use custom set weights");
            eventsToSpawn = generalConfig.Bind("__Event Settings", "How many events will spawn per round?", 3);
            showEventsInChat = generalConfig.Bind("__Event Settings", "Will Minus display events in chat?", false);
            goodEventIncrementMultiplier = generalConfig.Bind("__Event Settings", "Global multiplier for increment value on good and veryGood eventTypes.", 1.0f);
            badEventIncrementMultiplier = generalConfig.Bind("__Event Settings", "Global multiplier for increment value on bad and veryBad eventTypes.", 1.0f);

            // eventType weights
            veryGoodWeight = generalConfig.Bind("_EventType Weights", "VeryGood event weight", 6);
            goodWeight = generalConfig.Bind("_EventType Weights", "Good event weight", 18);
            neutralWeight = generalConfig.Bind("_EventType Weights", "Neutral event weight", 15);
            badWeight = generalConfig.Bind("_EventType Weights", "Bad event weight", 33);
            veryBadWeight = generalConfig.Bind("_EventType Weights", "VeryBad event weight", 13);
            removeEnemyWeight = generalConfig.Bind("_EventType Weights", "Remove event weight", 15, "These events remove something");

            // Weather settings
            useWeatherMultipliers = generalConfig.Bind("__Weather Settings", "Enable weather multipliers?", true, "'false'= Disable all weather multipliers     'true'= Enable weather multipliers");
            randomizeWeatherMultipliers = generalConfig.Bind("__Weather Settings", "Weather multiplier randomness?", false, "'false'= disable     'true'= enable");
            enableTerminalText = generalConfig.Bind("__Weather Settings", "Enable terminal text?", true);

            // Weather Random settings
            weatherRandomRandomMinInclusive = generalConfig.Bind("_Weather Random Value", "Min Inclusive", 0.9f, "Lower bound of random value");
            weatherRandomRandomMaxInclusive = generalConfig.Bind("_Weather Random Value", "Max Inclusive", 1.2f, "Upper bound of random value");

            // Weather multipliers settings
            Weather createWeatherSettings(Weather weather)
            {
                string configHeader = "_(" + weather.weatherType.ToString() + ") Weather multipliers";

                float valueMultiplierSetting = generalConfig.Bind(configHeader, "Value Multiplier", weather.scrapValueMultiplier, "Multiply Scrap value for " + weather.weatherType.ToString()).Value;
                float amountMultiplierSetting = generalConfig.Bind(configHeader, "Amount Multiplier", weather.scrapAmountMultiplier, "Multiply Scrap amount for " + weather.weatherType.ToString()).Value;
                float sizeMultiplerSetting = generalConfig.Bind(configHeader, "Factory Size Multiplier", weather.factorySizeMultiplier, "Multiply Factory size for " + weather.weatherType.ToString()).Value;

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



            // Create  Event Config
            eventConfig = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus\\Event_Settings.cfg", true);

            foreach (MEvent e in EventManager.events)
            {
                eventWeights.Add(eventConfig.Bind(e.Name(), "Custom Weight", e.Weight, "If you want to use custom weights change 'Use custom weights'? setting in '__Event Settings' to true."));
                eventDescriptions.Add(eventConfig.Bind(e.Name(), "Description", e.Description));
                eventColorHexes.Add(eventConfig.Bind(e.Name(), "Color Hex", e.ColorHex));
                eventTypes.Add(eventConfig.Bind(e.Name(), "Event Type", e.Type));
                eventEnables.Add(eventConfig.Bind(e.Name(), "Event Enabled?", e.Enabled, "Setting this to false will stop the event from occuring."));

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
            if (bindedLevelConfigurations) return;

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
                ConfigFile levelConfig = new ConfigFile(string.Format("{0}\\BrutalCompanyMinus\\Levels\\{1}_Settings.cfg", Paths.ConfigPath, level.name), true);

                Dictionary<string, int>
                    insideEnemyList = new Dictionary<string, int>(),
                    outsideEnemyList = new Dictionary<string, int>(),
                    daytimeEnemyList = new Dictionary<string, int>(),
                    scrapList = new Dictionary<string, int>();

                // Add all enemies with rarity 0
                foreach (KeyValuePair<string, EnemyType> enemy in Assets.EnemyList)
                {
                    insideEnemyList.Add(enemy.Key, 0);
                    outsideEnemyList.Add(enemy.Key, 0);
                    daytimeEnemyList.Add(enemy.Key, 0);
                }

                // Add all scrap with rarity 0
                foreach (string scrapName in scrapNameList)
                {
                    scrapList.Add(scrapName, 0);
                }

                // Assign rarities to lists
                Dictionary<string, int>
                    newInsideEnemyList = new Dictionary<string, int>(), 
                    newOutsideEnemyList = new Dictionary<string, int>(),
                    newDaytimeEnemyList = new Dictionary<string, int>(), 
                    newScrapList = new Dictionary<string, int>();

                // Inside enemies
                foreach (SpawnableEnemyWithRarity enemy in level.Enemies)
                {
                    if (enemy == null || enemy.enemyType == null)
                    {
                        Log.LogError(string.Format("Null entry on {0} in level.Enemies", level.name));
                        continue; // Skip entry
                    }
                    insideEnemyList[enemy.enemyType.name] = enemy.rarity;
                }
                foreach (KeyValuePair<string, int> enemy in insideEnemyList)
                {
                    int rarity = levelConfig.Bind("_Inside Enemies", enemy.Key, enemy.Value).Value;
                    if (rarity == 0) continue; // Dont add to list if rarity is 0
                    newInsideEnemyList.Add(enemy.Key, rarity);
                }
                insideEnemyRarityList.Add(level.name, newInsideEnemyList);

                // Outside enemies
                foreach (SpawnableEnemyWithRarity enemy in level.OutsideEnemies)
                {
                    if (enemy == null || enemy.enemyType == null)
                    {
                        Log.LogError(string.Format("Null entry on {0} in level.OutsideEnemies", level.name));
                        continue; // Skip entry
                    }
                    outsideEnemyList[enemy.enemyType.name] = enemy.rarity;
                }
                foreach (KeyValuePair<string, int> enemy in outsideEnemyList)
                {
                    int rarity = levelConfig.Bind("_Outside Enemies", enemy.Key, enemy.Value).Value;
                    if (rarity == 0) continue; // Dont add to list if rarity is 0
                    newOutsideEnemyList.Add(enemy.Key, rarity);
                }
                outsideEnemyRarityList.Add(level.name, newOutsideEnemyList);

                // Daytime enemies
                foreach (SpawnableEnemyWithRarity enemy in level.DaytimeEnemies)
                {
                    if (enemy == null || enemy.enemyType == null)
                    {
                        Log.LogError(string.Format("Null entry on {0} in level.DaytimeEnemies", level.name));
                        continue; // Skip entry
                    }
                    daytimeEnemyList[enemy.enemyType.name] = enemy.rarity;
                }
                foreach (KeyValuePair<string, int> enemy in daytimeEnemyList)
                {
                    int rarity = levelConfig.Bind("Daytime Enemies", enemy.Key, enemy.Value).Value;
                    if (rarity == 0) continue;// Dont add to list if rarity is 0
                    newDaytimeEnemyList.Add(enemy.Key, rarity);
                }
                daytimeEnemyRarityList.Add(level.name, newDaytimeEnemyList);

                // Scrap
                foreach (SpawnableItemWithRarity scrap in level.spawnableScrap)
                {
                    if (scrap == null || scrap.spawnableItem == null)
                    {
                        Log.LogError(string.Format("Null entry on {0} in level.spawnableScrap", level.name));
                        continue; // Skip Entry
                    }
                    scrapList[scrap.spawnableItem.name] = scrap.rarity;
                }
                foreach (KeyValuePair<string, int> scrap in scrapList)
                {
                    int rarity = levelConfig.Bind("Scrap", scrap.Key, scrap.Value).Value;
                    if (rarity == 0) continue;// Dont add to list if rarity is 0
                    newScrapList.Add(scrap.Key, rarity);
                }
                scrapRarityList.Add(level.name, newScrapList);

                // Add to list
                levelConfigs.Add(levelConfig);
            });
        }

    }
}
