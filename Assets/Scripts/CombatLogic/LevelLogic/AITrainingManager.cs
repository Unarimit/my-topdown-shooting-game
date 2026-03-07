using Assets.Scripts.CombatLogic.Characters;
using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using Assets.Scripts.CombatLogic.Characters.Player;
using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Level;
using Assets.Scripts.Services;
using BehaviorDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.LevelLogic
{
    /// <summary>
    /// AI训练管理器，用于收集战斗数据并输出训练结果
    /// 通过Unity Console打印JSON格式结果，供MCP读取
    /// </summary>
    public class AITrainingManager : MonoBehaviour
    {
        public static AITrainingManager Instance { get; private set; }

        [Header("训练配置")]
        [Tooltip("当前使用的训练配置")]
        public AITrainingConfig TrainingConfig;

        [Header("训练状态")]
        public bool IsTrainingMode = false;
        public bool IsRecording = false;

        // 当前会话数据
        private TrainingSessionResult _currentSession;
        private TrainingBattleResult _currentBattle;
        private float _battleStartTime;
        private List<CombatOperator> _teamOperatorsSnapshot;
        private List<CombatOperator> _enemyOperatorsSnapshot;

        // Agent行为追踪数据
        private Dictionary<Transform, AgentBehaviorData> _agentBehaviorData = new Dictionary<Transform, AgentBehaviorData>();

        // 行为树引用缓存
        private Dictionary<Transform, ExternalBehaviorTree> _originalBehaviorTrees = new Dictionary<Transform, ExternalBehaviorTree>();

        /// <summary>
        /// Agent行为追踪数据结构（简化版）
        /// </summary>
        private class AgentBehaviorData
        {
            public Transform AgentTransform;
            public BehaviorTree BehaviorTree;  // 行为树引用
            public string AgentName;
            public int Team;
            public Vector3 StartPosition;
            public Vector3 LastPosition;
            public float TotalDistanceMoved;
            public float TimeStationary;
            public float TimeInCombatRange;
            public int TimesInAttackRange;
            public bool WasInAttackRange;
            public float MaxAttackRange;
            
            // 行为树节点追踪
            public string LastActiveTaskName;
            public string CurrentActiveTaskName;
            public float TimeInCurrentTask;  // 在当前任务停留的时间
            public Dictionary<string, float> TaskDurations = new Dictionary<string, float>();  // 各任务停留时长统计
        }

        // 事件
        public event Action<TrainingBattleResult> OnBattleEnd;
        public event Action<TrainingSessionResult> OnSessionEnd;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        /// <summary>
        /// 初始化训练模式
        /// </summary>
        public void InitializeTraining(AITrainingConfig config)
        {
            TrainingConfig = config;
            IsTrainingMode = true;
            
            _currentSession = new TrainingSessionResult
            {
                SessionId = Guid.NewGuid().ToString("N")[..8],
                StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                BehaviorTreeVersion = config.TeamBehaviorTree ? config.TeamBehaviorTree.name : "Unknown"
            };

            if (config.VerboseLogging)
            {
                Debug.Log($"[AITraining] 训练会话初始化: {_currentSession.SessionId}");
                Debug.Log($"[AITraining] 使用行为树: {_currentSession.BehaviorTreeVersion}");
            }
        }

        /// <summary>
        /// 开始记录一场对战
        /// </summary>
        public void StartBattleRecording(string teamBehaviorName, string enemyBehaviorName)
        {
            if (!IsTrainingMode) return;

            _currentBattle = new TrainingBattleResult
            {
                ConfigName = TrainingConfig ? TrainingConfig.ConfigName : "Default",
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TeamBehaviorTreeName = teamBehaviorName,
                EnemyBehaviorTreeName = enemyBehaviorName
            };

            _battleStartTime = Time.time;
            IsRecording = true;

            // 记录初始状态
            var context = CombatContextManager.Instance;
            _teamOperatorsSnapshot = context.PlayerTeamTrans
                .Select(t => context.Operators[t])
                .ToList();
            _enemyOperatorsSnapshot = context.EnemyTeamTrans
                .Select(t => context.Operators[t])
                .ToList();

            // 记录最大HP
            _currentBattle.TeamMaxTotalHP = _teamOperatorsSnapshot.Sum(o => o.MaxHP);
            _currentBattle.EnemyMaxTotalHP = _enemyOperatorsSnapshot.Sum(o => o.MaxHP);

            // 启动Agent行为追踪
            StartAgentBehaviorTracking();

            if (TrainingConfig && TrainingConfig.VerboseLogging)
            {
                Debug.Log($"[AITraining] 开始记录对战: {teamBehaviorName} vs {enemyBehaviorName}");
            }
        }

        /// <summary>
        /// 启动Agent行为追踪
        /// </summary>
        private void StartAgentBehaviorTracking()
        {
            _agentBehaviorData.Clear();
            var context = CombatContextManager.Instance;
            
            // 追踪友方
            foreach (var trans in context.PlayerTeamTrans)
            {
                if (context.Operators.TryGetValue(trans, out var op))
                {
                    var behaviorTree = trans.GetComponent<BehaviorTree>();
                    _agentBehaviorData[trans] = new AgentBehaviorData
                    {
                        AgentTransform = trans,
                        BehaviorTree = behaviorTree,
                        AgentName = op.OpInfo.Name,
                        Team = 0,
                        StartPosition = trans.position,
                        LastPosition = trans.position,
                        MaxAttackRange = op.AttackRange,
                        CurrentActiveTaskName = GetActiveTaskName(behaviorTree)
                    };
                }
            }
            
            // 追踪敌方
            foreach (var trans in context.EnemyTeamTrans)
            {
                if (context.Operators.TryGetValue(trans, out var op))
                {
                    var behaviorTree = trans.GetComponent<BehaviorTree>();
                    _agentBehaviorData[trans] = new AgentBehaviorData
                    {
                        AgentTransform = trans,
                        BehaviorTree = behaviorTree,
                        AgentName = op.OpInfo.Name,
                        Team = 1,
                        StartPosition = trans.position,
                        LastPosition = trans.position,
                        MaxAttackRange = op.AttackRange,
                        CurrentActiveTaskName = GetActiveTaskName(behaviorTree)
                    };
                }
            }
        }

        /// <summary>
        /// 获取行为树当前活跃的任务名称（外部访问，不修改行为树节点）
        /// 简化版：通过反射获取行为树执行栈信息
        /// </summary>
        private string GetActiveTaskName(BehaviorTree behaviorTree)
        {
            if (behaviorTree == null) return "None";
            
            // 使用行为树的ExecutionStatus来判断状态
            var executionStatus = behaviorTree.ExecutionStatus;
            
            // 尝试通过反射获取行为树中的活跃任务信息
            // 注意：这是外部监控，不修改行为树节点本身
            try
            {
                // 获取行为树的外部行为名称作为标识
                var externalBehavior = behaviorTree.ExternalBehavior;
                if (externalBehavior != null)
                {
                    return externalBehavior.name;
                }
            }
            catch { }
            
            return executionStatus.ToString();
        }

        /// <summary>
        /// 更新Agent行为数据
        /// </summary>
        private void UpdateAgentBehaviorTracking()
        {
            if (_agentBehaviorData.Count == 0) return;
            
            var context = CombatContextManager.Instance;
            
            foreach (var kvp in _agentBehaviorData)
            {
                var data = kvp.Value;
                var trans = kvp.Key;
                
                if (trans == null) continue;
                
                // 计算移动距离
                float distance = Vector3.Distance(trans.position, data.LastPosition);
                data.TotalDistanceMoved += distance;
                
                // 检测静止
                if (distance < 0.05f)
                {
                    data.TimeStationary += Time.deltaTime;
                }
                
                // 检查战斗参与
                UpdateCombatParticipation(data, context);
                
                // 更新行为树节点监控（外部访问，不修改行为树）
                UpdateBehaviorTreeTracking(data);
                
                data.LastPosition = trans.position;
            }
        }

        /// <summary>
        /// 更新行为树节点追踪（外部访问，不修改行为树节点）
        /// </summary>
        private void UpdateBehaviorTreeTracking(AgentBehaviorData data)
        {
            if (data.BehaviorTree == null) return;
            
            // 获取当前活跃任务
            string currentTask = GetActiveTaskName(data.BehaviorTree);
            
            // 如果任务变化了
            if (currentTask != data.CurrentActiveTaskName)
            {
                // 记录上一个任务的停留时间
                if (!string.IsNullOrEmpty(data.CurrentActiveTaskName))
                {
                    if (!data.TaskDurations.ContainsKey(data.CurrentActiveTaskName))
                        data.TaskDurations[data.CurrentActiveTaskName] = 0f;
                    data.TaskDurations[data.CurrentActiveTaskName] += data.TimeInCurrentTask;
                }
                
                data.LastActiveTaskName = data.CurrentActiveTaskName;
                data.CurrentActiveTaskName = currentTask;
                data.TimeInCurrentTask = 0f;
            }
            else
            {
                // 继续在当前任务停留
                data.TimeInCurrentTask += Time.deltaTime;
            }
        }

        /// <summary>
        /// 更新战斗参与数据
        /// </summary>
        private void UpdateCombatParticipation(AgentBehaviorData data, CombatContextManager context)
        {
            float combatRangeThreshold = 30f;
            bool hasEnemyInRange = false;
            
            // 检查是否有敌人在战斗范围内
            var enemies = data.Team == 0 ? context.EnemyTeamTrans : context.PlayerTeamTrans;
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                float dist = Vector3.Distance(data.AgentTransform.position, enemy.position);
                if (dist <= combatRangeThreshold)
                {
                    hasEnemyInRange = true;
                    break;
                }
            }
            
            if (hasEnemyInRange)
            {
                data.TimeInCombatRange += Time.deltaTime;
            }
            
            // 检查攻击范围
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                float dist = Vector3.Distance(data.AgentTransform.position, enemy.position);
                bool inAttackRange = dist <= data.MaxAttackRange;
                
                if (inAttackRange && !data.WasInAttackRange)
                {
                    data.TimesInAttackRange++;
                }
                data.WasInAttackRange = inAttackRange;
                break; // 只检查最近的敌人
            }
        }

        /// <summary>
        /// 结束Agent行为追踪并生成统计
        /// </summary>
        private void EndAgentBehaviorTracking()
        {
            float battleDuration = Time.time - _battleStartTime;
            if (battleDuration <= 0) battleDuration = 1f;
            
            int idleCount = 0;
            int slackingCount = 0;
            float totalActivity = 0f;
            
            // 构建合并的日志
            var teamLogBuilder = new System.Text.StringBuilder();
            var enemyLogBuilder = new System.Text.StringBuilder();
            teamLogBuilder.AppendLine("友方Agent行为分析:");
            enemyLogBuilder.AppendLine("敌方Agent行为分析:");
            
            foreach (var data in _agentBehaviorData.Values)
            {
                // 创建统计对象
                var stats = new AIAgentBehaviorStats
                {
                    AgentName = data.AgentName,
                    Team = data.Team,
                    StartPosition = data.StartPosition,
                    EndPosition = data.AgentTransform != null ? data.AgentTransform.position : data.LastPosition,
                    TotalDistanceMoved = data.TotalDistanceMoved,
                    StationaryPercentage = (data.TimeStationary / battleDuration) * 100f,
                    TimeInCombatRange = data.TimeInCombatRange,
                    CombatParticipationRate = (data.TimeInCombatRange / battleDuration) * 100f,
                    TimesInAttackRange = data.TimesInAttackRange
                };
                
                // 计算评分
                stats.ActivityScore = 100f - stats.StationaryPercentage;
                stats.CombatScore = stats.CombatParticipationRate;
                
                // 判断发呆/划水
                bool isIdling = stats.IsIdling;
                bool isSlacking = stats.IsSlacking;
                if (isIdling) idleCount++;
                if (isSlacking) slackingCount++;
                totalActivity += stats.ActivityScore;
                
                // 存储到战斗结果
                if (data.Team == 0)
                    _currentBattle.TeamBehaviorStats.Add(stats);
                else
                    _currentBattle.EnemyBehaviorStats.Add(stats);
                
                // 构建个体分析字符串
                string behaviorInfo = $"  {stats.GetBehaviorAnalysis()}";
                if (isIdling || isSlacking)
                {
                    // 获取发呆/划水时的行为树节点信息
                    string currentTask = data.CurrentActiveTaskName;
                    float taskTime = data.TimeInCurrentTask;
                    
                    behaviorInfo += $"\n    └─ 当前节点: {currentTask} ({taskTime:F1}s)";
                    
                    // 输出停留时间最长的节点（可能是卡住的原因）
                    if (data.TaskDurations.Count > 0)
                    {
                        var longestTask = data.TaskDurations.OrderByDescending(kvp => kvp.Value).First();
                        behaviorInfo += $", 最长停留: {longestTask.Key} ({longestTask.Value:F1}s)";
                    }
                }
                
                // 添加到对应的日志构建器
                if (data.Team == 0)
                    teamLogBuilder.AppendLine(behaviorInfo);
                else
                    enemyLogBuilder.AppendLine(behaviorInfo);
            }
            
            // 汇总数据
            _currentBattle.IdleAgentCount = idleCount;
            _currentBattle.SlackingAgentCount = slackingCount;
            _currentBattle.TeamAverageActivity = _agentBehaviorData.Count > 0 ? 
                totalActivity / _agentBehaviorData.Count : 0f;
            
            // 输出合并后的日志（每条包含多个Agent）
            Debug.Log($"[AGENT_BEHAVIOR_TEAM]\n{teamLogBuilder}");
            Debug.Log($"[AGENT_BEHAVIOR_ENEMY]\n{enemyLogBuilder}");
            
            // 输出团队摘要
            Debug.Log($"[BEHAVIOR_SUMMARY] 发呆Agent: {idleCount}, 划水Agent: {slackingCount}, " +
                $"团队平均活跃度: {_currentBattle.TeamAverageActivity:F1}%");
            
            _agentBehaviorData.Clear();
        }

        /// <summary>
        /// 结束对战记录
        /// </summary>
        public void EndBattleRecording(CombatStatu result)
        {
            if (!IsRecording) return;

            // 结束行为追踪
            EndAgentBehaviorTracking();

            _currentBattle.Result = result;
            _currentBattle.BattleDuration = Time.time - _battleStartTime;

            var context = CombatContextManager.Instance;

            // 统计友方数据
            foreach (var op in _teamOperatorsSnapshot)
            {
                var stats = new OperatorBattleStats
                {
                    OperatorName = op.OpInfo.Name,
                    MaxHP = op.MaxHP,
                    RemainingHP = Mathf.Max(0, op.CurrentHP),
                    IsDead = op.IsDead,
                DamageDealt = op.StatCauseDamage,
                DamageTaken = op.MaxHP - Mathf.Max(0, op.CurrentHP),
                KillCount = op.StatKillCount
                };
                _currentBattle.TeamOperatorStats.Add(stats);
                
                if (op.IsDead) _currentBattle.TeamCasualties++;
                _currentBattle.TeamTotalHP += Mathf.Max(0, op.CurrentHP);
            }

            // 统计敌方数据
            foreach (var op in _enemyOperatorsSnapshot)
            {
                var stats = new OperatorBattleStats
                {
                    OperatorName = op.OpInfo.Name,
                    MaxHP = op.MaxHP,
                    RemainingHP = Mathf.Max(0, op.CurrentHP),
                    IsDead = op.IsDead,
                    DamageDealt = op.StatCauseDamage,
                    DamageTaken = op.MaxHP - Mathf.Max(0, op.CurrentHP),
                    KillCount = op.StatKillCount
                };
                _currentBattle.EnemyOperatorStats.Add(stats);
                
                if (op.IsDead) _currentBattle.EnemyCasualties++;
                _currentBattle.EnemyTotalHP += Mathf.Max(0, op.CurrentHP);
            }

            // 添加到会话
            _currentSession.BattleResults.Add(_currentBattle);
            IsRecording = false;

            // 输出结果（MCP可以通过读取Unity Console获取）
            OutputBattleResult(_currentBattle);

            OnBattleEnd?.Invoke(_currentBattle);

            if (TrainingConfig && TrainingConfig.VerboseLogging)
            {
                Debug.Log($"[AITraining] 对战结束: {result}, 得分: {_currentBattle.VictoryScore:F1}");
            }
        }

        /// <summary>
        /// 结束整个训练会话
        /// </summary>
        public TrainingSessionResult EndTrainingSession()
        {
            if (!IsTrainingMode) return null;

            _currentSession.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            OutputSessionResult(_currentSession);
            
            OnSessionEnd?.Invoke(_currentSession);
            
            IsTrainingMode = false;
            
            return _currentSession;
        }

        /// <summary>
        /// 应用行为树到指定单位
        /// </summary>
        public void ApplyBehaviorTree(Transform operatorTransform, ExternalBehaviorTree behaviorTree)
        {
            if (behaviorTree == null) return;

            var behaviorTreeComponent = operatorTransform.GetComponent<BehaviorTree>();
            if (behaviorTreeComponent == null)
            {
                behaviorTreeComponent = operatorTransform.gameObject.AddComponent<BehaviorTree>();
            }

            // 保存原始行为树
            if (!_originalBehaviorTrees.ContainsKey(operatorTransform))
            {
                _originalBehaviorTrees[operatorTransform] = behaviorTreeComponent.ExternalBehavior as ExternalBehaviorTree;
            }

            behaviorTreeComponent.ExternalBehavior = behaviorTree;
        }

        /// <summary>
        /// 恢复原始行为树
        /// </summary>
        public void RestoreOriginalBehaviorTree(Transform operatorTransform)
        {
            if (_originalBehaviorTrees.TryGetValue(operatorTransform, out var original))
            {
                var behaviorTreeComponent = operatorTransform.GetComponent<BehaviorTree>();
                if (behaviorTreeComponent != null)
                {
                    behaviorTreeComponent.ExternalBehavior = original;
                }
            }
        }

        /// <summary>
        /// 获取训练进度
        /// </summary>
        public string GetTrainingProgress()
        {
            if (_currentSession == null) return "No active session";
            
            return $"Session: {_currentSession.SessionId}, " +
                   $"Battles: {_currentSession.BattleResults.Count}, " +
                   $"WinRate: {_currentSession.WinRate:P1}, " +
                   $"AvgScore: {_currentSession.AverageVictoryScore:F1}";
        }

        /// <summary>
        /// 输出对战结果到Console（JSON格式，便于MCP解析）
        /// </summary>
        private void OutputBattleResult(TrainingBattleResult result)
        {
            // 使用特定标记便于MCP识别
            string json = JsonUtility.ToJson(result);
            Debug.Log($"[TRAINING_RESULT_BATTLE] {json}");
        }

        /// <summary>
        /// 输出会话结果到Console
        /// </summary>
        private void OutputSessionResult(TrainingSessionResult result)
        {
            string json = JsonUtility.ToJson(result);
            Debug.Log($"[TRAINING_RESULT_SESSION] {json}");
            
            // 同时输出人类可读的摘要
            Debug.Log($"[TRAINING_SUMMARY]\n{result.GetSummary()}");
        }

        /// <summary>
        /// 强制结束当前对战（超时等情况）
        /// </summary>
        public void ForceEndBattle(CombatStatu result)
        {
            if (IsRecording)
            {
                EndBattleRecording(result);
            }
        }

        /// <summary>
        /// 检查是否超时，并更新行为追踪
        /// </summary>
        private void Update()
        {
            if (!IsRecording || TrainingConfig == null) return;

            // 更新Agent行为追踪数据
            UpdateAgentBehaviorTracking();

            float elapsed = Time.time - _battleStartTime;
            if (elapsed > TrainingConfig.MaxBattleDuration)
            {
                Debug.LogWarning($"[AITraining] 对战超时 ({elapsed:F1}s)，强制结束");
                ForceEndBattle(CombatStatu.Loss); // 超时视为失败
            }
        }

        /// <summary>
        /// 批量训练：依次对战多个对手
        /// </summary>
        public IEnumerator RunBatchTraining(List<ExternalBehaviorTree> opponents, int roundsPerOpponent = 3)
        {
            if (!IsTrainingMode || TrainingConfig == null)
            {
                Debug.LogError("[AITraining] 未初始化训练模式");
                yield break;
            }

            string teamBehaviorName = TrainingConfig.TeamBehaviorTree ? 
                TrainingConfig.TeamBehaviorTree.name : "Current";

            foreach (var opponent in opponents)
            {
                if (opponent == null) continue;

                for (int i = 0; i < roundsPerOpponent; i++)
                {
                    Debug.Log($"[AITraining] 开始第 {i+1}/{roundsPerOpponent} 轮对战: " +
                        $"{teamBehaviorName} vs {opponent.name}");

                    // 通知GameLevelManager切换敌方行为树
                    // 这里需要通过事件或回调来实现
                    yield return new WaitForSeconds(0.5f);
                    
                    // 等待对战结束...
                    // 实际逻辑由GameLevelManager控制
                }
            }

            EndTrainingSession();
        }
    }
}
