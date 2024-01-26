using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using HarmonyLib;
using System.Collections;
using GameNetcodeStuff;
using TMPro;

namespace BrutalCompanyMinus.Minus
{
    [HarmonyPatch]
    public class Manager
    {

        public static int daysPassed = -1;
        public static int paycut = 0;

        internal static float terrainArea = 0.0f;
        internal static string terrainTag = "";
        internal static string terrainName = "";
        internal static List<Vector3> outsideObjectSpawnNodes = new List<Vector3>();
        internal static float outsideObjectSpawnRadius = 0.0f;

        internal static List<GameObject> objectsToClear = new List<GameObject>();

        internal static List<ObjectInfo> enemiesToSpawnInside = new List<ObjectInfo>();
        internal static List<ObjectInfo> enemiesToSpawnOutside = new List<ObjectInfo>();
        internal static List<ObjectInfo> insideObjectsToSpawnOutside = new List<ObjectInfo>();

        internal static float factorySizeMultiplier = 1f;
        internal static float scrapValueMultiplier = 0.4f;
        internal static float scrapAmountMultiplier = 1f;
        internal static int scrapMinAmount = 0;
        internal static int scrapMaxAmount = 0;

        internal static bool BountyActive = false;
        internal static bool DoorGlitchActive = false;

        internal static int randomItemsToSpawnOutsideCount = 0;

        public static class Spawn
        {
            internal static int randomSeedValue = 0;

            internal static void OutsideObjects(GameObject obj, Vector3 offset, float density, float radius = -1.0f)
            {
                if (obj == null) return;

                System.Random random = new System.Random(randomSeedValue);

                List<Vector3> spawnDenialPoints = Functions.GetSpawnDenialNodes();

                int count = (int)(density * terrainArea); // Compute amount
                for (int i = 0; i < count; i++)
                {
                    randomSeedValue++;

                    UnityEngine.Random.InitState(randomSeedValue); // Important or wont be same on all clients
                    Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
                    if(radius != -1.0f || outsideObjectSpawnNodes.Count == 0)
                    {
                        position = RoundManager.Instance.GetRandomNavMeshPositionInRadius(RoundManager.Instance.outsideAINodes[random.Next(0, RoundManager.Instance.outsideAINodes.Length)].transform.position, radius);
                    } else
                    {
                        position = RoundManager.Instance.GetRandomNavMeshPositionInRadius(outsideObjectSpawnNodes[random.Next(0, outsideObjectSpawnNodes.Count)], outsideObjectSpawnRadius);
                    }
                    Quaternion rotation = obj.transform.rotation;

                    RaycastHit info;
                    bool isInvalidPosition = false;
                    if (Physics.Raycast(new Ray(position, Vector3.down), out info))
                    {
                        if (info.collider.gameObject.tag != terrainTag && info.collider.gameObject.name != terrainName) // Did it hit terrain mesh? if not then position is not valid...
                        {
                            isInvalidPosition = true;
                        }
                    }
                    else // If didn't hit anything, position is is invalid
                    {
                        isInvalidPosition = true;
                    }
                    foreach (Vector3 spawnDenialPoint in spawnDenialPoints)
                    {
                        if (Vector3.Distance(position, spawnDenialPoint) <= 10.0f)
                        {
                            isInvalidPosition = true;
                        }
                    }

                    if (!isInvalidPosition)
                    {
                        position.y = info.point.y; // Match raycast hit y position

                        position += offset;
                        rotation.eulerAngles = rotation.eulerAngles + new Vector3(0.0f, random.Next(0, 360), 0.0f);

                        GameObject gameObject = UnityEngine.Object.Instantiate(obj, position, rotation);

                        NetworkObject netObject = gameObject.GetComponent<NetworkObject>();
                        if (netObject != null) gameObject.GetComponent<NetworkObject>().Spawn(true);

                        objectsToClear.Add(gameObject);
                    }
                }
            }

            /// <summary>
            /// Will spawn enemies outside safely.
            /// </summary>
            public static void OutsideEnemies(EnemyType enemy, int count) => enemiesToSpawnOutside.Add(new ObjectInfo(enemy.enemyPrefab, count));

            /// <summary>
            /// Will spawn enemies inside safely.
            /// </summary>
            public static void InsideEnemies(EnemyType enemy, int count, float radius = 0.0f) => enemiesToSpawnInside.Add(new ObjectInfo(enemy.enemyPrefab, count, 0.0f, radius));

