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

namespace BrutalCompanyMinus
{
    internal class Configuration
    {

        public static ConfigFile config;

        public static List<ConfigEntry<int>> eventWeights = new List<ConfigEntry<int>>();
        public static List<ConfigEntry<string>>
            eventDescriptions = new List<ConfigEntry<string>>(),
            eventColorHexes = new List<ConfigEntry<string>>();
        public static List<ConfigEntry<MEvent.EventType>> eventTypes = new List<ConfigEntry<MEvent.EventType>>();
        public static List<Dictionary<ScaleType, Scale>> eventScales = new List<Dictionary<ScaleType, Scale>>();
        public static List<ConfigEntry<bool>> eventEnables = new List<ConfigEntry<bool>>();

        public static ConfigEntry<bool> useCustomWeights, showEventsInChat;
        public static ConfigEntry<int> eventsToSpawn;

        public static ConfigEntry<bool> useWeatherMultipliers, randomizeWeatherMultipliers, enableTerminalText;

        public static ConfigEntry<int> veryGoodWeight, goodWeight, neutralWeight, badWeight, veryBadWeight, removeEnemyWeight;
        public static ConfigEntry<float> weatherRandomRandomMinInclusive, weatherRandomRandomMaxInclusive;

        public static Weather noneMultiplier, dustCloudMultiplier, rainyMultiplier, stormyMultiplier, foggyMultiplier, floodedMultiplier, eclipsedMultiplier;

        public static ConfigEntry<string> UIKey;
        public static ConfigEntry<bool> NormaliseScrapValueDisplay, EnableUI, ShowUILetterBox, ShowExtraProperties, PopUpUI;

        public static void Initalize()
        {
            // Create config file
            config = new ConfigFile(Paths.ConfigPath + "\\BrutalCompanyMinus.cfg", true);

            // Set event weights
            foreach (MEvent e in EventManager.events)
            {
                eventWeights.Add(config.Bind(e.Name(), "Custom Weight", e.Weight, "If you want to use custom weights change 'Use custom weights'? setting in '__Event Settings' to true."));
                eventDescriptions.Add(config.Bind(e.Name(), "Description", e.Description));
                eventColorHexes.Add(config.Bind(e.Name(), "Color Hex", e.ColorHex));
                eventTypes.Add(config.Bind(e.Name(), "Event Type", e.Type));
                eventEnables.Add(config.Bind(e.Name(), "Event Enabled?", e.Enabled, "Setting this to false will stop the event from occuring."));

                // Make scale list
                List<ScaleType> scaleTypes = new List<ScaleType>();
                List<ConfigEntry<float>> baseScales = new List<ConfigEntry<float>>();
                List<ConfigEntry<float>> incrementScales = new List<ConfigEntry<float>>();
                Dictionary<ScaleType, Scale> scales = new Dictionary<ScaleType, Scale>();
                foreach (KeyValuePair<ScaleType, Scale> obj in e.ScaleList)
                {
                    scaleTypes.Add(obj.Key);
                    baseScales.Add(config.Bind(e.Name(), obj.Key.ToString() + " Base Scale Value", obj.Value.Base, "Starting Value"));
                    incrementScales.Add(config.Bind(e.Name(), obj.Key.ToString() + " Increment Scale Value", obj.Value.Increment, "Formula: BaseScale + (IncrementScale * DaysPassed)"));
                }
                for (int i = 0; i != baseScales.Count; i++)
                {
                    scales.Add(scaleTypes[i], new Scale(baseScales[i].Value, incrementScales[i].Value));
                }
                eventScales.Add(scales);
            }

            // Weight settings
            useCustomWeights = config.Bind("__Event Settings", "Use custom weights?", false, "'false'= Use eventType weights to set all the weights     'true'= Use custom set weights");
            eventsToSpawn = config.Bind("__Event Settings", "How many events will spawn per round?", 3);
            showEventsInChat = config.Bind("__Event Settings", "Will Minus display events in chat?", false);

            // eventType weights
            veryGoodWeight = config.Bind("_EventType Weights", "VeryGood event weight", 6);
            goodWeight = config.Bind("_EventType Weights", "Good event weight", 18);
            neutralWeight = config.Bind("_EventType Weights", "Neutral event weight", 15);
            badWeight = config.Bind("_EventType Weights", "Bad event weight", 33);
            veryBadWeight = config.Bind("_EventType Weights", "VeryBad event weight", 13);
            removeEnemyWeight = config.Bind("_EventType Weights", "Remove event weight", 15, "These events remove something");

            // Weather settings
            useWeatherMultipliers = config.Bind("__Weather Settings", "Enable weather multipliers?", true, "'false'= Disable all weather multipliers     'true'= Enable weather multipliers");
            randomizeWeatherMultipliers = config.Bind("__Weather Settings", "Weather multiplier randomness?", false, "'false'= disable     'true'= enable");
            enableTerminalText = config.Bind("__Weather Settings", "Enable terminal text?", true);

            // Weather Random settings
            weatherRandomRandomMinInclusive = config.Bind("_Weather Random Value", "Min Inclusive", 0.9f, "Lower bound of random value");
            weatherRandomRandomMaxInclusive = config.Bind("_Weather Random Value", "Max Inclusive", 1.2f, "Upper bound of random value");

            // Weather multipliers settings
            Weather createWeatherSettings(Weather weather)
            {
                string configHeader = "_(" + weather.weatherType.ToString() + ") Weather multipliers";

                float valueMultiplierSetting = config.Bind(configHeader, "Value Multiplier", weather.scrapValueMultiplier, "Multiply Scrap value for " + weather.weatherType.ToString()).Value;
                float amountMultiplierSetting = config.Bind(configHeader, "Amount Multiplier", weather.scrapAmountMultiplier, "Multiply Scrap amount for " + weather.weatherType.ToString()).Value;
                float sizeMultiplerSetting = config.Bind(configHeader, "Factory Size Multiplier", weather.factorySizeMultiplier, "Multiply Factory size for " + weather.weatherType.ToString()).Value;

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
            UIKey = config.Bind("_UI Options", "Toggle UI Key", "K");
            NormaliseScrapValueDisplay = config.Bind("_UI Options", "Normlize scrap value display number?", true, "In game default value is 0.4, having this set to true will multiply the 'displayed value' by 2.5 so it looks normal.");
            EnableUI = config.Bind("_UI Options", "Enable UI?", true);
            ShowUILetterBox = config.Bind("_UI Options", "Display UI Letter Box?", true);
            ShowExtraProperties = config.Bind("_UI Options", "Display extra properties", true, "Display extra properties on UI such as scrap value and amount multipliers.");
            PopUpUI = config.Bind("_UI Options", "PopUp UI?", true, "Will the UI popup whenever you start the day?");

        }
    }
}
