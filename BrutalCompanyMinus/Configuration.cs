using BepInEx;
using BepInEx.Configuration;
using BrutalCompanyMinus.Minus;
using System.Collections.Generic;
using static BrutalCompanyMinus.Minus.MEvent;
using HarmonyLib;
using UnityEngine;
using System.Globalization;
using BrutalCompanyMinus.Minus.Events;
using static BrutalCompanyMinus.Minus.MonoBehaviours.EnemySpawnCycle;
using static BrutalCompanyMinus.Assets;
using static BrutalCompanyMinus.Helper;
using BrutalCompanyMinus.Minus.MonoBehaviours;
using UnityEngine.Experimental.AI;

namespace BrutalCompanyMinus
{
    [HarmonyPatch]
    internal class Configuration
    {
        // Config files
        public static ConfigFile uiConfig, eventConfig, weatherConfig, customAssetsConfig, difficultyConfig, moddedEventConfig, customEventConfig, allEnemiesConfig;

        // Event settings
        public static List<ConfigEntry<int>> eventWeights = new List<ConfigEntry<int>>();
        public static List<List<string>> eventDescriptions = new List<List<string>>();
        public static List<ConfigEntry<string>> eventColorHexes = new List<ConfigEntry<string>>();
        public static List<ConfigEntry<MEvent.EventType>> eventTypes = new List<ConfigEntry<MEvent.EventType>>();
        public static List<Dictionary<ScaleType, Scale>> eventScales = new List<Dictionary<ScaleType, Scale>>();
        public static List<ConfigEntry<bool>> eventEnables = new List<ConfigEntry<bool>>();
        public static List<List<string>> eventsToRemove = new List<List<string>>(), eventsToSpawnWith = new List<List<string>>();
        public static List<List<MonsterEvent>> monsterEvents = new List<List<MonsterEvent>>();
        public static List<ScrapTransmutationEvent> transmutationEvents = new List<ScrapTransmutationEvent>();

        // Difficulty Settings
        public static ConfigEntry<bool> useCustomWeights, showEventsInChat;
        public static Scale eventsToSpawn;
        public static ConfigEntry<float> goodEventIncrementMultiplier, badEventIncrementMultiplier;
        public static float[] weightsForExtraEvents;
        public static Scale[] eventTypeScales = new Scale[6];
        public static ConfigEntry<bool> enableQuotaChanges;
        public static ConfigEntry<int> deadLineDaysAmount, startingCredits, startingQuota, baseIncrease, increaseSteepness;
        public static Scale
            spawnChanceMultiplierScaling = new Scale(), insideEnemyMaxPowerCountScaling = new Scale(), outsideEnemyPowerCountScaling = new Scale(), enemyBonusHpScaling = new Scale(), spawnCapMultiplier = new Scale(),
            scrapAmountMultiplier = new Scale(), scrapValueMultiplier = new Scale(), insideSpawnChanceAdditive = new Scale(), outsideSpawnChanceAdditive = new Scale();
        public static ConfigEntry<bool> ignoreMaxCap;
        public static ConfigEntry<float> difficultyMaxCap;
        public static ConfigEntry<bool> scaleByDaysPassed, scaleByScrapInShip, scaleByMoonGrade;
        public static ConfigEntry<float> daysPassedDifficultyMultiplier, daysPassedDifficultyCap, scrapInShipDifficultyMultiplier, scrapInShipDifficultyCap;
        public static Dictionary<string, float> gradeAdditives = new Dictionary<string, float>();

        // Weather settings
        public static ConfigEntry<bool> useWeatherMultipliers, randomizeWeatherMultipliers, enableTerminalText;
        public static ConfigEntry<float> weatherRandomRandomMinInclusive, weatherRandomRandomMaxInclusive;
        public static Weather noneMultiplier, dustCloudMultiplier, rainyMultiplier, stormyMultiplier, foggyMultiplier, floodedMultiplier, eclipsedMultiplier;

        // UI settings
        public static ConfigEntry<string> UIKey;
        public static ConfigEntry<bool> NormaliseScrapValueDisplay, EnableUI, ShowUILetterBox, ShowExtraProperties, PopUpUI, DisplayUIAfterShipLeaves;
        public static ConfigEntry<float> UITime, scrollSpeed;

        // Custom assets settings
        public static ConfigEntry<int> nutSlayerLives, nutSlayerHp;
        public static ConfigEntry<float> nutSlayerMovementSpeed;
        public static ConfigEntry<bool> nutSlayerImmortal;
        public static ConfigEntry<int> slayerShotgunMinValue, slayerShotgunMaxValue;

        // All enemies settings
        public static ConfigEntry<bool> enableAllEnemies, enableAllAllEnemies;