            /// <summary>
            /// Will spawn outside scrap safely.
            /// </summary>
            public static void ScrapOutside(int Amount) => randomItemsToSpawnOutsideCount += Amount;

            internal static void DoSpawnOutsideEnemies()
            {
                List<Vector3> OutsideAiNodes = Functions.GetOutsideNodes();
                List<Vector3> SpawnDenialNodes = Functions.GetSpawnDenialNodes();

                // Spawn Outside enemies
                for (int i = 0; i < enemiesToSpawnOutside.Count; i++)
                {
                    for (int j = 0; j < enemiesToSpawnOutside[i].count; j++)
                    {
                        if (enemiesToSpawnOutside[i].obj == null)
                        {
                            Log.LogError("Enemy prefab on DoSpawnOutsideEnemies() is null, continuing.");
                            continue;
                        }
                        GameObject obj = UnityEngine.Object.Instantiate(
                            enemiesToSpawnOutside[i].obj,
                            Functions.GetSafePosition(OutsideAiNodes, SpawnDenialNodes, 20.0f),
                            Quaternion.Euler(Vector3.zero));

                        RoundManager.Instance.SpawnedEnemies.Add(obj.GetComponent<EnemyAI>());

                        obj.gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);
                    }
                }
                enemiesToSpawnOutside.Clear();
            }
            internal static void DoSpawnInsideEnemies()
            {
                // Spawn Inside enemies
                for (int i = 0; i < enemiesToSpawnInside.Count; i++)
                {
                    for (int j = 0; j < enemiesToSpawnInside[i].count; j++)
                    {
                        if (enemiesToSpawnInside[i].obj == null)
                        {
                            Log.LogError("Enemy prefab on DoSpawnInsideEnemies() is null, continuing.");
                            continue;
                        }
                        int index = UnityEngine.Random.Range(0, RoundManager.Instance.allEnemyVents.Length);
                        Vector3 position = RoundManager.Instance.allEnemyVents[index].floorNode.position;
                        position = RoundManager.Instance.GetRandomNavMeshPositionInRadius(position, enemiesToSpawnInside[i].radius, RoundManager.Instance.navHit);
                        Quaternion rotation = Quaternion.Euler(0.0f, RoundManager.Instance.allEnemyVents[index].floorNode.eulerAngles.y, 0.0f);
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(enemiesToSpawnInside[i].obj, position, rotation);

                        gameObject.GetComponentInChildren<NetworkObject>().Spawn(true);
                        EnemyAI component = gameObject.GetComponent<EnemyAI>();
                        RoundManager.Instance.SpawnedEnemies.Add(component);
                    }
                }
                enemiesToSpawnInside.Clear();
            }

            internal static void DoSpawnScrapOutside(int Amount = -1)
            {
                RoundManager r = RoundManager.Instance;
                System.Random rng = new System.Random();
                // Generate Scrap To Spawn
                int ScrapAmount = (int)(Amount * r.scrapAmountMultiplier);
                List<Item> ScrapToSpawn = new List<Item>();
                List<int> ScrapValues = new List<int>();
                List<int> ScrapWeights = new List<int>();
                for (int i = 0; i < r.currentLevel.spawnableScrap.Count; i++)
                {
                    if (i == r.increasedScrapSpawnRateIndex)
                    {
                        ScrapWeights.Add(i);
                    }
                    else
                    {
                        ScrapWeights.Add(r.currentLevel.spawnableScrap[i].rarity);
                    }
                }
                int[] weights = ScrapWeights.ToArray();
                for (int i = 0; i < ScrapAmount; i++)
                {
                    Item pickedScrap = r.currentLevel.spawnableScrap[r.GetRandomWeightedIndex(weights, rng)].spawnableItem;
                    ScrapToSpawn.Add(Assets.GetItem(pickedScrap.name)); // Get scrap safely
                }
                // Spawn Scrap
                List<NetworkObjectReference> ScrapSpawnsNet = new List<NetworkObjectReference>();
                List<Vector3> OutsideNodes = Functions.GetOutsideNodes();
                for (int i = 0; i < ScrapAmount; i++)
                {
                    if (ScrapToSpawn[i] == null)
                    {
                        Log.LogError("Found null element in list ScrapToSpawn. Skipping it.");
                        continue;
                    }
                    Vector3 position = r.GetRandomNavMeshPositionInBoxPredictable(OutsideNodes[UnityEngine.Random.Range(0, OutsideNodes.Count)], 10.0f, r.navHit, rng);
                    GameObject obj = UnityEngine.Object.Instantiate(ScrapToSpawn[i].spawnPrefab, position, Quaternion.identity, r.spawnedScrapContainer);
                    GrabbableObject grabbableObject = obj.GetComponent<GrabbableObject>();
                    grabbableObject.transform.rotation = Quaternion.Euler(grabbableObject.itemProperties.restingRotation);
                    grabbableObject.fallTime = 0.0f;
                    ScrapValues.Add((int)(UnityEngine.Random.Range(ScrapToSpawn[i].minValue, ScrapToSpawn[i].maxValue + 1) * r.scrapValueMultiplier));
                    grabbableObject.scrapValue = ScrapValues[ScrapValues.Count - 1];
                    NetworkObject netObj = obj.GetComponent<NetworkObject>();
                    netObj.Spawn();
                    ScrapSpawnsNet.Add(netObj);
                }
                r.StartCoroutine(waitForScrapToSpawnToSync(ScrapSpawnsNet.ToArray(), ScrapValues.ToArray()));
            }

