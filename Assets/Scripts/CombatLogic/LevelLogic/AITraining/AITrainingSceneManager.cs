using Assets.Scripts.CombatLogic.Characters;
using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using Assets.Scripts.CombatLogic.Characters.Player;
using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.CombatLogic.ContextExtends;
using Assets.Scripts.CombatLogic.EnviormentLogic;
using Assets.Scripts.Common;
using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.Entities.Level;
using Assets.Scripts.HomeLogic.Environment;
using Assets.Scripts.Services;
using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Entities;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.CombatLogic.LevelLogic
{
    /// <summary>
    /// AI训练场景管理器 - 完全参考 GameStartupManager 的实现
    /// </summary>
    public class AITrainingSceneManager : MonoBehaviour
    {
        [SerializeField] LightManager lightManager;
        CombatContextManager _context => CombatContextManager.Instance;

        [Header("训练配置")]
        public AITrainingConfig TrainingConfig;
        public bool EnableTrainingMode = true;
        public bool AutoStartTraining = true;

        private AITrainingManager _trainingManager;
        private List<ExternalBehaviorTree> _testOpponents = new List<ExternalBehaviorTree>();
        private int _currentOpponentIndex = 0;
        private int _currentRound = 0;
        private const int RoundsPerOpponent = 3;

        private void Awake()
        {
            if (!EnableTrainingMode) return;
            _trainingManager = GetComponent<AITrainingManager>() ?? gameObject.AddComponent<AITrainingManager>();
        }

        private void Start()
        {
            if (!EnableTrainingMode) return;

            // 初始化技能
            transform.GetComponent<SkillManager>()?.Init();

            // 设置训练配置
            MyConfig.IsEnemyNeedDrop = false;
            SetupTrainingConfig();
            UIManager.Instance.Init();

            if (AutoStartTraining)
            {
                StartCoroutine(TrainingSequence());
            }
        }

        private void SetupTrainingConfig()
        {
            if (TrainingConfig == null)
            {
                // 尝试加载默认训练配置
                TrainingConfig = Resources.Load<AITrainingConfig>("AITraining/New_Training_Config");
                
                // 如果仍为空，创建默认配置
                if (TrainingConfig == null)
                {
                    TrainingConfig = ScriptableObject.CreateInstance<AITrainingConfig>();
                    TrainingConfig.UseAIForPlayerSlot = true;
                    TrainingConfig.TeamOperatorCount = 5;
                    TrainingConfig.EnemyOperatorCount = 5;
                    TrainingConfig.MapType = MapType.Small;
                    TrainingConfig.MaxBattleDuration = 120f;
                }
            }
            _trainingManager.InitializeTraining(TrainingConfig);
        }

        private IEnumerator TrainingSequence()
        {
            SetupTestOpponents();

            for (_currentOpponentIndex = 0; _currentOpponentIndex < _testOpponents.Count; _currentOpponentIndex++)
            {
                var opponent = _testOpponents[_currentOpponentIndex];
                if (opponent == null) continue;

                for (_currentRound = 0; _currentRound < RoundsPerOpponent; _currentRound++)
                {
                    Debug.Log($"[AITraining] 对战 {opponent.name} - 第 {_currentRound + 1}/{RoundsPerOpponent} 轮");
                    
                    yield return StartCoroutine(PrepareBattleRound(opponent));
                    yield return StartCoroutine(WaitForBattleEnd());
                    yield return new WaitForSeconds(1f);
                }
            }

            var result = _trainingManager.EndTrainingSession();
            Debug.Log($"[TRAINING_FINAL_REPORT] {JsonUtility.ToJson(result)}");
        }

        private void SetupTestOpponents()
        {
            _testOpponents.Clear();
            if (TrainingConfig.BaseBehaviorTree != null)
                _testOpponents.Add(TrainingConfig.BaseBehaviorTree);
            if (TrainingConfig.UseMultipleOpponents && TrainingConfig.BestBehaviorTree != null 
                && TrainingConfig.BestBehaviorTree != TrainingConfig.BaseBehaviorTree)
                _testOpponents.Add(TrainingConfig.BestBehaviorTree);
            if (TrainingConfig.TeamBehaviorTree != null 
                && TrainingConfig.TeamBehaviorTree != TrainingConfig.BaseBehaviorTree)
                _testOpponents.Add(TrainingConfig.TeamBehaviorTree);
        }

        private IEnumerator PrepareBattleRound(ExternalBehaviorTree enemyBehaviorTree)
        {
            yield return StartCoroutine(ClearScene());

            var levelInfo = GenerateTrainingLevelInfo();
            _context.CombatVM.Level = levelInfo;
            _context.CombatVM.LevelResult = new CombatLevelResult { 
                CombatStatu = CombatStatu.Ing, 
                LevelRule = levelInfo.LevelRule, 
                JoinOperator = levelInfo.TeamOperators 
            };

            // 完全按照 GameStartupManager 的方式生成关卡
            PrepareLevel(levelInfo, enemyBehaviorTree);

            _trainingManager.StartBattleRecording(
                TrainingConfig.TeamBehaviorTree ? TrainingConfig.TeamBehaviorTree.name : "Current",
                enemyBehaviorTree.name
            );

            yield return null;
        }

        private IEnumerator ClearScene()
        {
            // 清理 Agents 下的所有角色
            var agentsTrans = transform.Find("Agents");
            if (agentsTrans != null)
            {
                for (int i = agentsTrans.childCount - 1; i >= 0; i--)
                {
                    Destroy(agentsTrans.GetChild(i).gameObject);
                }
            }

            // 清理 Terrain 下的地形（保留 collider）
            var terrainTrans = transform.Find("Terrain");
            if (terrainTrans != null)
            {
                for (int i = terrainTrans.childCount - 1; i >= 0; i--)
                {
                    var child = terrainTrans.GetChild(i);
                    if (child.name == "collider") continue;
                    Destroy(child.gameObject);
                }
            }

            _context.Operators.Clear();
            _context.PlayerTeamTrans.Clear();
            _context.EnemyTeamTrans.Clear();
            if(UIManager.Instance.IsInitialized) UIManager.Instance.Refresh();
            yield return new WaitForSeconds(0.1f);
        }

        private CombatLevelInfo GenerateTrainingLevelInfo()
        {
            var levelRule = new CombatLevelRule
            {
                MapType = TrainingConfig.MapType,
                IsAllowRespawn = false,
                AllowHomeBuilding = false,
                TeamAttackThreshold = 1f,
                EnemyAttackThreshold = 0f,
                WinCondition = new Condition[]
                {
                    new Condition { ItemName = MyConfig.ItemTable.KillEnemy.ToString(), Amount = TrainingConfig.EnemyOperatorCount, Description = "击败所有敌人 {0}\n" }
                },
                LossCondition = new Condition[]
                {
                    new Condition { ItemName = MyConfig.ItemTable.KillTeam.ToString(), Amount = TrainingConfig.TeamOperatorCount, Description = "友方全灭 {0}\n" }
                }
            };

            // 使用与 GameConfigInitHelper 中一致的坐标定义
            // RectInt(x, y, width, height) - x,y是起始位置，width,height是宽高
            switch (TrainingConfig.MapType)
            {
                case MapType.Small:
                    levelRule.TeamSpawn = new RectInt(5, 5, 5, 5);      // 友方在 (5~10, 5~10)
                    levelRule.EnemySpawn = new RectInt(25, 25, 5, 5);   // 敌方在 (25~30, 25~30)
                    break;
                case MapType.Middle:
                    levelRule.TeamSpawn = new RectInt(5, 5, 5, 5);
                    levelRule.EnemySpawn = new RectInt(25, 25, 5, 5);
                    break;
                case MapType.Big:
                    levelRule.TeamSpawn = new RectInt(5, 5, 10, 5);     // Big地图更大的出生区
                    levelRule.EnemySpawn = new RectInt(35, 35, 10, 5);
                    break;
                default:
                    levelRule.TeamSpawn = new RectInt(5, 5, 5, 5);
                    levelRule.EnemySpawn = new RectInt(25, 25, 5, 5);
                    break;
            }

            var levelInfo = new CombatLevelInfo
            {
                LevelRule = levelRule,
                Map = GenerateMap(TrainingConfig.MapType)
            };

            // 友方和敌方人员
            levelInfo.TeamOperators = generateTestOperators(TrainingConfig.TeamOperatorCount, 0);
            levelInfo.EnemyOperators = generateTestOperators(TrainingConfig.EnemyOperatorCount, 1);

            levelInfo.EnemyOperatorsBy = new List<OperatorPrefab>();
            foreach (var op in levelInfo.EnemyOperators)
            {
                levelInfo.EnemyOperatorsBy.Add(new OperatorPrefab
                {
                    OpInfo = op,
                    MinAmount = 1,
                    MaxAmount = 1,
                    AiAgressive = TrainingConfig.EnemyAiAggressive,
                    InitPosition = InitPosition.EnemySpawnScatter
                });
            }

            return levelInfo;
        }

        private int[][] GenerateMap(MapType mapType)
        {
            int size = mapType == MapType.Small ? 20 : (mapType == MapType.Middle ? 30 : 40);
            int[][] map = new int[size][];
            for (int i = 0; i < size; i++)
            {
                map[i] = new int[size];
                for (int j = 0; j < size; j++) map[i][j] = 0;
            }
            return map;
        }

        /// <summary>
        /// 完全复制 GameStartupManager.prepareLevel 的逻辑
        /// </summary>
        private void PrepareLevel(CombatLevelInfo level, ExternalBehaviorTree enemyBehaviorTree)
        {
            // terrain
            _context.GenerateTerrain(level.Map);

            // team - 完全按照 GameStartupManager 的方式
            {
                var ops = level.TeamOperators;
                var spawn = level.LevelRule.TeamSpawn;

                var spawnTrans = new GameObject().transform;
                spawnTrans.parent = transform;  // 直接挂到 GameRoot
                spawnTrans.gameObject.name = "TeamSpawnGO";
                spawnTrans.position = new Vector3(spawn.x + Random.Range(0, spawn.width), 0, spawn.y + Random.Range(0, spawn.height));

                for (int i = 0; i < ops.Count; i++)
                {
                    // 严格按照 GameStartupManager 的坐标计算方式
                    var v3 = new Vector3(spawn.x + Random.Range(0, spawn.width), 0, spawn.y + Random.Range(0, spawn.height));
                    
                    _context.GenerateAgent(ops[i], v3, Vector3.zero, 0, spawnTrans);
                }
            }

            // enemy - 完全按照 GameStartupManager 的方式
            {
                var ops = level.EnemyOperators;
                var spawn = level.LevelRule.EnemySpawn;

                var spawnTrans = new GameObject().transform;
                spawnTrans.parent = transform;  // 直接挂到 GameRoot
                spawnTrans.gameObject.name = "EnemySpawnGO";
                spawnTrans.position = new Vector3(spawn.x + Random.Range(0, spawn.width), 0, spawn.y + Random.Range(0, spawn.height));

                for (int i = 0; i < ops.Count; i++)
                {
                    // 严格按照 GameStartupManager 的方式，使用 GetPosByInitMethod
                    var v3 = GetPosByInitMethod(level, level.EnemyOperatorsBy[i].InitPosition);
                    // team = 1 表示敌方
                    _context.GenerateAgent(ops[i], v3, Vector3.zero, 1, spawnTrans);
                }
            }

            // NavMesh
            var nm = transform.Find("NavMesh Surface").GetComponent<NavMeshSurface>();
            nm.UpdateNavMesh(nm.navMeshData);

            // 添加必要的组件
            if (transform.GetComponent<AnimeHelper>() == null)
                transform.AddComponent<AnimeHelper>();


            GameLevelManager glm = transform.GetComponent<GameLevelManager>();
            if (glm == null)
            {
                glm = transform.AddComponent<GameLevelManager>();
            }
            glm.Init(level.LevelRule);

            // 应用行为树
            ApplyBehaviorTrees(enemyBehaviorTree);

            // 随机灯光
            if (Random.Range(0, 2) == 0) lightManager.Day();
            else lightManager.Night();
        }

        private Vector3 GetPosByInitMethod(CombatLevelInfo level, InitPosition method)
        {
            if (method == InitPosition.EnemySpawnCenter)
            {
                return new Vector3(level.LevelRule.EnemySpawn.center.x, 0, level.LevelRule.EnemySpawn.center.y);
            }
            else if (method == InitPosition.EnemySpawnScatter)
            {
                var spawn = level.LevelRule.EnemySpawn;
                return new Vector3(spawn.x + Random.Range(0, spawn.width), 0, spawn.y + Random.Range(0, spawn.height));
            }
            else if (method == InitPosition.MapScatter)
            {
                return new Vector3(Random.Range(0, level.Map.Length), 0, Random.Range(0, level.Map[0].Length));
            }
            return Vector3.zero;
        }

        private void SetupAIForPlayer(Transform playerTrans)
        {
            var playerController = playerTrans.GetComponent<PlayerController>();
            if (playerController != null) Destroy(playerController);

            var agentController = playerTrans.GetComponent<AgentController>();
            if (agentController == null) playerTrans.gameObject.AddComponent<AgentController>();
        }

        private void ApplyBehaviorTrees(ExternalBehaviorTree enemyBehaviorTree)
        {
            foreach (var teamTrans in _context.PlayerTeamTrans)
                _trainingManager.ApplyBehaviorTree(teamTrans, TrainingConfig.TeamBehaviorTree);
            
            foreach (var enemyTrans in _context.EnemyTeamTrans)
                _trainingManager.ApplyBehaviorTree(enemyTrans, enemyBehaviorTree);
        }

        private IEnumerator WaitForBattleEnd()
        {
            while (_trainingManager.IsRecording)
                yield return new WaitForSeconds(0.5f);
        }

        [ContextMenu("Start Training")]
        public void ManualStartTraining()
        {
            if (!EnableTrainingMode) EnableTrainingMode = true;
            StartCoroutine(TrainingSequence());
        }

        private int _opId;
        private List<Operator> generateTestOperators(int num, int team)
        {
            var res = new List<Operator>();
            for (int i = 0; i < num - 1; i++)
            {
                res.Add(new Operator
                {
                    Name = $"CA_{_opId}", ModelResourceUrl = team == 0 ? "Shiroko" : "Hoshino", Id = (_opId).ToString(),
                    Trait = OperatorTrait.Tactical
                });
                _opId += 1;
            }
            res.Add(new Operator { Name = $"CV_{_opId}", ModelResourceUrl = "Shiroko", Type = OperatorType.CV,
                WeaponSkillId = 6,
                Fighters = new List<Fighter>{
                    new Fighter { Operator = new Operator { Name = "ho", ModelResourceUrl = "Hoshino", Id = (_opId++).ToString() } },
                    new Fighter { Operator = new Operator { Name = "shi", ModelResourceUrl = "Shiroko", Id = (_opId++).ToString() } }
                },
                Id = _opId.ToString()
            });
            _opId += 1;
            return res;
        }
    }
}