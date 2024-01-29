using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Buildings;
using System.Collections.Generic;
using static Assets.Scripts.Services.MyConfig;
using UnityEngine;
using Assets.Scripts.Common;
using Assets.Scripts.Entities.Level;
using System.Security.Cryptography;
using Assets.Scripts.Entities.HomeMessage;
using Assets.Scripts.HomeLogic;

namespace Assets.Scripts.Services.Database
{
    /// <summary>
    /// 数据库初始化配置时的帮助类
    /// </summary>
    internal static class GameConfigInitHelper
    {
        public static List<CombatSkill> GetConfigCombatSkills()
        {
            return ResourceManager.Load<SkillListConfig>("SkillList").CombatSkills;
        }
        public static Dictionary<string, Building> GetConfigBuildings()
        {
            var d3b3 = new Vector2Int(3, 3);
            var d2b2 = new Vector2Int(2, 2);
            var res = new Dictionary<string, Building>();
            res.Add("e1",
                new ResourceBuilding
                {
                    BuildingId = "e1",
                    Name = "发电厂",
                    Description = "测试电力建筑+10",
                    ModelUrl = "cooling-tower Variant",
                    Dimensions = d3b3,
                    Produces = new Produce[] { new Produce { ItemId = ItemTable.Electric.ToString(), Amount = 10 } },
                    Costs = new Produce[] { new Produce { ItemId = ItemTable.Iron.ToString(), Amount = 100 } }

                }
            );

            res.Add("iaa1",
                new ResourceBuilding
                {
                    BuildingId = "iaa1",
                    Name = "冶炼厂",
                    Description = "测试建筑，生产基本资源+10",
                    ModelUrl = "industry-factory Variant",
                    Dimensions = d3b3,
                    Produces = new Produce[] {
                        new Produce { ItemId = ItemTable.Iron.ToString(), Amount = 10 },
                        new Produce { ItemId = ItemTable.Ammo.ToString(), Amount = 10 },
                        new Produce { ItemId = ItemTable.Al.ToString(), Amount = 10 } },
                    Costs = new Produce[] { new Produce { ItemId = ItemTable.Iron.ToString(), Amount = 100 } }
                }
            );
            res.Add("s1", new ResourceBuilding
            {
                BuildingId = "s1",
                Name = "购物广场",
                Description = "测试建筑，生产抽卡资源+10",
                ModelUrl = "skyscraper-part-bottom Variant",
                Dimensions = d3b3,
                Produces = new Produce[] {
                    new Produce { ItemId = ItemTable.Red.ToString(), Amount = 10 } },
                Costs = new Produce[] {
                    new Produce { ItemId = ItemTable.Electric.ToString(), Amount = 100 },
                    new Produce { ItemId = ItemTable.Iron.ToString(), Amount = 100 } }
            });
            res.Add("h1", new ResourceBuilding
            {
                BuildingId = "h1",
                Name = "住宅",
                Description = "测试建筑，恢复角色体力+2",
                ModelUrl = "House_1Room_Blue Variant",
                Dimensions = d2b2,
                Produces = new Produce[] {
                    new Produce { ItemId = ItemTable.PowerRecover.ToString(), Amount = 2 } },
                Costs = new Produce[] { new Produce { ItemId = ItemTable.Iron.ToString(), Amount = 50 } }
            });

            res.Add("mgTower1", new CombatBuilding
            {
                BuildingId = "mgTower1",
                Name = "机枪塔",
                Description = "测试建筑，攻击敌人",
                ModelUrl = "MachineGunTower_0 Variant",
                Dimensions = d2b2,
                Costs = new Produce[] { new Produce { ItemId = ItemTable.Iron.ToString(), Amount = 50 } },
                Hp = 30,
                WeaponId = 4,
                Range = 10,
            });
            res.Add("cmd", new CombatBuilding
            {
                BuildingId = "cmd",
                Name = "指挥部",
                Description = "测试建筑",
                ModelUrl = "Airport_Building Variant",
                Dimensions = new Vector2Int(12, 6),
                CanBuild = false,
                Hp = 50,
                WeaponId = -1,
                Range = -1,
            });

            return res;
        }