            private static IEnumerator waitForScrapToSpawnToSync(NetworkObjectReference[] spawnedScrap, int[] scrapValues)
            {
                yield return new WaitForSeconds(10.0f);
                RoundManager.Instance.SyncScrapValuesClientRpc(spawnedScrap, scrapValues);
            }
        }

        public static void DeliverRandomItems(int Amount, int MaxPrice)
        {
            if (RoundManager.Instance.IsServer)
            {
                Terminal terminal = GameObject.FindObjectOfType<Terminal>();

                List<int> validItems = new List<int>();
                for (int i = 0; i < terminal.buyableItemsList.Length; i++)
                {
                    if (terminal.buyableItemsList[i].creditsWorth <= MaxPrice) validItems.Add(i);
                }

                for (int i = 0; i < Amount; i++)
                {
                    int item = validItems[UnityEngine.Random.Range(0, validItems.Count)];
                    terminal.orderedItemsFromTerminal.Add(item);
                }
            }
        }

        internal static void SampleMap()
        {
            // Compute Map Area
            List<Vector2> OuterPoints = new List<Vector2>();
            foreach (GameObject outsideAiNode in RoundManager.Instance.outsideAINodes)
            {
                OuterPoints.Add(new Vector2(outsideAiNode.transform.position.x, outsideAiNode.transform.position.z));
            }
            OuterPoints = Functions.ComputeConvexHull(OuterPoints).ToList();

            // Get outside spawn nodes
            if(OuterPoints.Count > 0)
            {
                float xSum = 0.0f, ySum = 0.0f;
                foreach (Vector2 outerPoint in OuterPoints)
                {
                    xSum += outerPoint.x;
                    ySum += outerPoint.y;
                }
                Vector2 CentrePoint = new Vector2(xSum / OuterPoints.Count, ySum / OuterPoints.Count);

                foreach (Vector2 outerPoint in OuterPoints)
                {
                    Vector2 innerPoint = (outerPoint + CentrePoint) * 0.5f;

                    outsideObjectSpawnNodes.Add(new Vector3(innerPoint.x, 100.0f, innerPoint.y));
                }
                outsideObjectSpawnRadius = Vector2.Distance(CentrePoint, OuterPoints[0]) + 75.0f;
            }


            float Area = 0.0f;
            for (int i = 0; i != OuterPoints.Count - 1; i++)
            {
                Vector2 from = OuterPoints[i], to = OuterPoints[i + 1];

                float averageHeight = (from.y + to.y) * 0.5f;
                float width = from.x - to.x;

                Area += averageHeight * width;
            }
            if (Area < 0.0f) Area *= -1.0f;
            terrainArea = Area;

            // Get terrainTag and terrainName
            List<Vector3> nodes = Functions.GetOutsideNodes();
            List<RaycastHit> hits = new List<RaycastHit>();
            for (int i = 0; i != nodes.Count * 10; i++) // 10 samples per node
            {
                Vector3 node = nodes[i % nodes.Count];
                Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * 3.0f;
                RaycastHit hit;
                if (Physics.Raycast(new Ray(node + new Vector3(randomPoint.x, 10.0f, randomPoint.y), Vector3.down), out hit))
                {
                    hits.Add(hit);
                }
            }

            terrainTag = Functions.MostCommon(hits.Select(x => x.collider.gameObject.tag).ToList());
            terrainName = Functions.MostCommon(hits.Select(x => x.collider.gameObject.name).ToList());
        }