        // Other
        public static CultureInfo en = new CultureInfo("en-US"); // This is important, no touchy
        public static string scaleDescription = "Format: BaseScale, IncrementScale, MinCap, MaxCap,   Forumla: BaseScale + (IncrementScale * Difficulty),   By default difficulty goes between 0 to 100 depending on certain factors";

        private static void Initalize()
        {
            // Difficulty Settings
            useCustomWeights = difficultyConfig.Bind("_Event Settings", "Use custom weights?", false, "'false'= Use eventType weights to set all the weights     'true'= Use custom set weights");
            eventsToSpawn = getScale(difficultyConfig.Bind("_Event Settings", "Event amount scale", "2, 0.0417, 2.0, 4.0", "The base amount of events   Format: BaseScale, IncrementScale, MinCap, MaxCap,   Forumla: BaseScale + (IncrementScale * DaysPassed)").Value);
            weightsForExtraEvents = ParseValuesFromString(difficultyConfig.Bind("_Event Settings", "Weights for extra events.", "40, 40, 15, 5", "Weights for extra events, can be expanded. (40, 40, 15, 5) is equivalent to (+0, +1, +2, +3) events").Value);
            showEventsInChat = difficultyConfig.Bind("_Event Settings", "Will Minus display events in chat?", false);

            eventTypeScales = new Scale[6]
            {
                getScale(difficultyConfig.Bind("_EventType Weights", "VeryBad event rarity scale", "0, 0.4, 0, 40", scaleDescription).Value),
                getScale(difficultyConfig.Bind("_EventType Weights", "Bad event rarity scale", "53, -0.4, 13, 53", scaleDescription).Value),
                getScale(difficultyConfig.Bind("_EventType Weights", "Neutral event rarity scale", "15, -0.167, 10, 15", scaleDescription).Value),
                getScale(difficultyConfig.Bind("_EventType Weights", "Good event rarity scale", "20, -0.167, 15, 20", scaleDescription).Value),
                getScale(difficultyConfig.Bind("_EventType Weights", "VeryGood event rarity scale", "2, 0.1, 2, 12", scaleDescription).Value),
                getScale(difficultyConfig.Bind("_EventType Weights", "Remove event rarity scale", "15, -0.167, 10, 15", "These events remove something   " + scaleDescription).Value)
            };

            ignoreMaxCap = difficultyConfig.Bind("Difficuly Scaling", "Ignore max cap from all scales?", true, "Will ignore max cap if true, upperlimit is dictated by difficulty max cap setting as well.");
            difficultyMaxCap = difficultyConfig.Bind("Difficuly Scaling", "Difficulty max cap", 100.0f, "The difficulty value wont go beyond this.");
            scaleByDaysPassed = difficultyConfig.Bind("Difficuly Scaling", "Scale by days passed?", true, "Will add to difficulty depending on how many days have passed.");
            daysPassedDifficultyMultiplier = difficultyConfig.Bind("Difficuly Scaling", "Difficulty per days passed?", 1.0f, "");
            daysPassedDifficultyCap = difficultyConfig.Bind("Difficuly Scaling", "Days passed difficulty cap", 60.0f, "Days passed difficulty scaling wont add beyond this.");
            scaleByScrapInShip = difficultyConfig.Bind("Difficuly Scaling", "Scale by scrap in ship?", true, "Will add to difficulty depending on how much scrap is inside the ship.");
            scrapInShipDifficultyMultiplier = difficultyConfig.Bind("Difficuly Scaling", "Difficulty per scrap value in ship?", 0.00134f, "By default if scrap in ship is 7,500 => Difficulty +10.0, if 15,0000 => +20.0");
            scrapInShipDifficultyCap = difficultyConfig.Bind("Difficuly Scaling", "Days passed difficulty cap", 25.0f, "Scrap in ship difficulty scaling wont add beyond this.");
            scaleByMoonGrade = difficultyConfig.Bind("Difficuly Scaling", "Scale by moon grade?", true, "Will add to difficulty depending on grade of moon you land on.");
            gradeAdditives = new Dictionary<string, float>()
            {
                { "D", difficultyConfig.Bind("Difficuly Scaling", "D grade difficulty", -8.0f, "Difficulty added for moons grade of D").Value },
                { "C", difficultyConfig.Bind("Difficuly Scaling", "C grade difficulty", -8.0f, "Difficulty added for moons grade of C").Value },
                { "B", difficultyConfig.Bind("Difficuly Scaling", "B grade difficulty", -4.0f, "Difficulty added for moons grade of B").Value },
                { "A", difficultyConfig.Bind("Difficuly Scaling", "A grade difficulty", 5.0f, "Difficulty added for moons grade of A").Value },
                { "S", difficultyConfig.Bind("Difficuly Scaling", "S grade difficulty", 10.0f, "Difficulty added for moons grade of S").Value },
                { "S+", difficultyConfig.Bind("Difficuly Scaling", "S+ grade difficulty", 15.0f, "Difficulty added for moons grade of S+").Value },
                { "S++", difficultyConfig.Bind("Difficuly Scaling", "S++ grade difficulty", 20.0f, "Difficulty added for moons grade of S++").Value },
                { "S+++", difficultyConfig.Bind("Difficuly Scaling", "S+++ grade difficulty", 30.0f, "Difficulty added for moons grade of S+++").Value },
                { "Other", difficultyConfig.Bind("Difficuly Scaling", "Other grade difficulty", 10.0f, "For every other grade, use this value.").Value },
            };

            spawnChanceMultiplierScaling = getScale(difficultyConfig.Bind("Difficulty", "Spawn chance multiplier scale", "1.0, 0.017, 1.0, 2.0", "This will multiply the spawn chance by this,   " + scaleDescription).Value);
            insideSpawnChanceAdditive = getScale(difficultyConfig.Bind("Difficulty", "Inside spawn chance additive", "0.0, 0.0, 0.0, 0.0", "This will add to all keyframes for insideSpawns on the animationCurve,   " + scaleDescription).Value);
            outsideSpawnChanceAdditive = getScale(difficultyConfig.Bind("Difficulty", "Outside spawn chance additive", "0.0, 0.0, 0.0, 0.0", "This will add to all keyframes for outsideSpawns on the animationCurve,   " + scaleDescription).Value);
            spawnCapMultiplier = getScale(difficultyConfig.Bind("Difficulty", "Spawn cap multipler scale", "1.0, 0.017, 1.0, 2.0", "This will multiply outside and inside power counts by this,   " + scaleDescription).Value);
            insideEnemyMaxPowerCountScaling = getScale(difficultyConfig.Bind("Difficulty", "Additional Inside Max Enemy Power Count", "0, 0, 0, 0", "Added max enemy power count for inside enemies.,   " + scaleDescription).Value);
            outsideEnemyPowerCountScaling = getScale(difficultyConfig.Bind("Difficulty", "Additional Outside Max Enemy Power Count", "0, 0, 0, 0", "Added max enemy power count for outside enemies.,   " + scaleDescription).Value);
            enemyBonusHpScaling = getScale(difficultyConfig.Bind("Difficulty", "Additional hp", "0, 0.084, 0, 5", "Added hp to all enemies,   " + scaleDescription).Value);
            scrapValueMultiplier = getScale(difficultyConfig.Bind("Difficulty", "Global scrap value multiplier scale", "1.0, 0.0, 1.0, 1.0", "Mutliplies scrap value,   " + scaleDescription).Value);
            scrapAmountMultiplier = getScale(difficultyConfig.Bind("Difficulty", "Global scrap amount multiplier scale", "1.0, 0.0, 1.0, 1.0", "Mutliplies scrap value,   " + scaleDescription).Value);
            goodEventIncrementMultiplier = difficultyConfig.Bind("Difficulty", "Global multiplier for increment value on good and veryGood eventTypes.", 1.0f);
            badEventIncrementMultiplier = difficultyConfig.Bind("Difficulty", "Global multiplier for increment value on bad and veryBad eventTypes.", 1.0f);


            // Custom scrap settings
            nutSlayerLives = customAssetsConfig.Bind("NutSlayer", "Lives", 5, "If hp reaches zero or below, decrement lives and reset hp until 0 lives.");
            nutSlayerHp = customAssetsConfig.Bind("NutSlayer", "Hp", 6);
            nutSlayerMovementSpeed = customAssetsConfig.Bind("NutSlayer", "Speed?", 9.5f);
            nutSlayerImmortal = customAssetsConfig.Bind("NutSlayer", "Immortal?", true);
            grabbableTurret.minValue = customAssetsConfig.Bind("Grabbable Landmine", "Min value", 50).Value;
            grabbableTurret.maxValue = customAssetsConfig.Bind("Grabbable Landmine", "Max value", 75).Value;
            grabbableLandmine.minValue = customAssetsConfig.Bind("Grabbable Turret", "Min value", 100).Value;
            grabbableLandmine.maxValue = customAssetsConfig.Bind("Grabbable Turret", "Max value", 150).Value;
            slayerShotgunMinValue = customAssetsConfig.Bind("Slayer Shotgun", "Min value", 200);
            slayerShotgunMaxValue = customAssetsConfig.Bind("Slayer Shotgun", "Max value", 300);

            // Weather settings
            useWeatherMultipliers = weatherConfig.Bind("_Weather Settings", "Enable weather multipliers?", true, "'false'= Disable all weather multipliers     'true'= Enable weather multipliers");
            randomizeWeatherMultipliers = weatherConfig.Bind("_Weather Settings", "Weather multiplier randomness?", false, "'false'= disable     'true'= enable");
            enableTerminalText = weatherConfig.Bind("_Weather Settings", "Enable terminal text?", true);

            weatherRandomRandomMinInclusive = weatherConfig.Bind("_Weather Random Multipliers", "Min Inclusive", 0.9f, "Lower bound of random value");
            weatherRandomRandomMaxInclusive = weatherConfig.Bind("_Weather Random Multipliers", "Max Inclusive", 1.2f, "Upper bound of random value");

            Weather createWeatherSettings(Weather weather)
            {
                string configHeader = "(" + weather.weatherType.ToString() + ") Weather multipliers";

                float valueMultiplierSetting = weatherConfig.Bind(configHeader, "Scrap Value Multiplier", weather.scrapValueMultiplier, "Multiply Scrap value for " + weather.weatherType.ToString()).Value;
                float amountMultiplierSetting = weatherConfig.Bind(configHeader, "Scrap Amount Multiplier", weather.scrapAmountMultiplier, "Multiply Scrap amount for " + weather.weatherType.ToString()).Value;

                return new Weather(weather.weatherType, valueMultiplierSetting, amountMultiplierSetting);
            }

            noneMultiplier = createWeatherSettings(new Weather(LevelWeatherType.None, 1.00f, 1.00f));
            dustCloudMultiplier = createWeatherSettings(new Weather(LevelWeatherType.DustClouds, 1.05f, 1.00f));
            rainyMultiplier = createWeatherSettings(new Weather(LevelWeatherType.Rainy, 1.05f, 1.00f));
            stormyMultiplier = createWeatherSettings(new Weather(LevelWeatherType.Stormy, 1.35f, 1.20f));
            foggyMultiplier = createWeatherSettings(new Weather(LevelWeatherType.Foggy, 1.20f, 1.10f));
            floodedMultiplier = createWeatherSettings(new Weather(LevelWeatherType.Flooded, 1.25f, 1.15f));
            eclipsedMultiplier = createWeatherSettings(new Weather(LevelWeatherType.Eclipsed, 1.35f, 1.20f));

            // UI Settings
            UIKey = uiConfig.Bind("UI Options", "Toggle UI Key", "K");
            NormaliseScrapValueDisplay = uiConfig.Bind("UI Options", "Normlize scrap value display number?", true, "In game default value is 0.4, having this set to true will multiply the 'displayed value' by 2.5 so it looks normal.");
            EnableUI = uiConfig.Bind("UI Options", "Enable UI?", true);
            ShowUILetterBox = uiConfig.Bind("UI Options", "Display UI Letter Box?", true);
            ShowExtraProperties = uiConfig.Bind("UI Options", "Display extra properties", true, "Display extra properties on UI such as scrap value and amount multipliers.");
            PopUpUI = uiConfig.Bind("UI Options", "PopUp UI?", true, "Will the UI popup whenever you start the day?");
            UITime = uiConfig.Bind("UI Options", "PopUp UI time.", 45.0f, "UI popup time");
            scrollSpeed = uiConfig.Bind("UI Options", "Scroll speed", 1.0f, "Multiplier speed on scrolling with arrows.");
            DisplayUIAfterShipLeaves = uiConfig.Bind("UI Options", "Display UI after ship leaves?", false, "Will only display event's after ship has left.");

            // Event settings
            void RegisterEvents(ConfigFile toConfig, List<MEvent> events)
            {
                // Event settings
                foreach (MEvent e in events)
                {
                    eventWeights.Add(toConfig.Bind(e.Name(), "Custom Weight", e.Weight, "If you want to use custom weights change 'Use custom weights'? setting in '__Event Settings' to true."));
                    eventDescriptions.Add(ListToDescriptions(toConfig.Bind(e.Name(), "Descriptions", StringsToList(e.Descriptions, "|"), "Seperated by |").Value));
                    eventColorHexes.Add(toConfig.Bind(e.Name(), "Color Hex", e.ColorHex));
                    eventTypes.Add(toConfig.Bind(e.Name(), "Event Type", e.Type));
                    eventEnables.Add(toConfig.Bind(e.Name(), "Event Enabled?", e.Enabled, "Setting this to false will stop the event from occuring.")); // Normal event

                    // Make scale list
                    Dictionary<ScaleType, Scale> scales = new Dictionary<ScaleType, Scale>();
                    foreach (KeyValuePair<ScaleType, Scale> obj in e.ScaleList)
                    {
                        scales.Add(obj.Key, getScale(toConfig.Bind(e.Name(), obj.Key.ToString(), GetStringFromScale(obj.Value), ScaleInfoList[obj.Key] + "   " + scaleDescription).Value));
                    }
                    eventScales.Add(scales);

                    // EventsToRemove and EventsToSpawnWith
                    eventsToRemove.Add(ListToStrings(toConfig.Bind(e.Name(), "Events To Remove", StringsToList(e.EventsToRemove, ", "), "Will prevent said event(s) from occuring.").Value));
                    eventsToSpawnWith.Add(ListToStrings(toConfig.Bind(e.Name(), "Events To Spawn With", StringsToList(e.EventsToSpawnWith, ", "), "Will spawn said events(s).").Value));

                    // Monster events
                    List<MonsterEvent> newMonsterEvents = new List<MonsterEvent>();
                    for(int i = 0; i < e.monsterEvents.Count; i++)
                    {
                        newMonsterEvents.Add(new MonsterEvent(
                            toConfig.Bind(e.Name(), $"Enemy {i} Name", e.monsterEvents[i].enemy.name, "Inputting an invalid enemy name will cause it to return an empty enemyType").Value,
                            getScale(toConfig.Bind(e.Name(), $"{e.monsterEvents[i].enemy.name} {ScaleType.InsideEnemyRarity}", GetStringFromScale(e.monsterEvents[i].insideSpawnRarity), $"{ScaleInfoList[ScaleType.InsideEnemyRarity]}   {scaleDescription}").Value),
                            getScale(toConfig.Bind(e.Name(), $"{e.monsterEvents[i].enemy.name} {ScaleType.OutsideEnemyRarity}", GetStringFromScale(e.monsterEvents[i].outsideSpawnRarity), $"{ScaleInfoList[ScaleType.OutsideEnemyRarity]}   {scaleDescription}").Value),
                            getScale(toConfig.Bind(e.Name(), $"{e.monsterEvents[i].enemy.name} {ScaleType.MinInsideEnemy}", GetStringFromScale(e.monsterEvents[i].minInside), $"{ScaleInfoList[ScaleType.MinInsideEnemy]}   {scaleDescription}").Value),
                            getScale(toConfig.Bind(e.Name(), $"{e.monsterEvents[i].enemy.name} {ScaleType.MaxInsideEnemy}", GetStringFromScale(e.monsterEvents[i].maxInside), $"{ScaleInfoList[ScaleType.MaxInsideEnemy]}   {scaleDescription}").Value),
                            getScale(toConfig.Bind(e.Name(), $"{e.monsterEvents[i].enemy.name} {ScaleType.MinOutsideEnemy}", GetStringFromScale(e.monsterEvents[i].minOutside), $"{ScaleInfoList[ScaleType.MinOutsideEnemy]}   {scaleDescription}").Value),
                            getScale(toConfig.Bind(e.Name(), $"{e.monsterEvents[i].enemy.name} {ScaleType.MaxOutsideEnemy}", GetStringFromScale(e.monsterEvents[i].maxOutside), $"{ScaleInfoList[ScaleType.MaxOutsideEnemy]}   {scaleDescription}").Value)
                        ));
                    }
                    monsterEvents.Add(newMonsterEvents);

                    // Scrap transmutation events
                    SpawnableItemWithRarity[] newScrapTransmuations = new SpawnableItemWithRarity[e.scrapTransmutationEvent.items.Length];
                    for(int i = 0; i < e.scrapTransmutationEvent.items.Length; i++)
                    {
                        newScrapTransmuations[i] = new SpawnableItemWithRarity()
                        {
                            spawnableItem = GetItem(toConfig.Bind(e.Name(), $"Scrap {i} name", e.scrapTransmutationEvent.items[i].spawnableItem.name, "Inputting an invalid scrap name will cause it to return an empty item").Value),
                            rarity = toConfig.Bind(e.Name(), $"{e.scrapTransmutationEvent.items[i].spawnableItem.name} Rarity", e.scrapTransmutationEvent.items[i].rarity).Value
                        };
                    }
                    transmutationEvents.Add(new ScrapTransmutationEvent(newScrapTransmuations));
                }
            }

            // Custom enemy events
            int customEventCount = customEventConfig.Bind("_Temp Custom Monster Event Count", "How many events to generate in config?", 3, "This is temporary for the time being.").Value;
            for (int i = 0; i < customEventCount; i++)
            {
                MEvent e = new Minus.CustomEvents.CustomMonsterEvent();
                e.Enabled = false;
                e.Initalize();
                EventManager.customEvents.Add(e);
            }

            RegisterEvents(eventConfig, EventManager.vanillaEvents);
            RegisterEvents(moddedEventConfig, EventManager.moddedEvents);
            RegisterEvents(customEventConfig, EventManager.customEvents);

            EventManager.events.AddRange(EventManager.vanillaEvents);
            EventManager.events.AddRange(EventManager.moddedEvents);
            EventManager.events.AddRange(EventManager.customEvents);

            // Specific event settings
            Minus.Handlers.FacilityGhost.actionTimeCooldown = eventConfig.Bind(nameof(FacilityGhost), "Normal Action Time Interval", 15.0f, "How often does it take for the ghost to make a decision?").Value;
            Minus.Handlers.FacilityGhost.ghostCrazyActionInterval = eventConfig.Bind(nameof(FacilityGhost), "Crazy Action Time Interval", 0.1f, "How often does it take for the ghost to make a decision while going crazy?").Value;
            Minus.Handlers.FacilityGhost.ghostCrazyPeriod = eventConfig.Bind(nameof(FacilityGhost), "Crazy Period", 5.0f, "How long will the ghost go crazy for?").Value;
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

            DDay.bombardmentInterval = eventConfig.Bind(nameof(Warzone), "Bombardment interval", 100, "The time it takes before each bombardment event.").Value;
            DDay.bombardmentTime = eventConfig.Bind(nameof(Warzone), "Bombardment time", 15, "When a bombardment event occurs, how long will it last?").Value;
            DDay.fireInterval = eventConfig.Bind(nameof(Warzone), "Fire interval", 1, "During a bombardment event how often will it fire?").Value;
            DDay.fireAmount = eventConfig.Bind(nameof(Warzone), "Fire amount", 8, "For every fire interval, how many shot's will it take? This will get scaled higher on bigger maps.").Value;
            DDay.displayWarning = eventConfig.Bind(nameof(Warzone), "Display warning?", true, "Display warning message before bombardment?").Value;
            DDay.volume = eventConfig.Bind(nameof(Warzone), "Siren Volume?", 0.3f, "Volume of the siren? between 0.0 and 1.0").Value;
            ArtilleryShell.speed = eventConfig.Bind(nameof(Warzone), "Artillery shell speed", 100.0f, "How fast does the artillery shell travel?").Value;

            Minus.Handlers.Mimics.spawnRateScales[0] = getScale(moddedEventConfig.Bind(nameof(Mimics), "Zero Mimics Scale", "0, 0, 0, 0", "Weight Scale of zero mimics spawning   " + scaleDescription).Value);
            Minus.Handlers.Mimics.spawnRateScales[1] = getScale(moddedEventConfig.Bind(nameof(Mimics), "One Mimic Scale", "0, 0, 0, 0", "Weight Scale of one mimic spawning   " + scaleDescription).Value);
            Minus.Handlers.Mimics.spawnRateScales[2] = getScale(moddedEventConfig.Bind(nameof(Mimics), "Two Mimics Scale", "0, 0, 0, 0", "Weight Scale of two mimics spawning   " + scaleDescription).Value);
            Minus.Handlers.Mimics.spawnRateScales[3] = getScale(moddedEventConfig.Bind(nameof(Mimics), "Three Mimics Scale", "80.0, -1.25, 5.0, 80.0", "Weight Scale of three mimics spawning   " + scaleDescription).Value);
            Minus.Handlers.Mimics.spawnRateScales[4] = getScale(moddedEventConfig.Bind(nameof(Mimics), "Four Mimics Scale", "40.0, -0.5, 10.0, 40.0", "Weight Scale of four mimics spawning   " + scaleDescription).Value);
            Minus.Handlers.Mimics.spawnRateScales[5] = getScale(moddedEventConfig.Bind(nameof(Mimics), "Maximum Mimics Scale", "10.0, 0.84, 10.0, 60.0", "Weight Scale of maximum mimics spawning   " + scaleDescription).Value);
        }