        public static List<LevelRule> GetConfigLevels()
        {
            // 演习作战（聚集分布敌人，进攻性强）、攻城作战（互动单位，中范围聚集分布敌人，进攻性弱）、三角定位（互动单位，和随机位置分布敌人，进攻性弱）
            return new List<LevelRule>()
            {
                new CombatLevelRule
                {
                    LevelName = "演习作战",
                    Description = "达到击杀数就是胜利！可能会出现8-10个水平相当的敌人",
                    ImageUrl = "Textures/combatLevel-1",
                    MapType = MapType.Middle,
                    TeamSpawn = new RectInt(5, 5, 5, 5),
                    EnemySpawn = new RectInt(25, 25, 5, 5),
                    OperatorPrefabs = new OperatorPrefab[]
                    {
                        new OperatorPrefab
                        {
                            OpInfo = getRandomDD(),
                            MinAmount = 2,
                            MaxAmount = 4,
                            UseRandomCModel = true,
                            MechaRandomUpgradeFactor = 0,
                            AiAgressive = true,
                            InitPosition = InitPosition.EnemySpawnScatter,
                            Dropouts = new Dropout[]{
                                new Dropout(ItemHelper.GetItem(ItemTable.Sphere.ToString()))
                            }
                        },
                        new OperatorPrefab
                        {
                            OpInfo = getRandomCA(),
                            MinAmount = 3,
                            MaxAmount = 5,
                            UseRandomCModel = true,
                            MechaRandomUpgradeFactor = 0,
                            AiAgressive = true,
                            InitPosition = InitPosition.EnemySpawnScatter,
                            Dropouts = new Dropout[]{
                                new Dropout(ItemHelper.GetItem(ItemTable.Sphere.ToString()))
                            }
                        },
                        new OperatorPrefab
                        {
                            OpInfo = getRandomCV(),
                            MinAmount = 1,
                            MaxAmount = 2,
                            UseRandomCModel = true,
                            MechaRandomUpgradeFactor = 0,
                            AiAgressive = true,
                            InitPosition = InitPosition.EnemySpawnScatter,
                            Dropouts = new Dropout[]{
                                new Dropout(ItemHelper.GetItem(ItemTable.Red.ToString())),
                                new Dropout(ItemHelper.GetItem(ItemTable.Purple.ToString()))
                            }
                        }
                    },
                    WinCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  ItemTable.KillEnemy.ToString(),
                            Amount = 15,
                            Description = "击杀{0}个敌人"
                        }
                    },
                    LossCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  ItemTable.KillTeam.ToString(),
                            Amount = 15,
                            Description = "被击杀{0}个队友"
                        }
                    },
                    AllowRespawn = true,
                    TeamAttackThreshold = 0,
                    EnemyAttackThreshold = 0
                },
                new CombatLevelRule
                {
                    LevelName = "攻城作战",
                    Description = "深入敌营，破坏敌人重军防守的主要目标！可能会出现12-14个水平相当的敌人",
                    MapType = MapType.Middle,
                    TeamSpawn = new RectInt(5, 5, 5, 5),
                    EnemySpawn = new RectInt(25, 25, 5, 5),
                    OperatorPrefabs = new OperatorPrefab[]
                    {
                        new OperatorPrefab
                        {
                            OpInfo = getRandomCA(),
                            MinAmount = 5,
                            MaxAmount = 8,
                            UseRandomCModel = true,
                            MechaRandomUpgradeFactor = 0,
                            AiAgressive = true,
                            InitPosition = InitPosition.EnemySpawnScatter,
                        },
                        new OperatorPrefab
                        {
                            OpInfo = getRandomCV(),
                            MinAmount = 1,
                            MaxAmount = 2,
                            UseRandomCModel = true,
                            MechaRandomUpgradeFactor = 0,
                            AiAgressive = true,
                            InitPosition = InitPosition.EnemySpawnScatter,
                        }
                    },
                    InteractablePrefabs = new InteractablePrefab[]
                    {
                        new InteractablePrefab
                        {
                            ObjectId = "secret",
                            InteractTip = "破坏",
                            Duration = 3,
                            MinAmount = 1,
                            MaxAmount = 1,
                            InitPosition = InitPosition.EnemySpawnCenter,
                            Dropouts = new Dropout[]{
                                new Dropout(ItemHelper.GetItem(ItemTable.Key.ToString()))
                            },
                            ModelUrl = "Objects/Key",
                        }
                    },
                    WinCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  ItemTable.Key.ToString(),
                            Amount = 1,
                            Description = "摧毁{0}个红色方块"
                        }
                    },
                    LossCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  ItemTable.Time.ToString(),
                            Amount = 120,
                            Description = "时间经过{0}秒"
                        }
                    },
                    AllowRespawn = true,
                    TeamAttackThreshold = 0.5f,
                    EnemyAttackThreshold = 0.5f
                },
                new CombatLevelRule
                {
                    LevelName = "三角定位",
                    Description = "激活地图上的三个红色方块！小心散落在地图上的敌人",
                    ImageUrl = "Textures/combatLevel-3",
                    MapType = MapType.Middle,
                    TeamSpawn = new RectInt(5, 5, 5, 5),
                    EnemySpawn = new RectInt(15, 15, 5, 5),
                    OperatorPrefabs = new OperatorPrefab[]
                    {
                        new OperatorPrefab
                        {
                            OpInfo = getRandomCA(),
                            MinAmount = 5,
                            MaxAmount = 8,
                            UseRandomCModel = true,
                            MechaRandomUpgradeFactor = 0,
                            AiAgressive = true,
                            InitPosition = InitPosition.MapScatter,
                        },
                        new OperatorPrefab
                        {
                            OpInfo = getRandomCV(),
                            MinAmount = 1,
                            MaxAmount = 2,
                            UseRandomCModel = true,
                            MechaRandomUpgradeFactor = 0,
                            AiAgressive = true,
                            InitPosition = InitPosition.MapScatter,
                        }
                    },
                    InteractablePrefabs = new InteractablePrefab[]
                    {
                        new InteractablePrefab
                        {
                            ObjectId = "secret",
                            InteractTip = "激活",
                            Duration = 3,
                            MinAmount = 3,
                            MaxAmount = 3,
                            InitPosition = InitPosition.MapScatter,
                            Dropouts = new Dropout[]
                            {
                                new Dropout(ItemHelper.GetItem(ItemTable.Key.ToString()))
                            },
                            ModelUrl = "Objects/Key",
                        }
                    },
                    WinCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  ItemTable.Key.ToString(),
                            Amount = 3,
                            Description = "激活{0}个红色方块"
                        }
                    },
                    LossCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  ItemTable.Time.ToString(),
                            Amount = 120,
                            Description = "时间经过{0}秒"
                        }
                    },
                    AllowRespawn = true,
                    TeamAttackThreshold = 0.5f,
                    EnemyAttackThreshold = 0.5f
                },
                new CombatLevelRule
                {
                    LevelName = "阻止入侵",
                    Description = "抵抗不明势力的猛烈进攻，保证指挥部安全，持续120秒！可能会出现12-14个水平相当的敌人。若失败会导致游戏结束。",
                    IsOnly = true,
                    EnableFunc = (database) => {
                        return database.Inventory[ItemTable.GTime.ToString()] % 7 == 0;
                    },
                    MapType = MapType.Invasion,
                    TeamSpawn = new RectInt(5, 5, 5, 5),
                    EnemySpawn = new RectInt(70, 70, 5, 5),
                    OperatorPrefabs = new OperatorPrefab[]
                        {
                            new OperatorPrefab
                            {
                                OpInfo = getRandomCA(),
                                MinAmount = 5,
                                MaxAmount = 8,
                                UseRandomCModel = true,
                                MechaRandomUpgradeFactor = 0,
                                AiAgressive = true,
                                InitPosition = InitPosition.EnemySpawnScatter,
                            },
                            new OperatorPrefab
                            {
                                OpInfo = getRandomCV(),
                                MinAmount = 1,
                                MaxAmount = 2,
                                UseRandomCModel = true,
                                MechaRandomUpgradeFactor = 0,
                                AiAgressive = true,
                                InitPosition = InitPosition.EnemySpawnScatter,
                            }
                        },
                    AllowHomeBuilding = true,
                    WinCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  ItemTable.Time.ToString(),
                            Amount = 120,
                            Description = "时间经过{0}秒"
                        }
                    },
                    LossCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  ItemTable.Key.ToString(),
                            Amount = 1,
                            Description = "基地被摧毁{0}"
                        }
                    },
                    AllowRespawn = true,
                    TeamAttackThreshold = 0.5f,
                    EnemyAttackThreshold = 0
                },
                new CombatLevelRule
                {
                    LevelName = "演习作战ProMax",
                    Description = "达到击杀数就是胜利！可能会出现40-50个水平相当的敌人",
                    ImageUrl = "Textures/combatLevel-1",
                    MapType = MapType.Big,
                    TeamSpawn = new RectInt(5, 5, 10, 5),
                    EnemySpawn = new RectInt(35, 35, 10, 5),
                    OperatorPrefabs = new OperatorPrefab[]
                    {
                        new OperatorPrefab
                        {
                            OpInfo = getRandomDD(),
                            MinAmount = 20,
                            MaxAmount = 25,
                            UseRandomCModel = true,
                            MechaRandomUpgradeFactor = 0,
                            AiAgressive = true,
                            InitPosition = InitPosition.EnemySpawnScatter,
                            Dropouts = new Dropout[]{
                                new Dropout(ItemHelper.GetItem(ItemTable.Sphere.ToString()))
                            }
                        },
                        new OperatorPrefab
                        {
                            OpInfo = getRandomCA(),
                            MinAmount = 13,
                            MaxAmount = 17,
                            UseRandomCModel = true,
                            MechaRandomUpgradeFactor = 0,
                            AiAgressive = true,
                            InitPosition = InitPosition.EnemySpawnScatter,
                            Dropouts = new Dropout[]{
                                new Dropout(ItemHelper.GetItem(ItemTable.Sphere.ToString()))
                            }
                        },
                        new OperatorPrefab
                        {
                            OpInfo = getRandomCV(),
                            MinAmount = 4,
                            MaxAmount = 6,
                            UseRandomCModel = true,
                            MechaRandomUpgradeFactor = 0,
                            AiAgressive = true,
                            InitPosition = InitPosition.EnemySpawnScatter,
                            Dropouts = new Dropout[]{
                                new Dropout(ItemHelper.GetItem(ItemTable.Red.ToString())),
                                new Dropout(ItemHelper.GetItem(ItemTable.Purple.ToString()))
                            }
                        }
                    },
                    WinCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  ItemTable.KillEnemy.ToString(),
                            Amount = 60,
                            Description = "击杀{0}个敌人"
                        }
                    },
                    LossCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  ItemTable.KillTeam.ToString(),
                            Amount = 60,
                            Description = "被击杀{0}个队友"
                        }
                    },
                    AllowRespawn = true,
                    TeamAttackThreshold = 0,
                    EnemyAttackThreshold = 0
                },
                // EventRule
                new EventLevelRule
                {
                    LevelName = "什么也不做",
                    Description = "啊对对对，你说什么我都开摆~",
                    ImageUrl = "Textures/eventLevel-aniya",
                    MessageAction = (context) => { },
                },
                new EventLevelRule
                {
                    LevelName = "我什么都做不到！",
                    Description = "密码正确！获得大量资源",
                    ImageUrl = "Textures/eventLevel-yayi",
                    MessageAction = (context) => {
                        context.Afford(new List<Produce>(){
                            new Produce { ItemId = ItemTable.Red.ToString(), Amount = -100 },
                            new Produce { ItemId = ItemTable.Iron.ToString(), Amount = -1000 },
                            new Produce { ItemId = ItemTable.Al.ToString(), Amount = -1000 },
                        });
                    },
                }
            };
        }

        public static List<string> GetConfigModelList()
        {
            return modelList;
        }

        /// <summary>
        /// 获得初始资源数据
        /// </summary>
        public static Dictionary<string, int> GetConfigInitInventory()
        {
            var res = new Dictionary<string, int>();
            res.Add(ItemTable.GTime.ToString(), 1);

            res.Add(ItemTable.Red.ToString(), 100);
            res.Add(ItemTable.Purple.ToString(), 100);
            res.Add(ItemTable.Sphere.ToString(), 100);

            res.Add(ItemTable.Electric.ToString(), 1000);
            res.Add(ItemTable.Iron.ToString(), 1000);
            res.Add(ItemTable.Al.ToString(), 1000);
            res.Add(ItemTable.Ammo.ToString(), 1000);

            return res;
        }

        public static HomeMessageQueue GetConfigInitHomeMessages()
        {
            return new HomeMessageQueue() {
                new HomeMessage {  // 卑微的胜利提示
                    Day = 50,
                    MessageActionId = 1,
                    MessageAction = (context) => { Debug.Log("You WIN!!!!"); }
                }
            };
        }

        public static BuildingArea GetConfigInitBuildingArea()
        {
            return new BuildingArea()
            {
                PlaceInfos = new PlaceInfo[] {
                new PlaceInfo { BuildingId = "h1", AreaIndex = 0, PlacePosition = new Vector2Int(4, 4)},
                new PlaceInfo { BuildingId = "h1", AreaIndex = 1, PlacePosition = new Vector2Int(4, 4)},
                new PlaceInfo { BuildingId = "e1", AreaIndex = 2, PlacePosition = new Vector2Int(1, 4)},
                new PlaceInfo { BuildingId = "e1", AreaIndex = 2, PlacePosition = new Vector2Int(4, 4)},
                new PlaceInfo { BuildingId = "cmd", AreaIndex = PlaceInfo.BattleIndex, PlacePosition = new Vector2Int(4, 0)},
                new PlaceInfo { BuildingId = "mgTower1", AreaIndex = PlaceInfo.BattleIndex, PlacePosition = new Vector2Int(10, 10)},
            }
            };
        }
        public static Dictionary<int, System.Action<HomeContextManager>> GetConfigHomeEvents()
        {
            return new Dictionary<int, System.Action<HomeContextManager>>
            {
                { 1, (context) => { Debug.Log("You WIN!!!!"); } }
            };
        }

        private static readonly List<string> modelList = new List<string> { "Hoshino", "Shiroko", "Aru", "Karin", "Mashiro" };
        // 区分当作原型使用的Operator
        private static Operator getRandomCA()
        {
            return new Operator
            {
                Name = "CA_",
                ModelResourceUrl = modelList[Random.Range(0, modelList.Count)],
                WeaponSkillId = 4,
                Type = OperatorType.CA,
                Id = "prefab_CA",
            };
        }
        private static Operator getRandomCV()
        {
            return new Operator
            {
                Name = "CV_",
                ModelResourceUrl = modelList[Random.Range(0, modelList.Count)],
                WeaponSkillId = 6,
                Type = OperatorType.CV,
                Fighters = new List<Fighter>{
                    new Fighter { Operator = new Operator { Name = "r1", ModelResourceUrl = modelList[Random.Range(0, modelList.Count)] } },
                    new Fighter { Operator = new Operator { Name = "r2", ModelResourceUrl = modelList[Random.Range(0, modelList.Count)] } }
                },
                Id = "prefab_CV"
            };
        }
        private static Operator getRandomDD()
        {
            return new Operator
            {
                Name = "DD_",
                ModelResourceUrl = modelList[Random.Range(0, modelList.Count)],
                WeaponSkillId = 8,
                Type = OperatorType.DD,
                Id = "prefab_DD"
            };
        }

    }
}