        public static SpawnableItemWithRarity generateItemWithRarity(Item item, int rarity)
        {

            SpawnableItemWithRarity spawnableItemWithRarity = new SpawnableItemWithRarity();
            spawnableItemWithRarity.spawnableItem = item;
            spawnableItemWithRarity.rarity = rarity;
            if (item.spawnPrefab == null)
            {
                Log.LogError("Item prefab on generateItemWithRarity() is null, setting rarity to 0");
                spawnableItemWithRarity.rarity = 0;
            }
            return spawnableItemWithRarity;
        }

        public static void AddEnemyToPoolWithRarity(ref List<SpawnableEnemyWithRarity> list, EnemyType enemy, int rarity)
        {
            if (enemy.enemyPrefab == null)
            {
                Log.LogError("Enemy prefab is null on AddEnemyToPoolWithRarity(), returning.");
                return;
            }
            SpawnableEnemyWithRarity spawnableEnemyWithRarity = new SpawnableEnemyWithRarity();
            spawnableEnemyWithRarity.enemyType = enemy;
            spawnableEnemyWithRarity.rarity = rarity;
            list.Add(spawnableEnemyWithRarity);
        }

        public static void RemoveSpawn(string Name)
        {
            bool removedEnemy = false;
            int index = RoundManager.Instance.currentLevel.Enemies.FindIndex(x => x.enemyType.name.ToUpper() == Name.ToUpper());
            if (index != -1)
            {
                RoundManager.Instance.currentLevel.Enemies.RemoveAt(index);
                removedEnemy = true;
            }
            index = RoundManager.Instance.currentLevel.OutsideEnemies.FindIndex(x => x.enemyType.name.ToUpper() == Name.ToUpper());
            if (index != -1)
            {
                RoundManager.Instance.currentLevel.OutsideEnemies.RemoveAt(index);
                removedEnemy = true;
            }
            index = RoundManager.Instance.currentLevel.DaytimeEnemies.FindIndex(x => x.enemyType.name.ToUpper() == Name.ToUpper());
            if (index != -1)
            {
                RoundManager.Instance.currentLevel.DaytimeEnemies.RemoveAt(index);
                removedEnemy = true;
            }
            if (!removedEnemy) Log.LogInfo(string.Format("Failed to remove '{0}' from enemy pool, either it dosen't exist on the map or wrong string used.", Name));
        }

        internal static MEvent RandomWeightedEvent(List<MEvent> events)
        {
            int WeightedSum = 0;
            foreach (MEvent e in events) WeightedSum += e.Weight;

            foreach (MEvent e in events)
            {
                if (UnityEngine.Random.Range(0, WeightedSum) < e.Weight)
                {
                    return e;
                }
                WeightedSum -= e.Weight;
            }

            return events[events.Count - 1];
        }