        internal static bool Initalized = false;
        public static void CreateConfig()
        {
            if (Initalized) return;

            // Initalize Events
            foreach (MEvent e in EventManager.vanillaEvents) e.Initalize();
            foreach (MEvent e in EventManager.moddedEvents) e.Initalize();

            // Config
            Initalize();

            // Use config settings
            for (int i = 0; i != EventManager.events.Count; i++)
            {
                EventManager.events[i].Weight = eventWeights[i].Value;
                EventManager.events[i].Descriptions = eventDescriptions[i];
                EventManager.events[i].ColorHex = eventColorHexes[i].Value;
                EventManager.events[i].Type = eventTypes[i].Value;
                EventManager.events[i].ScaleList = eventScales[i];
                EventManager.events[i].Enabled = eventEnables[i].Value;
                EventManager.events[i].EventsToRemove = eventsToRemove[i];
                EventManager.events[i].EventsToSpawnWith = eventsToSpawnWith[i];
                EventManager.events[i].monsterEvents = monsterEvents[i];
                EventManager.events[i].scrapTransmutationEvent = transmutationEvents[i];
            }

            // Create disabled events list and update
            List<MEvent> newEvents = new List<MEvent>();
            foreach (MEvent e in EventManager.events)
            {
                if (!e.Enabled)
                {
                    EventManager.disabledEvents.Add(e);
                }
                else
                {
                    newEvents.Add(e);
                    switch(e.Type)
                    {
                        case EventType.VeryBad:
                            EventManager.allVeryBad.Add(e);
                            break;
                        case EventType.Bad:
                            EventManager.allBad.Add(e);
                            break;
                        case EventType.Neutral:
                            EventManager.allNeutral.Add(e);
                            break;
                        case EventType.Good:
                            EventManager.allGood.Add(e);
                            break;
                        case EventType.VeryGood:
                            EventManager.allVeryGood.Add(e);
                            break;
                        case EventType.Remove:
                            EventManager.allRemove.Add(e);
                            break;
                    }
                }
            }
            EventManager.events = newEvents;

            EventManager.UpdateEventTypeCounts();
            if (!useCustomWeights.Value) EventManager.UpdateAllEventWeights();

            Log.LogInfo(
                $"\n\nTotal Events:{EventManager.events.Count},   Disabled Events:{EventManager.disabledEvents.Count},   Total Events - Remove Count:{EventManager.events.Count - EventManager.eventTypeCount[5]}\n" +
                $"Very Bad:{EventManager.eventTypeCount[0]}\n" +
                $"Bad:{EventManager.eventTypeCount[1]}\n" +
                $"Neutral:{EventManager.eventTypeCount[2]}\n" +
                $"Good:{EventManager.eventTypeCount[3]}\n" +
                $"Very Good:{EventManager.eventTypeCount[4]}\n" +
                $"Remove:{EventManager.eventTypeCount[5]}\n");

            CreateAllEnemiesConfig();

            Initalized = true;
        }

