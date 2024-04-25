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

namespace BrutalCompanyMinus
{
    internal static class Helper
    {
        public static CultureInfo en = new CultureInfo("en-US"); // This is important, no touchy

        public static List<Vector3> GetOutsideNodes() => GameObject.FindGameObjectsWithTag("OutsideAINode").Select(n => n.transform.position).ToList();

        public static List<Vector3> GetInsideAINodes() => GameObject.FindGameObjectsWithTag("AINode").Select(n => n.transform.position).ToList();
        public static List<Vector3> GetSpawnDenialNodes()
        {
            List<Vector3> nodes = GameObject.FindGameObjectsWithTag("SpawnDenialPoint").Select(n => n.transform.position).ToList();
            nodes.Add(GameObject.FindGameObjectWithTag("ItemShipLandingNode").transform.position);
            nodes.AddRange(GameObject.FindObjectsOfType<EntranceTeleport>().Select(i => i.gameObject.transform.position).ToList());
            return nodes;
        }

        public static int[] IntArray(this float[] Values)
        {
            int[] newValues = new int[Values.Length];
            for (int i = 0; i < Values.Length; i++)
            {
                newValues[i] = (int)Values[i];
            }
            return newValues;
        }

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

        public static WeatherEffect[] Add(this WeatherEffect[] toObjects, WeatherEffect newObject)
        {
            WeatherEffect[] newMapObjects = new WeatherEffect[toObjects.Length + 1];
            for (int i = 0; i < toObjects.Length; i++)
            {
                newMapObjects[i] = toObjects[i];
            }
            newMapObjects[toObjects.Length] = newObject;
            return newMapObjects;
        }

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

        public static string GetPercentage(float value) => (value * 100.0f).ToString("F1") + "%";

        public static string GetDifficultyColorHex(float difficulty, float cap) // (0, 255, 0) => (0, 127, 0) => (255, 0, 0) => (127, 0, 0) => (40, 0, 0) => (15, 0, 0)
        {
            if (cap < 1) cap = 1.0f;
            difficulty *= Configuration.difficultyMaxCap.Value / cap;

            if (difficulty < 0.0f) difficulty = 0.0f;

            float r = 0.0f, g = 0.0f;
            if(difficulty >= 0.0f && difficulty < 10.0f)
            {
                g = 1.0f;
            } else if (difficulty >= 10.0f && difficulty < 20.0f)
            {
                g = 1.5f - (difficulty * 0.05f);
            } else if(difficulty >= 20.0f && difficulty < 35.0f)
            {
                r = (difficulty * 0.067f) - 1.3334f;
                g = 1.16667f - (difficulty * 0.034f);
            } else if (difficulty >= 35.0f && difficulty < 60.0f)
            {
                r = 1.7f - (difficulty * 0.02f);
            } else if (difficulty >= 60.0f)
            {
                r = 1 - (difficulty * 0.0084f);
            }

            return ((int)Mathf.Clamp(r * 255, 15.0f, 255.0f)).ToString("X2") + ((int)Mathf.Clamp(g * 255, 0.0f, 255.0f)).ToString("X2") + "00";
        }

        private static float InBetween(float min, float max, float at) => (at * (max - min)) + min;

        public static string GetDifficultyText(float difficulty)
        {
            string difficultyText = "Easy";
            if (difficulty >= 15.0f && difficulty < 30.0f)
            {
                difficultyText = "Medium";
            }
            else if (difficulty >= 30.0f && difficulty < 50.0f)
            {
                difficultyText = "Hard";
            }
            else if(difficulty >= 50.0f && difficulty < 75.0f)
            {
                difficultyText = "Very hard";
            } 
            else if(difficulty >= 75.0f)
            {
                difficultyText = "Insane";
            }
            return difficultyText;
        }

        public static Vector3 GetSafePosition(List<Vector3> nodes, List<Vector3> denialNodes, float radius, int seed)
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

        public static Scale getScale(string from)
        {
            float[] values = ParseValuesFromString(from);
            return new Scale(values[0], values[1], values[2], values[3]);
        }

        public static string GetStringFromScale(Scale from) => $"{from.Base.ToString(en)}, {from.Increment.ToString(en)}, {from.MinCap.ToString(en)}, {from.MaxCap.ToString(en)}";

        public static float[] ParseValuesFromString(string from)
        {
            return from.Split(',').Select(x => float.Parse(x, en)).ToArray();
        }

        public static string StringsToList(List<string> strings, string seperator) // This is confusing naming
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

        public static List<string> ListToStrings(string text)
        {
            if (text.IsNullOrWhiteSpace()) return new List<string>();
            text = text.Replace(" ", "");
            return text.Split(',').ToList();
        }

        public static List<string> ListToDescriptions(string text)
        {
            if (text.IsNullOrWhiteSpace()) return new List<string>() { "" };
            return text.Split("|").ToList();
        }

        public static IList<Vector2> ComputeConvexHull(List<Vector2> points, bool sortInPlace = false) // Taken from https://gist.github.com/dLopreiato/7fd142d0b9728518552188794b8a750c
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

        public class Generics<T>
        {

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