        internal static List<MEvent> ChooseEvents(SelectableLevel newLevel, List<MEvent> events, out List<MEvent> additionalEvents)
        {
            List<MEvent> chosenEvents = new List<MEvent>();
            List<MEvent> eventsToChooseForm = new List<MEvent>();
            foreach (MEvent e in events) eventsToChooseForm.Add(e);

            for (int i = 0; i < Configuration.eventsToSpawn.Value; i++)
            {
                MEvent newEvent = RandomWeightedEvent(eventsToChooseForm);
                chosenEvents.Add(newEvent);

                if(!newEvent.AddEventIfOnly()) // If event condition is false, remove event from eventsToChoosefrom and iterate again
                {
                    i--;
                    eventsToChooseForm.RemoveAll(x => x.Name() == newEvent.Name());
                    continue;
                }

                // Remove so no further accurrences
                eventsToChooseForm.RemoveAll(x => x.Name() == newEvent.Name());

                // Remove incompatible events from toChooseList
                int AmountRemoved = 0;

                foreach (string eventToRemove in newEvent.EventsToRemove)
                {
                    eventsToChooseForm.RemoveAll(x => x.Name() == eventToRemove);
                    AmountRemoved += chosenEvents.RemoveAll(x => x.Name() == eventToRemove);
                }

                foreach (string eventToSpawnWith in newEvent.EventsToSpawnWith)
                {
                    eventsToChooseForm.RemoveAll(x => x.Name() == eventToSpawnWith);
                    AmountRemoved += chosenEvents.RemoveAll(x => x.Name() == eventToSpawnWith);
                }

                i -= AmountRemoved; // Decrement each time an event is removed from chosenEvents list
            }

            // Generate eventsToSpawnWith list with no copies
            List<MEvent> eventsToSpawnWith = new List<MEvent>();
            for (int i = 0; i < chosenEvents.Count; i++)
            {
                foreach (string eventToSpawnWith in chosenEvents[i].EventsToSpawnWith)
                {
                    int index = eventsToSpawnWith.FindIndex(x => x.Name() == eventToSpawnWith);
                    if (index == -1) eventsToSpawnWith.Add(MEvent.GetEvent(eventToSpawnWith)); // If dosen't exist in list, add.
                }
            }

            // Remove incompatible events
            for (int i = 0; i < eventsToSpawnWith.Count; i++)
            {
                foreach (string eventToRemove in eventsToSpawnWith[i].EventsToRemove)
                {
                    eventsToSpawnWith.RemoveAll(x => x.Name() == eventToRemove);
                }
            }

            // Remove disabledEvents from EventsToSpawnWith List
            foreach (MEvent e in Plugin.disabledEvents)
            {
                int index = eventsToSpawnWith.FindIndex(x => x.Name() == e.Name());
                if (index != -1) eventsToSpawnWith.RemoveAt(index);
            }

            additionalEvents = eventsToSpawnWith;
            return chosenEvents;
        }

        internal static void ApplyEvents(List<MEvent> currentEvents)
        {
            foreach (MEvent e in currentEvents) e.Execute();
        }

        internal static void UpdateAllEventWeights()
        {
            float eventTypeWeightSum = Configuration.veryGoodWeight.Value + Configuration.goodWeight.Value + Configuration.neutralWeight.Value + Configuration.badWeight.Value + Configuration.veryBadWeight.Value + Configuration.removeEnemyWeight.Value;

            float veryGoodProbability = Configuration.veryGoodWeight.Value / eventTypeWeightSum;
            float goodProbablity = Configuration.goodWeight.Value / eventTypeWeightSum;
            float neutralProbability = Configuration.neutralWeight.Value / eventTypeWeightSum;
            float removeEnemyProbability = Configuration.removeEnemyWeight.Value / eventTypeWeightSum;
            float badProbability = Configuration.badWeight.Value / eventTypeWeightSum;
            float veryBadProbability = Configuration.veryBadWeight.Value / eventTypeWeightSum;


            // Update all weights on events
            // CurrentSplit: VeryGood = 5%, Good = 18%, Neutral = 12%, Remove = 15%, Bad = 35%, VeryBad = 15%
            int VeryGoodCount = 0, GoodCount = 0, NeutralCount = 0, RemoveCount = 0, BadCount = 0, VeryBadCount = 0, Sum = 0;
            foreach (MEvent e in Plugin.events)
            {
                switch (e.Type)
                {
                    case MEvent.EventType.VeryGood:
                        VeryGoodCount++;
                        break;
                    case MEvent.EventType.Good:
                        GoodCount++;
                        break;
                    case MEvent.EventType.Neutral:
                        NeutralCount++;
                        break;
                    case MEvent.EventType.Remove:
                        RemoveCount++;
                        break;
                    case MEvent.EventType.Bad:
                        BadCount++;
                        break;
                    case MEvent.EventType.VeryBad:
                        VeryBadCount++;
                        break;
                }
            }

            Sum = VeryBadCount + GoodCount + NeutralCount + RemoveCount + BadCount + VeryBadCount;

            float VeryGoodWeight = (Sum / VeryGoodCount) * veryGoodProbability, GoodWeight = (Sum / GoodCount) * goodProbablity, NeutralWeight = (Sum / NeutralCount) * neutralProbability,
                  RemoveWeight = (Sum / RemoveCount) * removeEnemyProbability, BadWeight = (Sum / BadCount) * badProbability, VeryBadWeight = (Sum / VeryBadCount) * veryBadProbability;

            foreach (MEvent e in Plugin.events)
            {
                switch (e.Type)
                {
                    case MEvent.EventType.VeryGood:
                        e.Weight = (int)(VeryGoodWeight * 1000f);
                        break;
                    case MEvent.EventType.Good:
                        e.Weight = (int)(GoodWeight * 1000f);
                        break;
                    case MEvent.EventType.Neutral:
                        e.Weight = (int)(NeutralWeight * 1000f);
                        break;
                    case MEvent.EventType.Remove:
                        e.Weight = (int)(RemoveWeight * 1000f);
                        break;
                    case MEvent.EventType.Bad:
                        e.Weight = (int)(BadWeight * 1000f);
                        break;
                    case MEvent.EventType.VeryBad:
                        e.Weight = (int)(VeryBadWeight * 1000f);
                        break;
                }
                switch (e.Name())
                {
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RoundManager), "FinishGeneratingLevel")]
        private static void ObjectSpawnHandling()
        {
            SampleMap();

            // Client side objects
            Spawn.randomSeedValue = StartOfRound.Instance.randomMapSeed + 2; // Reset seed value
            RoundManager.Instance.StartCoroutine(DelayedExecution());

            // Net objects
            foreach (ObjectInfo obj in insideObjectsToSpawnOutside) Spawn.OutsideObjects(obj.obj, new Vector3(0.0f, -0.05f, 0.0f), obj.density);
        }