        private static void CreateAllEnemiesConfig()
        {
            enableAllEnemies = allEnemiesConfig.Bind("_All Enemies", "Enable?", false, "This will make all enemies capable of spawning on all moons, this will make the game harder.");
            enableAllAllEnemies = allEnemiesConfig.Bind("_All All Enemies", "Enable?", false, "This will make all inside enemies spawn inside and outside and all outside enemies spawn inside and outside, so giants and worms can spawn inside, enable both 'All' and 'All All' if you are a sadist. This will make the game harder.");

            List<EnemySpawnInfo> allSpawnInfos = new List<EnemySpawnInfo>()
            {
                // Inside
                CreateEnemyEntry(EnemyName.Bracken, 8, 1, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.HoardingBug, 60, 10, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.CoilHead, 20, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Thumper, 25, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.BunkerSpider, 35, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Jester, 7, 1, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.SnareFlea, 45, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Hygrodere, 10, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.GhostGirl, 5, 1, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.SporeLizard, 15, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.NutCracker, 15, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Masked, 10, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Butler, 20, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Lasso, 5, 1, SpawnLocation.Inside),
                CreateEnemyEntry(kamikazieBug.name, 30, 5, SpawnLocation.Inside),
                CreateEnemyEntry(antiCoilHead.name, 10, 2, SpawnLocation.Inside),
                CreateEnemyEntry(nutSlayer.name, 3, 1, SpawnLocation.Inside),
                // Outside
                CreateEnemyEntry(EnemyName.EyelessDog, 25, 5, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.ForestKeeper, 10, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.EarthLeviathan, 8, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.BaboonHawk, 35, 10, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.OldBird, 6, 3, SpawnLocation.Outside)
            };

            foreach(EnemyType enemy in EnemyList.Values)
            {
                if (enemy == null || enemy.enemyPrefab == null || enemy.isDaytimeEnemy || allSpawnInfos.Exists(x => x.enemy.name == enemy.name)) continue;

                if(enemy.isOutsideEnemy)
                {
                    CreateEnemyEntry(enemy.name, 5, 1, SpawnLocation.Outside);
                } else
                {
                    CreateEnemyEntry(enemy.name, 5, 1, SpawnLocation.Inside);
                }
            }

            allEnemiesCycle = new SpawnCycle()
            {
                enemies = allSpawnInfos.ToArray(),
                nothingWeight = allEnemiesConfig.Bind("_All Enemies", "All enemies nothing weight", 400.0f, "This is the weight chance for a spawn to not occur.").Value,
                spawnAttemptInterval = allEnemiesConfig.Bind("_All Enemies", "Spawn interval", 86.0f, "How often will this cycle attempt to spawn an enemy? in seconds").Value,
                spawnCycleDuration = 0.0f
            };

            header = "All All Enemies";
            List<EnemySpawnInfo> allAllSpawnInfos = new List<EnemySpawnInfo>()
            {
                // Inside
                CreateEnemyEntry(EnemyName.Bracken, 8, 1, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.HoardingBug, 60, 10, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.CoilHead, 20, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Thumper, 25, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.BunkerSpider, 35, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Jester, 7, 1, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.SnareFlea, 45, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Hygrodere, 10, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.GhostGirl, 5, 1, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.SporeLizard, 15, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.NutCracker, 15, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Masked, 10, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Butler, 20, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Lasso, 5, 1, SpawnLocation.Inside),
                CreateEnemyEntry(kamikazieBug.name, 30, 5, SpawnLocation.Inside),
                CreateEnemyEntry(antiCoilHead.name, 10, 2, SpawnLocation.Inside),
                CreateEnemyEntry(nutSlayer.name, 3, 1, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.EyelessDog, 10, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.ForestKeeper, 6, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.EarthLeviathan, 8, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.BaboonHawk, 20, 10, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.OldBird, 6, 3, SpawnLocation.Inside),
                // Outside
                CreateEnemyEntry(EnemyName.Bracken, 4, 1, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.HoardingBug, 30, 10, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.CoilHead, 10, 5, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.Thumper, 13, 5, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.BunkerSpider, 18, 5, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.Jester, 3, 1, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.SnareFlea, 10, 5, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.Hygrodere, 5, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.GhostGirl, 3, 1, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.SporeLizard, 7, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.NutCracker, 8, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.Masked, 5, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.Butler, 10, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.Lasso, 3, 1, SpawnLocation.Outside),
                CreateEnemyEntry(kamikazieBug.name, 15, 5, SpawnLocation.Outside),
                CreateEnemyEntry(antiCoilHead.name, 5, 2, SpawnLocation.Outside),
                CreateEnemyEntry(nutSlayer.name, 2, 1, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.EyelessDog, 15, 5, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.ForestKeeper, 10, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.EarthLeviathan, 12, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.BaboonHawk, 35, 10, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.OldBird, 10, 3, SpawnLocation.Outside)
            };

            foreach (EnemyType enemy in EnemyList.Values)
            {
                if (enemy == null || enemy.enemyPrefab == null || allSpawnInfos.Exists(x => x.enemy.name == enemy.name)) continue;

                CreateEnemyEntry(enemy.name, 5, 1, SpawnLocation.Inside);
                CreateEnemyEntry(enemy.name, 5, 1, SpawnLocation.Outside);
            }

            allAllEnemiesCycle = new SpawnCycle()
            {
                enemies = allAllSpawnInfos.ToArray(),
                nothingWeight = allEnemiesConfig.Bind("_All All Enemies", "All enemies nothing weight", 400.0f, "This is the weight chance for a spawn to not occur.").Value,
                spawnAttemptInterval = allEnemiesConfig.Bind("_All All Enemies", "Spawn interval", 86.0f, "How often will this cycle attempt to spawn enemies? in seconds").Value,
                spawnCycleDuration = 0.0f
            };
        }

