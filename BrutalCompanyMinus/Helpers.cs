using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;
using BepInEx;
using static BrutalCompanyMinus.Minus.MEvent;
using System.Globalization;
using BrutalCompanyMinus.Minus;
using UnityEngine.AI;
using static BrutalCompanyMinus.Minus.EventManager;

namespace BrutalCompanyMinus
{
    public static class Helper
    {
        internal static CultureInfo en = new CultureInfo("en-US"); // This is important, no touchy

        public static List<Vector3> GetOutsideNodes() => GameObject.FindGameObjectsWithTag("OutsideAINode").Select(n => n.transform.position).ToList();
        public static List<Vector3> GetInsideAINodes() => GameObject.FindGameObjectsWithTag("AINode").Select(n => n.transform.position).ToList();

        /// <summary>
        /// Gets spawn denial Nodes.
        /// </summary>
        /// <returns>This returns SpawnDenialPoints, ItemShipLandingNode and EntranceTeleport positions.</returns>
        public static List<Vector3> GetSpawnDenialNodes()
        {
            List<Vector3> nodes = GameObject.FindGameObjectsWithTag("SpawnDenialPoint").Select(n => n.transform.position).ToList();
            nodes.Add(GameObject.FindGameObjectWithTag("ItemShipLandingNode").transform.position);
            nodes.AddRange(GameObject.FindObjectsOfType<EntranceTeleport>().Select(i => i.gameObject.transform.position).ToList());
            nodes.AddRange(GameObject.FindObjectsOfType<InteractTrigger>().Where(l => l.isLadder).Select(l => l.gameObject.transform.position).ToList());
            return nodes;
        }

        internal static int[] IntArray(this float[] Values)
        {
            int[] newValues = new int[Values.Length];
            for (int i = 0; i < Values.Length; i++)
            {
                newValues[i] = (int)Values[i];
            }
            return newValues;
        }

        /// <summary>
        /// This is primarly used to add landmine, turret and spiketrap spawns to the array.
        /// </summary>
        /// <returns>New array with added object.</returns>
        public static SpawnableMapObject[] Add(this SpawnableMapObject[] toObjects, SpawnableMapObject newObject)
        {
            SpawnableMapObject[] newMapObjects = new SpawnableMapObject[toObjects.Length + 1];
            for(int i = 0; i < toObjects.Length; i++)
            {
                newMapObjects[i] = toObjects[i];
            }
            newMapObjects[toObjects.Length] = newObject;
            return newMapObjects;
        }

        internal static WeatherEffect[] Add(this WeatherEffect[] toObjects, WeatherEffect newObject)
        {
            WeatherEffect[] newMapObjects = new WeatherEffect[toObjects.Length + 1];
            for (int i = 0; i < toObjects.Length; i++)
            {
                newMapObjects[i] = toObjects[i];
            }
            newMapObjects[toObjects.Length] = newObject;
            return newMapObjects;
        }

        /// <summary>
        /// This is used by primarly used to reality shift to teleport enemies away from the player.
        /// </summary>
        /// <returns>Returns new position from pos and inbetween minRadius and maxRadius</returns>
        public static Vector3 GetRandomNavMeshPositionInBox(Vector3 pos, float minRadius, float maxRadius)
        {
            float halfPointRadius = (maxRadius + minRadius) * 0.5f;
            float betweenRadius = (maxRadius - minRadius) * 0.5f;

            for(int i = 0; i < 15; i++) // 15 Attempts 
            {
                UnityEngine.Random.InitState(Net.Instance._seed++ + Environment.TickCount);
                Vector3 newPos = new Vector3((Mathf.Cos(UnityEngine.Random.Range(0, Mathf.PI * 2)) * halfPointRadius) + pos.x, pos.y, (Mathf.Sin(UnityEngine.Random.Range(0, Mathf.PI * 2)) * halfPointRadius) + pos.z);
                if (NavMesh.SamplePosition(newPos, out NavMeshHit navHit, betweenRadius, -1) && Vector3.Distance(pos, newPos) >= minRadius)
                {
                    return navHit.position;
                }
            }
            return pos;
        }

        internal static string GetPercentage(float value) => (value * 100.0f).ToString("F1") + "%";

