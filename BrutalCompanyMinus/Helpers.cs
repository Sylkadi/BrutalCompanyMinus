using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;

namespace BrutalCompanyMinus
{
    internal static class Helper
    {
        public static List<Vector3> GetOutsideNodes() => GameObject.FindGameObjectsWithTag("OutsideAINode").Select(n => n.transform.position).ToList();
        public static List<Vector3> GetSpawnDenialNodes()
        {
            List<Vector3> nodes = GameObject.FindGameObjectsWithTag("SpawnDenialPoint").Select(n => n.transform.position).ToList();
            nodes.Add(GameObject.FindGameObjectWithTag("ItemShipLandingNode").transform.position);

            switch (RoundManager.Instance.currentLevel.name) // Custom denial points so spawned objects dont block something
            {
                case "ExperimentationLevel":
                    nodes.Add(new Vector3(-72, 0, -100));
                    nodes.Add(new Vector3(-72, 0, -45));
                    nodes.Add(new Vector3(-72, 0, 15));
                    nodes.Add(new Vector3(-72, 0, 75));
                    nodes.Add(new Vector3(-30, 2, -30));
                    nodes.Add(new Vector3(-20, -2, 75));
                    break;
                case "AssuranceLevel":
                    nodes.Add(new Vector3(63, -2, -43));
                    nodes.Add(new Vector3(120, -1, 75));
                    break;
                case "OffenseLevel":
                    nodes.Add(new Vector3(120, 10, -65));
                    break;
                case "DineLevel":
                    nodes.Add(new Vector3(-40, 0, 80));
                    break;
                case "TitanLevel":
                    nodes.Add(new Vector3(-16, -3, 5));
                    nodes.Add(new Vector3(-50, 20, -30));
                    break;
            }

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