        private static string header = "All Enemies";
        private static EnemySpawnInfo CreateEnemyEntry(string enemy, float defaultWeight, int spawnCap, SpawnLocation spawnLocation)
        {
            return new EnemySpawnInfo()
            {
                enemy = GetEnemyOrDefault(enemy).enemyPrefab,
                enemyWeight = allEnemiesConfig.Bind(header, $"{spawnLocation} {enemy} Weight", defaultWeight, "weight").Value,
                spawnCap = allEnemiesConfig.Bind(header, $"{spawnLocation} {enemy} Spawn Cap", spawnCap, "weight").Value,
                spawnLocation = spawnLocation
            };
        }

        private static EnemySpawnInfo CreateEnemyEntry(EnemyName name, float defaultWeight, int spawnCap, SpawnLocation spawnLocation) => CreateEnemyEntry(EnemyNameList[name], defaultWeight, spawnCap, spawnLocation);

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TimeOfDay), "Awake")]
        private static void OnTimeDayStart(ref TimeOfDay __instance)
        {
            enableQuotaChanges = difficultyConfig.Bind("Quota Settings", "_Enable Quota Changes", false);
            if(enableQuotaChanges.Value)
            {
                __instance.quotaVariables.deadlineDaysAmount = difficultyConfig.Bind("Quota Settings", "Deadline Days Amount", __instance.quotaVariables.deadlineDaysAmount).Value;
                __instance.quotaVariables.startingCredits = difficultyConfig.Bind("Quota Settings", "Starting Credits", __instance.quotaVariables.startingCredits).Value;
                __instance.quotaVariables.startingQuota = difficultyConfig.Bind("Quota Settings", "Starting Quota", __instance.quotaVariables.startingQuota).Value;
                __instance.quotaVariables.baseIncrease = difficultyConfig.Bind("Quota Settings", "Base Increase", __instance.quotaVariables.baseIncrease).Value;
                __instance.quotaVariables.increaseSteepness = difficultyConfig.Bind("Quota Settings", "Increase Steepness", __instance.quotaVariables.increaseSteepness).Value;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Terminal), "Awake")]
        private static void OnTerminalAwake()
        {
            Manager.currentTerminal = GameObject.FindObjectOfType<Terminal>();
        }
    }
}
