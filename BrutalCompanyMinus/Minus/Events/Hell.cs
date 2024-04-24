using BrutalCompanyMinus;
using BrutalCompanyMinus.Minus.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering.HighDefinition;
using static BrutalCompanyMinus.Minus.MonoBehaviours.EnemySpawnCycle;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Hell : MEvent
    {
        public override string Name() => nameof(Hell);

        public static Hell Instance;

        public static SpawnCycle insideHellSpawnCycle, outsideHellSpawnCycle; // I need to make these configurable

        public static bool Active = false, SpawnCycle = false;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Great reward, but at what cost...", "This is the worst event of them all", "You are going to need jesus for this one", "Before crushing the life out of you, I will show you why my power is utterly beyond question!" };
            ColorHex = "#280000";
            Type = EventType.VeryBad;

            EventsToRemove = new List<string>() { nameof(Trees), nameof(LeaflessBrownTrees), nameof(Gloomy), nameof(Raining), nameof(HeavyRain), nameof(Warzone), nameof(EarlyShip), nameof(LateShip) };
            EventsToSpawnWith = new List<string>() { nameof(LeaflessTrees), nameof(FacilityGhost), nameof(Spiders), nameof(Thumpers), nameof(Landmines) };

            monsterEvents = new List<MonsterEvent>(){ new MonsterEvent(
                Assets.EnemyName.Bracken,
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(1.0f, 0.0f, 1.0f, 1.0f),
                new Scale(2.0f, 0.0f, 2.0f, 2.0f),
                new Scale(1.0f, 0.0f, 1.0f, 1.0f),
                new Scale(1.0f, 0.0f, 1.0f, 1.0f)), new MonsterEvent(
                Assets.antiCoilHead,
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(1.0f, 0.0f, 1.0f, 1.0f),
                new Scale(1.0f, 0.0f, 1.0f, 1.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f)), new MonsterEvent(
                Assets.EnemyName.CoilHead,
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(1.0f, 0.0f, 1.0f, 1.0f),
                new Scale(1.0f, 0.0f, 1.0f, 1.0f)), new MonsterEvent(
                Assets.kamikazieBug,
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(3.0f, 0.0f, 3.0f, 3.0f),
                new Scale(3.0f, 0.0f, 3.0f, 3.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f))
            };

            ScaleList.Add(ScaleType.ScrapValue, new Scale(1.75f, 0.0225f, 1.75f, 4.0f));
            ScaleList.Add(ScaleType.ScrapAmount, new Scale(1.75f, 0.0225f, 1.75f, 4.0f));
            ScaleList.Add(ScaleType.SpawnMultiplier, new Scale(1.0f, 0.02f, 1.0f, 3.0f));
            ScaleList.Add(ScaleType.FactorySize, new Scale(1.5f, 0.01f, 1.5f, 2.5f));
            
            insideHellSpawnCycle = new SpawnCycle()
            {
                nothingWeight = 100,
                spawnAttemptInterval = 30,
                spawnCycleDuration = 2000.0f,
                multiplyBySpawnCurve = false,
                enemies = new List<EnemySpawnInfo>()
                {
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.SnareFlea).enemyPrefab,
                        enemyWeight = 12,
                        spawnCap = 5,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.BunkerSpider).enemyPrefab,
                        enemyWeight = 18,
                        spawnCap = 5,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.HoardingBug).enemyPrefab,
                        enemyWeight = 12,
                        spawnCap = 5,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.Bracken).enemyPrefab,
                        enemyWeight = 35,
                        spawnCap = 10,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.Thumper).enemyPrefab,
                        enemyWeight = 24,
                        spawnCap = 5,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.Hygrodere).enemyPrefab,
                        enemyWeight = 2,
                        spawnCap = 5,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.GhostGirl).enemyPrefab,
                        enemyWeight = 60,
                        spawnCap = 20,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.NutCracker).enemyPrefab,
                        enemyWeight = 40,
                        spawnCap = 10,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.CoilHead).enemyPrefab,
                        enemyWeight = 30,
                        spawnCap = 10,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.Jester).enemyPrefab,
                        enemyWeight = 10,
                        spawnCap = 5,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.Lasso).enemyPrefab,
                        enemyWeight = 1,
                        spawnCap = 1,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.Masked).enemyPrefab,
                        enemyWeight = 25,
                        spawnCap = 10,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.Butler).enemyPrefab,
                        enemyWeight = 35,
                        spawnCap = 10,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.EyelessDog).enemyPrefab,
                        enemyWeight = 10,
                        spawnCap = 3,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.nutSlayer.enemyPrefab,
                        enemyWeight = 1,
                        spawnCap = 1,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.kamikazieBug.enemyPrefab,
                        enemyWeight = 12,
                        spawnCap = 5,
                        spawnLocation = SpawnLocation.Inside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.antiCoilHead.enemyPrefab,
                        enemyWeight = 15,
                        spawnCap = 10,
                        spawnLocation = SpawnLocation.Inside
                    },
                }
            };

            outsideHellSpawnCycle = new SpawnCycle()
            {
                nothingWeight = 100,
                spawnAttemptInterval = 30,
                spawnCycleDuration = 2000.0f,
                multiplyBySpawnCurve = false,
                enemies = new List<EnemySpawnInfo>()
                {
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.EyelessDog).enemyPrefab,
                        enemyWeight = 25,
                        spawnCap = 15,
                        spawnLocation = SpawnLocation.Outside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.ForestKeeper).enemyPrefab,
                        enemyWeight = 15,
                        spawnCap = 10,
                        spawnLocation = SpawnLocation.Outside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.EarthLeviathan).enemyPrefab,
                        enemyWeight = 5,
                        spawnCap = 5,
                        spawnLocation = SpawnLocation.Outside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.OldBird).enemyPrefab,
                        enemyWeight = 5,
                        spawnCap = 5,
                        spawnLocation = SpawnLocation.Outside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.BaboonHawk).enemyPrefab,
                        enemyWeight = 50,
                        spawnCap = 25,
                        spawnLocation = SpawnLocation.Outside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.Bracken).enemyPrefab,
                        enemyWeight = 15,
                        spawnCap = 5,
                        spawnLocation = SpawnLocation.Outside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.Jester).enemyPrefab,
                        enemyWeight = 3,
                        spawnCap = 1,
                        spawnLocation = SpawnLocation.Outside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.NutCracker).enemyPrefab,
                        enemyWeight = 10,
                        spawnCap = 5,
                        spawnLocation = SpawnLocation.Outside
                    },
                    new EnemySpawnInfo()
                    {
                        enemy = Assets.GetEnemy(Assets.EnemyName.BunkerSpider).enemyPrefab,
                        enemyWeight = 10,
                        spawnCap = 3,
                        spawnLocation = SpawnLocation.Outside
                    }

                }
            };

            foreach (EnemyType enemy in Assets.EnemyList.Values)
            {
                if (enemy == null || enemy.enemyPrefab == null || enemy.isDaytimeEnemy || insideHellSpawnCycle.enemies.Select(x => x.enemy.name).Contains(enemy.name)) continue;

                if (enemy.isOutsideEnemy)
                {
                    outsideHellSpawnCycle.enemies.Add(new EnemySpawnInfo() { enemy = enemy.enemyPrefab, enemyWeight = 8, spawnCap = 2, spawnLocation = SpawnLocation.Outside });
                }
                else
                {
                    insideHellSpawnCycle.enemies.Add(new EnemySpawnInfo() { enemy = enemy.enemyPrefab, enemyWeight = 8, spawnCap = 2, spawnLocation = SpawnLocation.Inside });
                }
            }
        }

        public override void Execute()
        {
            RoundManager.Instance.currentLevel.enemySpawnChanceThroughoutDay = new AnimationCurve(new Keyframe(0.0f, -100.0f), new Keyframe(1.0f, -100.0f));
            RoundManager.Instance.currentLevel.outsideEnemySpawnChanceThroughDay = new AnimationCurve(new Keyframe(0f, -100.0f), new Keyframe(1.0f, -100.0f));

            Manager.scrapValueMultiplier *= Getf(ScaleType.ScrapValue);
            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);
            Manager.factorySizeMultiplier *= Getf(ScaleType.FactorySize);

            insideHellSpawnCycle.nothingWeight = 100 / Getf(ScaleType.SpawnMultiplier);
            outsideHellSpawnCycle.nothingWeight = 100 / Getf(ScaleType.SpawnMultiplier);

            Net.Instance.MoveTimeServerRpc(860, 0.139534883721f);

            Manager.SetAtmosphere("bloodyrain", true);
            Manager.SetAtmosphere(Assets.AtmosphereNameList[Assets.AtmosphereName.Exclipsed], true);

            ExecuteAllMonsterEvents();

            Active = true;
            SpawnCycle = true;
        }

        public override void OnShipLeave() => Active = false;

        public override void OnGameStart() => Active = false;
    }
}
