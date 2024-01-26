using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BrutalCompanyMinus
{
    public class Assets
    {
        internal static AssetBundle bundle;

        public enum EnemyName
        {
            Bracken, HoardingBug, CoilHead, Thumper, BunkerSpider, Jester, SnareFlea, Hygrodere, GhostGirl, SporeLizard, NutCracker, Masked, EyelessDog, ForestKeeper, EarthLeviathan, BaboonHawk, RoamingLocust, Manticoil, CircuitBee, Lasso
        }
        private static Dictionary<EnemyName, string> EnemyNameList = new Dictionary<EnemyName, string>() { 
            { EnemyName.SnareFlea, "Centipede" }, { EnemyName.BunkerSpider, "SandSpider" }, { EnemyName.HoardingBug, "HoarderBug" }, { EnemyName.Bracken, "Flowerman" }, { EnemyName.Thumper, "Crawler" },
            { EnemyName.Hygrodere, "Blob" }, { EnemyName.GhostGirl, "DressGirl" }, { EnemyName.SporeLizard, "Puffer" }, { EnemyName.NutCracker, "Nutcracker" }, { EnemyName.EyelessDog, "MouthDog" },
            { EnemyName.ForestKeeper, "ForestGiant" }, { EnemyName.EarthLeviathan, "SandWorm" }, { EnemyName.CircuitBee, "RedLocustBees" }, { EnemyName.BaboonHawk, "BaboonHawk" }, { EnemyName.CoilHead, "SpringMan" },
            { EnemyName.Jester, "Jester" }, { EnemyName.Lasso, "LassoMan" }, { EnemyName.Masked, "MaskedPlayerEnemy" }, { EnemyName.Manticoil, "Doublewing" }, { EnemyName.RoamingLocust, "DocileLocustBees" }
        };

        public enum ItemName
        {
            LargeAxle, V_TypeEngine, PlasticFish, MetalSheet, LaserPointer, BigBolt, Bottles, Ring, SteeringWheel, CookieMoldPan, EggBeater, JarOfPickles, DustPan, AirHorn, ClownHorn, CashRegister, Candy, GoldBar, YieldSign, HomemadeFlashbang, Gift, Flask, ToyCube, Remote, ToyRobot, MagnifyingGlass, StopSign, TeaKettle, Mug, RedSoda, OldPhone, HairDryer, Brush, Bell, WhoopieCushion, Comedy, Tragedy, RubberDucky, ChemicalJug, FancyLamp, GoldenCup, Painting, Toothpaste, PillBottle, PerfumeBottle, Teeth, Magic7Ball
        }
        private static Dictionary<ItemName, string> ItemNameList = new Dictionary<ItemName, string>() {
            { ItemName.LargeAxle, "Cog1" }, { ItemName.V_TypeEngine, "EnginePart1"}, { ItemName.PlasticFish, "FishTestProp" }, { ItemName.MetalSheet, "MetalSheet" }, { ItemName.LaserPointer, "FlashLaserPointer" },
            { ItemName.BigBolt, "BigBolt" }, { ItemName.Bottles, "BottleBin" }, { ItemName.Ring, "Ring" }, { ItemName.SteeringWheel, "SteeringWheel" }, { ItemName.CookieMoldPan, "MoldPan" },
            { ItemName.EggBeater, "EggBeater" }, { ItemName.JarOfPickles, "PickleJar" }, { ItemName.DustPan, "DustPan" }, { ItemName.AirHorn, "Airhorn" }, { ItemName.ClownHorn, "ClownHorn" },
            { ItemName.CashRegister, "CashRegister" }, { ItemName.Candy, "Candy" }, { ItemName.GoldBar, "GoldBar" }, { ItemName.YieldSign, "YieldSign" }, { ItemName.HomemadeFlashbang, "DiyFlashbang" },
            { ItemName.Gift, "GiftBox" }, { ItemName.Flask, "Flask" }, { ItemName.ToyCube, "ToyCube" }, { ItemName.Remote, "Remote" }, { ItemName.ToyRobot, "RobotToy" },
            { ItemName.MagnifyingGlass, "MagnifyingGlass" }, { ItemName.StopSign, "StopSign" }, { ItemName.TeaKettle, "TeaKettle" }, { ItemName.Mug, "Mug" }, { ItemName.RedSoda, "SodaCanRed" },
            { ItemName.OldPhone, "Phone" }, { ItemName.HairDryer, "Hairdryer" }, { ItemName.Brush, "Brush" }, { ItemName.Bell, "Bell" }, { ItemName.WhoopieCushion, "WhoopieCushion" },
            { ItemName.Comedy, "ComedyMask" }, { ItemName.Tragedy, "TragedyMask" }, { ItemName.RubberDucky, "RubberDuck" }, { ItemName.ChemicalJug, "ChemicalJug" }, { ItemName.FancyLamp, "FancyLamp" },
            { ItemName.Painting, "FancyPainting" }, { ItemName.GoldenCup, "FancyCup" }, { ItemName.Toothpaste, "Toothpaste" }, { ItemName.PillBottle, "PillBottle" }, { ItemName.PerfumeBottle, "PerfumeBottle" },
            { ItemName.Teeth, "Dentures" }, { ItemName.Magic7Ball, "7Ball" }
        };

        public enum ObjectName
        {
            LargeRock1, LargeRock2, LargeRock3, LargeRock4, TreeLeaflessBrown1, GiantPumkin, GreyRockGrouping2, GreyRockGrouping4, Tree, TreeLeafless2, TreeLeafless3, Landmine, Turret
        }
        private static Dictionary<ObjectName, string> ObjectNameList = new Dictionary<ObjectName, string>() {
            { ObjectName.LargeRock1, "LargeRock1" }, { ObjectName.LargeRock2, "LargeRock2" }, { ObjectName.LargeRock3, "LargeRock3" }, { ObjectName.LargeRock4, "LargeRock4" }, { ObjectName.GreyRockGrouping2, "GreyRockGrouping2" },
            { ObjectName.GreyRockGrouping4, "GreyRockGrouping4" }, { ObjectName.GiantPumkin, "GiantPumpkin" }, { ObjectName.Tree, "tree" }, { ObjectName.TreeLeaflessBrown1, "treeLeaflessBrown.001 Variant" }, { ObjectName.TreeLeafless2, "treeLeafless.002_LOD0" },
            { ObjectName.TreeLeafless3, "treeLeafless.003_LOD0" }, { ObjectName.Landmine, "Landmine" }, { ObjectName.Turret, "TurretContainer" }
        };

        private static Dictionary<string, EnemyType> EnemyList = new Dictionary<string, EnemyType>();
        private static Dictionary<string, Item> ItemList = new Dictionary<string, Item>();
        private static Dictionary<string, GameObject> ObjectList = new Dictionary<string, GameObject>();

        internal static List<float> factorySizeMultiplierList = new List<float>();

        internal static void Load()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BrutalCompanyMinus.Asset.asset"))
            {
                bundle = AssetBundle.LoadFromStream(stream);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static bool generatedList = false;
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(!generatedList && StartOfRound.Instance != null)
            {
                // Generate Enemy List
                Log.LogInfo("Generating 'EnemyList'");

                EnemyType[] AllEnemies = Resources.FindObjectsOfTypeAll<EnemyType>().Concat(GameObject.FindObjectsByType<EnemyType>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID)).ToArray();
                Array.Reverse(AllEnemies); // Reverse(Important)
                AllEnemies = AllEnemies.GroupBy(x => x.name).Select(x => x.FirstOrDefault()).ToArray(); // Remove duplicates
                
                for(int i = 0; i < AllEnemies.Length; i++)
                {
                    if (AllEnemies[i].enemyPrefab == null) Log.LogWarning(string.Format("Enemy:{0}, prefab is null, this may cause issues...", AllEnemies[i].name));
                    EnemyList.Add(AllEnemies[i].name, AllEnemies[i]);
                }

                // Check list
                foreach(KeyValuePair<string, EnemyType> e in EnemyList)
                {
                    bool existsInList = false;
                    foreach(KeyValuePair<EnemyName, string> n in EnemyNameList)
                    {
                        if (e.Key == n.Value) existsInList = true;
                    }
                    if (!existsInList) Log.LogWarning(string.Format("Enemy:'{0}', isn't matched with enum, this may cause issues...", e.Key));
                }

                Log.LogInfo(string.Format("Finished generating 'EnemyList', Count:{0}", EnemyList.Count));



                // Generate Item List
                Log.LogInfo("Generating 'ItemList'");

                Item[] AllItems = Resources.FindObjectsOfTypeAll<Item>().Concat(GameObject.FindObjectsByType<Item>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID)).ToArray();
                Array.Reverse(AllItems); // Reverse(Important)
                AllItems = AllItems.GroupBy(x => x.name).Select(x => x.FirstOrDefault()).ToArray(); // Remove duplicates

                for(int i = 0; i < AllItems.Length; i++)
                {
                    if (AllItems[i].spawnPrefab == null) Log.LogWarning(string.Format("Item:{0}, prefab is null, this may cause issues...", AllItems[i].name));
                    ItemList.Add(AllItems[i].name, AllItems[i]);
                }

                // Check list
                foreach (KeyValuePair<string, Item> i in ItemList)
                {
                    bool existsInList = false;
                    foreach (KeyValuePair<ItemName, string> n in ItemNameList)
                    {
                        if (i.Key == n.Value) existsInList = true;
                    }
                    if (!existsInList) Log.LogWarning(string.Format("Item:'{0}', isn't matched with enum, this may cause issues...", i.Key));
                }

                Log.LogInfo(string.Format("Finished generating 'ItemList', Count:{0}", ItemList.Count));



                // Generate Object List
                Log.LogInfo("Generating 'ObjectList'");

                List<SpawnableMapObject> insideObjectList = new List<SpawnableMapObject>();
                List<SpawnableOutsideObjectWithRarity> outsideObjectList = new List<SpawnableOutsideObjectWithRarity>();

                foreach (SelectableLevel level in StartOfRound.Instance.levels)
                {
                    foreach (SpawnableMapObject obj in level.spawnableMapObjects)
                    {
                        if (insideObjectList.FindIndex(o => o.prefabToSpawn.name == obj.prefabToSpawn.name) < 0) // If dosent exist in list then add
                        {
                            insideObjectList.Add(obj);
                        }
                    }

                    foreach (SpawnableOutsideObjectWithRarity obj in level.spawnableOutsideObjects)
                    {
                        if (outsideObjectList.FindIndex(o => o.spawnableObject.prefabToSpawn.name == obj.spawnableObject.prefabToSpawn.name) < 0) // If dosent exist in list then add
                        {
                            outsideObjectList.Add(obj);
                        }
                    }
                }

                foreach (SpawnableMapObject obj in insideObjectList) ObjectList.Add(obj.prefabToSpawn.name, obj.prefabToSpawn);
                foreach (SpawnableOutsideObjectWithRarity obj in outsideObjectList) ObjectList.Add(obj.spawnableObject.prefabToSpawn.name, obj.spawnableObject.prefabToSpawn);

                // Check list
                foreach (KeyValuePair<string, GameObject> o in ObjectList)
                {
                    bool existsInList = false;
                    foreach (KeyValuePair<ObjectName, string> n in ObjectNameList)
                    {
                        if (o.Key == n.Value) existsInList = true;
                    }
                    if (!existsInList) Log.LogWarning(string.Format("Object:'{0}', isn't matched with enum, this may cause issues...", o.Key));
                }

                Log.LogInfo(string.Format("Finished generating 'ObjectList', Count:{0}", ObjectList.Count));


                // Generate FactorySize List
                foreach (SelectableLevel level in StartOfRound.Instance.levels) factorySizeMultiplierList.Add(level.factorySizeMultiplier);

                generatedList = true;
            }
        }

        public static EnemyType GetEnemy(EnemyName name) => EnemyList[EnemyNameList[name]];
        public static EnemyType GetEnemy(string name) => EnemyList[name];

        public static Item GetItem(ItemName name) => ItemList[ItemNameList[name]];
        public static Item GetItem(string name) => ItemList[name];

        public static GameObject GetObject(ObjectName name) => ObjectList[ObjectNameList[name]];
        public static GameObject GetObject(string name) => ObjectList[name];
    }
}
