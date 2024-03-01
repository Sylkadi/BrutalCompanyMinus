using System;
using Unity.Netcode;
using HarmonyLib;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BrutalCompanyMinus.Minus
{
    internal struct Weather : INetworkSerializable, IEquatable<Weather>
    {

        public LevelWeatherType weatherType;

        public float scrapValueMultiplier;
        public float scrapAmountMultiplier;
        public float factorySizeMultiplier;

        public Weather(LevelWeatherType weatherType, float scrapValueMultiplier, float scrapAmountMultiplier, float factorySizeMultiplier)
        {
            this.weatherType = weatherType;
            this.scrapValueMultiplier = scrapValueMultiplier;
            this.scrapAmountMultiplier = scrapAmountMultiplier;
            this.factorySizeMultiplier = factorySizeMultiplier;
        }

        public static Weather operator *(Weather left, Weather right)
        {
            return new Weather(left.weatherType, left.scrapValueMultiplier * right.scrapValueMultiplier, left.scrapAmountMultiplier * right.scrapAmountMultiplier, left.factorySizeMultiplier * right.factorySizeMultiplier);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                FastBufferReader reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out weatherType);
                reader.ReadValueSafe(out scrapValueMultiplier);
                reader.ReadValueSafe(out scrapAmountMultiplier);
                reader.ReadValueSafe(out factorySizeMultiplier);
            }
            else
            {
                FastBufferWriter writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(weatherType);
                writer.WriteValueSafe(scrapValueMultiplier);
                writer.WriteValueSafe(scrapAmountMultiplier);
                writer.WriteValueSafe(factorySizeMultiplier);
            }
        }

        public bool Equals(Weather other)
        {
            return weatherType == other.weatherType;
        }

        public static NetworkList<Weather> InitalizeWeatherMultipliers(NetworkList<Weather> currentWeatherMultipliers)
        {
            // Set Initial Values
            currentWeatherMultipliers.Add(Configuration.noneMultiplier);
            currentWeatherMultipliers.Add(Configuration.dustCloudMultiplier);
            currentWeatherMultipliers.Add(Configuration.rainyMultiplier);
            currentWeatherMultipliers.Add(Configuration.stormyMultiplier);
            currentWeatherMultipliers.Add(Configuration.foggyMultiplier);
            currentWeatherMultipliers.Add(Configuration.floodedMultiplier);
            currentWeatherMultipliers.Add(Configuration.eclipsedMultiplier);

            return currentWeatherMultipliers;
        }

        public static NetworkList<Weather> RandomizeWeatherMultipliers(NetworkList<Weather> currentWeatherMultipliers)
        {
            if (!Configuration.randomizeWeatherMultipliers.Value) return currentWeatherMultipliers;
            for (int i = 0; i < currentWeatherMultipliers.Count; i++)
            {
                float minInclusive = Configuration.weatherRandomRandomMinInclusive.Value, maxInclusive = Configuration.weatherRandomRandomMaxInclusive.Value;

                Weather multipliers = new Weather(currentWeatherMultipliers[i].weatherType, UnityEngine.Random.Range(minInclusive, maxInclusive), UnityEngine.Random.Range(minInclusive, maxInclusive), UnityEngine.Random.Range(minInclusive, maxInclusive));

                switch (currentWeatherMultipliers[i].weatherType)
                {
                    case LevelWeatherType.None:
                        currentWeatherMultipliers[i] = multipliers * Configuration.noneMultiplier;
                        break;
                    case LevelWeatherType.DustClouds:
                        currentWeatherMultipliers[i] = multipliers * Configuration.dustCloudMultiplier;
                        break;
                    case LevelWeatherType.Rainy:
                        currentWeatherMultipliers[i] = multipliers * Configuration.rainyMultiplier;
                        break;
                    case LevelWeatherType.Stormy:
                        currentWeatherMultipliers[i] = multipliers * Configuration.stormyMultiplier;
                        break;
                    case LevelWeatherType.Foggy:
                        currentWeatherMultipliers[i] = multipliers * Configuration.foggyMultiplier;
                        break;
                    case LevelWeatherType.Flooded:
                        currentWeatherMultipliers[i] = multipliers * Configuration.floodedMultiplier;
                        break;
                    case LevelWeatherType.Eclipsed:
                        currentWeatherMultipliers[i] = multipliers * Configuration.eclipsedMultiplier;
                        break;
                }
            }
            return currentWeatherMultipliers;
        }
    }

    [HarmonyPatch]
    [HarmonyPatch(typeof(Terminal))]
    internal class WeatherTerminal
    {
        [HarmonyPrefix]
        [HarmonyPatch("TextPostProcess")]
        [HarmonyPriority(Priority.First)]
        public static void OnLoadNewNode(ref string modifiedDisplayText)
        {
            if (!Configuration.useWeatherMultipliers.Value || !Configuration.enableTerminalText.Value) return;

            // Are we on moons tab? else return
            if (!modifiedDisplayText.Contains("Welcome to the exomoons catalogue")) return;

            if(modifiedDisplayText.Contains("[planetTime]"))
            {
                Log.LogInfo("Text contains '[planetTime]' modifying terminal text using ModifyTextVanilla()");
                ModifyTextVanilla(ref modifiedDisplayText);
            } else
            {
                Log.LogInfo("Text dosen't contain '[planetTime]' modifying terminal text using ModifyTextNonVanilla()");
                ModifyTextNonVanilla(ref modifiedDisplayText);
            }
        }

        private static void ModifyTextNonVanilla(ref string modifiedDisplayText)
        {
            // Add format text
            int index = modifiedDisplayText.IndexOf("INFO.");
            if (index > 0) modifiedDisplayText = modifiedDisplayText.Insert(index + 5, "\nFormat: (xScrapValue, xScrapAmount, xFactorySize)");

            // Create index list of '*' on all moons
            List<int> indexList = new List<int>();
            index = modifiedDisplayText.IndexOf("*") + 1;
            while (index > 0)
            {
                index = modifiedDisplayText.IndexOf("*", index + 1);
                indexList.Add(index);
            }
            indexList.Remove(-1);

            // Create moon text list
            List<string> moonTextList = new List<string>();
            for (int i = 0; i < indexList.Count - 1; i++)
            {
                moonTextList.Add(modifiedDisplayText.Substring(indexList[i], indexList[i + 1] - indexList[i] - 1));
            }
            moonTextList.Add(modifiedDisplayText.Substring(indexList[indexList.Count - 1], modifiedDisplayText.Length - indexList[indexList.Count - 1] - 1));

            string[] oldMoonTextList = moonTextList.ToArray();
            // Make new moon text list
            for (int i = 0; i < moonTextList.Count; i++)
            {
                // Remove new line occurence
                bool RemovedNewLine = false;
                moonTextList[i] = Regex.Replace(moonTextList[i], @"\r|\n", "");
                if (moonTextList[i] != oldMoonTextList[i]) RemovedNewLine = true;

                // Check if weather is 'none'
                index = moonTextList[i].IndexOf("(");
                if (index > 0) // Moon has weather
                {
                    foreach (Weather w in Net.Instance.currentWeatherMultipliers)
                    {
                        if (moonTextList[i].Contains(w.weatherType.ToString()))
                        {
                            string multiplierText =
                                " (x" + w.scrapValueMultiplier.ToString("F2") +
                                ", x" + w.scrapAmountMultiplier.ToString("F2") +
                                ", x" + w.factorySizeMultiplier.ToString("F2") + ")";
                            moonTextList[i] = moonTextList[i].Insert(moonTextList[i].Length, multiplierText);
                        }
                    }
                }
                else // Moon weather is 'none'
                {
                    string multiplierText =
                        "x" + Net.Instance.currentWeatherMultipliers[0].scrapValueMultiplier.ToString("F2") +
                        ", x" + Net.Instance.currentWeatherMultipliers[0].scrapAmountMultiplier.ToString("F2") +
                        ", x" + Net.Instance.currentWeatherMultipliers[0].factorySizeMultiplier.ToString("F2");
                    moonTextList[i] = moonTextList[i].Insert(moonTextList[i].Length - 1, "( " + multiplierText + ")");
                }

                // Give back new line
                if (RemovedNewLine) moonTextList[i] += "\r\n";
            }

            // Update text
            for(int i = 0; i < oldMoonTextList.Length; i++)
            {
                modifiedDisplayText = modifiedDisplayText.Replace(oldMoonTextList[i], moonTextList[i]);
            }

        }

        private static void ModifyTextVanilla(ref string modifiedDisplayText)
        {
            // Add format text
            int index = modifiedDisplayText.IndexOf("INFO.");
            if (index > 0) modifiedDisplayText = modifiedDisplayText.Insert(index + 5, "\nFormat: [xScrapValue, xScrapAmount, xFactorySize]");

            // Update moon text list
            int planeTimeCount = modifiedDisplayText.Split("[planetTime]").Length - 1;
            Regex planeTimeRegex = new Regex(Regex.Escape("[planetTime]"));

            for (int i = 0; i < planeTimeCount && i < Manager.currentTerminal.moonsCatalogueList.Length; i++)
            {
                LevelWeatherType weatherType = Manager.currentTerminal.moonsCatalogueList[i].currentWeather;

                index = modifiedDisplayText.IndexOf("[planetTime]", index + 1);
                foreach (Weather w in Net.Instance.currentWeatherMultipliers)
                {
                    if (w.weatherType != weatherType) continue;

                    string addExtraSpace = (w.weatherType == LevelWeatherType.None) ? "" : " ";
                    string replacement = $"[planetTime]{addExtraSpace}(x{w.scrapValueMultiplier.ToString("F2", Configuration.en)}, x{w.scrapAmountMultiplier.ToString("F2", Configuration.en)}, x{w.factorySizeMultiplier.ToString("F2", Configuration.en)})";
                    modifiedDisplayText = planeTimeRegex.Replace(modifiedDisplayText, replacement, 1, index);
                }
            }
        }
    }
}