        internal static string GetDifficultyColorHex(float difficulty, float cap)
        {
            if (cap == 0) cap = 1.0f;
            difficulty *= Configuration.difficultyMaxCap.Value / cap;

            DifficultyTransition[] chosenAndNextTransitions = GetChosenAndNextTransition(difficulty);

            return chosenAndNextTransitions[0].GetTransitionHex(chosenAndNextTransitions[1]);
        }

        internal static string GetDifficultyText()
        {
            DifficultyTransition[] chosenAndNextTransitions = GetChosenAndNextTransition(Manager.difficulty);
            return $"<color=#{chosenAndNextTransitions[0].GetTransitionHex(chosenAndNextTransitions[1])}>{chosenAndNextTransitions[0].name}</color>";
        }
        internal static DifficultyTransition[] GetChosenAndNextTransition(float difficulty)
        {
            DifficultyTransition chosenTransition = Configuration.difficultyTransitions[0],
                                 nextTransition = Configuration.difficultyTransitions[0];

            int index = 0;
            for (int i = 0; i < Configuration.difficultyTransitions.Length; i++)
            {
                if (Configuration.difficultyTransitions[i].above <= difficulty)
                {
                    chosenTransition = Configuration.difficultyTransitions[i];
                    index = i;
                }
            }

            if (index == Configuration.difficultyTransitions.Length - 1)
            {
                nextTransition = chosenTransition;
            }
            else
            {
                nextTransition = Configuration.difficultyTransitions[index + 1];
            }

            return new DifficultyTransition[] { chosenTransition, nextTransition };
        }

        internal static Vector3 GetSafePosition(List<Vector3> nodes, List<Vector3> denialNodes, float radius, int seed)
        {
            System.Random rng = new System.Random(seed);
            UnityEngine.Random.InitState(seed);
            Vector3 position = nodes[rng.Next(nodes.Count)];
            int Iteration = 0;

            while (true)
            {
                Iteration++;
                Vector3 newPosition = RoundManager.Instance.GetRandomNavMeshPositionInRadius(position, radius);
                bool foundSafe = true;
                foreach (Vector3 node in denialNodes)
                {
                    if (Vector3.Distance(node, newPosition) <= 16.0f) foundSafe = false;
                }
                if (foundSafe)
                {
                    position = newPosition;
                    break;
                }
                if (Iteration > 51)
                {
                    Log.LogError("GetSafePosition() got stuck, returning " + position);
                    break;
                }
                if (Iteration % 10 == 0) // Refresh if not found
                {
                    position = nodes[UnityEngine.Random.Range(0, nodes.Count)];
                }
            }

            return position;
        }

        public static bool IsSafe(Vector3 testNode, List<Vector3> fromNodes, float radius)
        {
            foreach(Vector3 node in fromNodes)
            {
                if(Vector3.Distance(testNode, node) <= radius) return false;
            }
            return true;
        }

        public static string MostCommon(List<string> list)
        {
            string mostCommon = "";

            if (list != null && list.Count > 0)
            {
                Dictionary<string, int> counts = new Dictionary<string, int>();

                foreach (string s in list)
                {
                    if (counts.ContainsKey(s))
                    {
                        counts[s]++;
                    }
                    else
                    {
                        counts.Add(s, 1);
                    }
                }

                int max = 0;
                foreach (KeyValuePair<string, int> count in counts)
                {
                    if (count.Value > max)
                    {
                        mostCommon = count.Key;
                        max = count.Value;
                    }
                }

            }
            return mostCommon;
        }

        internal static Scale getScale(string from)
        {
            float[] values = ParseValuesFromString(from);
            return new Scale(values[0], values[1], values[2], values[3]);
        }

        internal static string GetStringFromScale(Scale from) => $"{from.Base.ToString(en)}, {from.Increment.ToString(en)}, {from.MinCap.ToString(en)}, {from.MaxCap.ToString(en)}";

        internal static float[] ParseValuesFromString(string from)
        {
            return from.Split(',').Select(x => float.Parse(x, en)).ToArray();
        }

        internal static string StringsToList(List<string> strings, string seperator) // This is confusing naming
        {
            string text = "";
            foreach (string s in strings)
            {
                text += s;
                text += seperator;
            }
            if (strings.Count > 0) text = text.Substring(0, text.Length - seperator.Length);
            return text;
        }

