using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Mechas;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Cinemachine.DocumentationSortingAttribute;

namespace Assets.Scripts
{
    internal static class TestDB
    {
        #region 常量配置
        public static readonly int CHARACTER_LAYER = LayerMask.NameToLayer("Character");
        public static readonly int DOBJECT_LAYER = LayerMask.NameToLayer("DestructibleObject");
        public static readonly string PLAYER_TAG = "Player";
        public enum SkillSelectorStr
        {
            Trigger
        }
        #endregion

        #region 全局信息（如关卡、仓库状态）
        public static List<LevelRule> LevelRules { get; private set; }
        public enum DropoutTable
        {
            KillEnemy,
            KillTeam,
            Time,
            Key
        }
        #endregion

        static TestDB()
        {
            // 演习作战（聚集分布敌人，进攻性强）、攻城作战（互动单位，中范围聚集分布敌人，进攻性弱）、三角定位（互动单位，和随机位置分布敌人，进攻性弱）
            LevelRules = new List<LevelRule>()
            {
                new LevelRule
                {
                    LevelName = "演习作战",
                    Description = "达到击杀数就是胜利！可能会出现8-10个水平相当的敌人",
                    MapSize = MapSize.Middle,
                    TeamSpawn = new RectInt(5, 5, 5, 5),
                    EnemySpawn = new RectInt(25, 25, 5, 5),
                    OperatorPrefabs = new OperatorPrefab[]
                    {
                        new OperatorPrefab
                        {
                            OpInfo = GetRandomCA(),
                            MinAmount = 5,
                            MaxAmount = 8,
                            UseRandomCModel = true,
                            MechaRandomUpgradeFactor = 0,
                            AiAgressive = true,
                            InitPosition = InitPosition.EnemySpawnScatter,
                        },
                        new OperatorPrefab
                        {
                            OpInfo = GetRandomCV(),
                            MinAmount = 1,
                            MaxAmount = 2,
                            UseRandomCModel = true,
                            MechaRandomUpgradeFactor = 0,
                            AiAgressive = true,
                            InitPosition = InitPosition.EnemySpawnScatter,
                        }
                    },
                    WinCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  DropoutTable.KillEnemy.ToString(),
                            Amount = 15,
                            Description = "击杀{0}个敌人"
                        }
                    },
                    LossCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  DropoutTable.KillTeam.ToString(),
                            Amount = 15,
                            Description = "被击杀{0}个队友"
                        }
                    },
                    AllowRespawn = true,
                    TeamAttackThreshold = 0,
                    EnemyAttackThreshold = 0
                },
                new LevelRule
                {
                    LevelName = "攻城作战",
                    Description = "深入敌营，破坏敌人重军防守的主要目标！可能会出现12-14个水平相当的敌人",
                    MapSize = MapSize.Middle,
                    TeamSpawn = new RectInt(5, 5, 5, 5),
                    EnemySpawn = new RectInt(25, 25, 5, 5),
                    OperatorPrefabs = new OperatorPrefab[]
                    {
                        new OperatorPrefab
                        {
                            OpInfo = GetRandomCA(),
                            MinAmount = 5,
                            MaxAmount = 8,
                            UseRandomCModel = true,
                            MechaRandomUpgradeFactor = 0,
                            AiAgressive = true,
                            InitPosition = InitPosition.EnemySpawnScatter,
                        },
                        new OperatorPrefab
                        {
                            OpInfo = GetRandomCV(),
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
                            Dropouts = new KeyValuePair<string, int>[] {
                                new KeyValuePair<string, int>(DropoutTable.Key.ToString(), 1),
                            },
                            ModelUrl = "Objects/Key",
                        }
                    },
                    WinCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  DropoutTable.Key.ToString(),
                            Amount = 1,
                            Description = "摧毁{0}个红色方块"
                        }
                    },
                    LossCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  DropoutTable.Time.ToString(),
                            Amount = 120,
                            Description = "时间经过{0}秒"
                        }
                    },
                    AllowRespawn = true,
                    TeamAttackThreshold = 0.5f,
                    EnemyAttackThreshold = 0.5f
                },
                new LevelRule
                {
                    LevelName = "三角定位",
                    Description = "激活地图上的三个红色方块！小心散落在地图上的敌人",
                    MapSize = MapSize.Middle,
                    TeamSpawn = new RectInt(5, 5, 5, 5),
                    EnemySpawn = new RectInt(15, 15, 5, 5),
                    OperatorPrefabs = new OperatorPrefab[]
                    {
                        new OperatorPrefab
                        {
                            OpInfo = GetRandomCA(),
                            MinAmount = 5,
                            MaxAmount = 8,
                            UseRandomCModel = true,
                            MechaRandomUpgradeFactor = 0,
                            AiAgressive = true,
                            InitPosition = InitPosition.MapScatter,
                        },
                        new OperatorPrefab
                        {
                            OpInfo = GetRandomCV(),
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
                            Dropouts = new KeyValuePair<string, int>[] {
                                new KeyValuePair<string, int>(DropoutTable.Key.ToString(), 1),
                            },
                            ModelUrl = "Objects/Key",
                        }
                    },
                    WinCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  DropoutTable.Key.ToString(),
                            Amount = 3,
                            Description = "激活{0}个红色方块"
                        }
                    },
                    LossCondition = new Condition[]{
                        new Condition
                        {
                            ItemName =  DropoutTable.Time.ToString(),
                            Amount = 120,
                            Description = "时间经过{0}秒"
                        }
                    },
                    AllowRespawn = true,
                    TeamAttackThreshold = 0.5f,
                    EnemyAttackThreshold = 0.5f
                }
            };
        }

        // 临时存储，传递到下一个场景的信息
        public static LevelInfo Level { get; set; }

        public static List<Operator> GetOperators()
        {
            // TODO: these just for test
            return new List<Operator>() {
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino", WeaponSkillId = 4 },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", Type = OperatorType.CV, 
                    WeaponSkillId = 6,
                    Fighters = new List<Fighter>{ 
                        new Fighter { Operator = new Operator { Name = "ho", ModelResourceUrl = "Hoshino" } },
                        new Fighter { Operator = new Operator { Name = "shi", ModelResourceUrl = "Shiroko" } }
                    } 
                },

                new Operator { Name = "aru", ModelResourceUrl = "Aru",  },
                new Operator { Name = "akrin", ModelResourceUrl = "Karin",  },
                new Operator { Name = "mashiro", ModelResourceUrl = "Mashiro",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
            };
        }

        public static List<MechaBase> GetMechas()
        {
            return new List<MechaBase>
            {
                MechaBody.DefaultMecha(),
                MechaHead.DefaultMecha(),
                MechaLeg.DefaultMecha()
            };
        }

        static List<string> clist = new List<string> { "Hoshino", "Shiroko", "Aru", "Karin", "Mashiro"};
        public static string GetRandomModelUrl()
        {
            return clist[Random.Range(0, clist.Count)];
        }
        public static Operator GetRandomCA()
        {
            return new Operator
            {
                Name = "random test",
                ModelResourceUrl = clist[Random.Range(0, clist.Count)],
                WeaponSkillId = 4,
                Type = OperatorType.CA
            };
        }
        public static Operator GetRandomCV()
        {
            return new Operator
            {
                Name = "random test",
                ModelResourceUrl = clist[Random.Range(0, clist.Count)],
                WeaponSkillId = 6,
                Type = OperatorType.CV,
                Fighters = new List<Fighter>{
                    new Fighter { Operator = new Operator { Name = "r1", ModelResourceUrl = clist[Random.Range(0, clist.Count)] } },
                    new Fighter { Operator = new Operator { Name = "r2", ModelResourceUrl = clist[Random.Range(0, clist.Count)] } }
                }
            };
        }
        public static List<Operator> GetRandomOperator(int t)
        {
            var ans = new List<Operator>(t);
            for(int i = 0; i < t; i++) ans.Add(GetRandomCA());

            return ans;
        }
    }
}