        private static IEnumerator DelayedExecution() // Delay this to fix trees not spawning in correctly on clients
        {
            yield return new WaitForSeconds(5.0f);
            foreach (OutsideObjectsToSpawn obj in Net.Instance.outsideObjectsToSpawn)
            {
                Spawn.OutsideObjects(Assets.GetObject((Assets.ObjectName)obj.objectEnumID), new Vector3(0.0f, -1.0f, 0.0f), obj.density);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RoundManager), "RefreshEnemyVents")]
        private static void OnRefreshEnemyVents()
        {
            if (RoundManager.Instance.allEnemyVents.Length != 0)
            {
                Spawn.DoSpawnInsideEnemies();
                Spawn.DoSpawnOutsideEnemies();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RoundManager), "Update")]
        private static void OnUpdate()
        {
            if (paycut > 0)
            {
                Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();

                terminal.groupCredits += paycut;
                terminal.SyncGroupCreditsServerRpc(terminal.groupCredits, terminal.numberOfItemsInDropship);

                HUDManager.Instance.AddTextToChatOnServer("<color=green>+" + paycut + "$</color>");

                paycut = 0;
            }
        }

        // Set days passed
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "Start")]
        private static void SetDaysPassed(ref string ___currentSaveFileName) => daysPassed = ES3.Load("Stats_DaysSpent", ___currentSaveFileName, 0) - 1;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SaveFileUISlot), nameof(SaveFileUISlot.SetFileToThis))]
        private static void _SetDaysPassed(ref string ___fileString) => daysPassed = ES3.Load("Stats_DaysSpent", ___fileString, 0) - 1;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartOfRound), "OnDisable")]
        private static void __SetDaysPassed() => daysPassed = -1;

        internal struct ObjectInfo
        {
            public int count;
            public float radius;
            public float density;
            public GameObject obj;

            public ObjectInfo(GameObject obj, int count)
            {
                this.obj = obj;
                this.count = count;
                radius = 0.0f;
                density = 0.0f;
            }

            public ObjectInfo(GameObject obj, float density)
            {
                this.obj = obj;
                this.density = density;
                radius = 0.0f;
                count = 0;
            }

            public ObjectInfo(GameObject obj, float density, float radius)
            {
                this.obj = obj;
                this.density = density;
                this.radius = radius;
                count = 0;
            }

            public ObjectInfo(GameObject obj, int count, float density, float radius)
            {
                this.obj = obj;
                this.density = density;
                this.radius = radius;
                this.count = count;
            }
        }
    }

    
    // Mostly used for sample map
    internal static class Functions
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

        public static Vector3 GetSafePosition(List<Vector3> nodes, List<Vector3> denialNodes, float radius)
        {
            Vector3 position = nodes[UnityEngine.Random.Range(0, nodes.Count)];
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
                if (Iteration > 1000)
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