        internal static List<string> ListToStrings(string text)
        {
            if (text.IsNullOrWhiteSpace()) return new List<string>();
            text = text.Replace(" ", "");
            return text.Split(',').ToList();
        }

        internal static List<string> ListToDescriptions(string text)
        {
            if (text.IsNullOrWhiteSpace()) return new List<string>() { "" };
            return text.Split("|").ToList();
        }

        internal static DifficultyTransition[] GetDifficultyTransitionsFromString(string s)
        {
            string[] strings = s.Split("|");

            DifficultyTransition[] transitions = new DifficultyTransition[strings.Length];

            for(int i = 0; i < strings.Length; i++)
            {
                string[] properties = strings[i].Split(",");
                if(properties.Length != 3)
                {
                    Log.LogError($"DifficultyTransition config entry is of length:{properties.Length}, must be of length 3, returning a working version of difficultyTransitions.");
                    return new DifficultyTransition[2] { new DifficultyTransition("Easy", "FFFFFF", 10.0f), new DifficultyTransition("Medium", "000000", 20.0f) }; 
                }

                float value = i;
                try
                {
                    value = float.Parse(properties[2]);
                } catch
                {
                    Log.LogError($"Failed to parse number from difficulty transition, value is going to be {i}.");
                }

                transitions[i] = new DifficultyTransition(properties[0], properties[1], value);
            }

            Array.Sort(transitions);
            return transitions;
        }


        internal static Dictionary<string, float> GetMoonRiskFromString(string text)
        {
            Dictionary<string, float> moonRiskValues = new Dictionary<string, float>();

            string[] strings = text.Split("|");

            for(int i = 0; i < strings.Length; i++)
            {
                string[] properties = strings[i].Split(",");

                float value = 0.0f;
                try
                {
                    value = float.Parse(properties[1]);
                } catch
                {
                    Log.LogError($"Moon Risk Difficulty Entry contains a value that isn't a number, value will be 0, attempted input {properties[1]}.");
                }

                if (!moonRiskValues.TryAdd(properties[0], value))
                {
                    Log.LogError($"Entry {properties[0]} already exists in the dicitionary.");
                }
            }

            return moonRiskValues;
        }

        internal static IList<Vector2> ComputeConvexHull(List<Vector2> points, bool sortInPlace = false) // Taken from https://gist.github.com/dLopreiato/7fd142d0b9728518552188794b8a750c
        {
            if (!sortInPlace)
                points = new List<Vector2>(points);
            points.Sort((a, b) =>
                a.x == b.x ? a.y.CompareTo(b.y) : (a.x > b.x ? 1 : -1));

            CircularList<Vector2> hull = new CircularList<Vector2>();
            int L = 0, U = 0;

            for (int i = points.Count - 1; i >= 0; i--)
            {
                Vector2 p = points[i], p1;

                while (L >= 2 && (p1 = hull.Last).Sub(hull[hull.Count - 2]).Cross(p.Sub(p1)) >= 0)
                {
                    hull.PopLast();
                    L--;
                }
                hull.PushLast(p);
                L++;

                while (U >= 2 && (p1 = hull.First).Sub(hull[1]).Cross(p.Sub(p1)) <= 0)
                {
                    hull.PopFirst();
                    U--;
                }
                if (U != 0)
                    hull.PushFirst(p);
                U++;
                Debug.Assert(U + L == hull.Count + 1);
            }
            hull.PopLast();
            return hull;
        }

        private static Vector2 Sub(this Vector2 a, Vector2 b)
        {
            return a - b;
        }

        private static float Cross(this Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        private class CircularList<T> : List<T>
        {
            public T Last
            {
                get
                {
                    return this[this.Count - 1];
                }
                set
                {
                    this[this.Count - 1] = value;
                }
            }

            public T First
            {
                get
                {
                    return this[0];
                }
                set
                {
                    this[0] = value;
                }
            }

            public void PushLast(T obj)
            {
                this.Add(obj);
            }

            public T PopLast()
            {
                T retVal = this[this.Count - 1];
                this.RemoveAt(this.Count - 1);
                return retVal;
            }

            public void PushFirst(T obj)
            {
                this.Insert(0, obj);
            }

            public T PopFirst()
            {
                T retVal = this[0];
                this.RemoveAt(0);
                return retVal;
            }
        }

    }
}
