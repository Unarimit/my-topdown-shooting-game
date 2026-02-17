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

        // 行为树引用缓存
        private Dictionary<Transform, ExternalBehaviorTree> _originalBehaviorTrees = new Dictionary<Transform, ExternalBehaviorTree>();

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

            if (TrainingConfig && TrainingConfig.VerboseLogging)
            {
                Debug.Log($"[AITraining] 开始记录对战: {teamBehaviorName} vs {enemyBehaviorName}");
            }
        }

        /// <summary>
        /// 结束对战记录
        /// </summary>
        public void EndBattleRecording(CombatStatu result)
        {
            if (!IsRecording) return;

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
        /// 检查是否超时
        /// </summary>
        private void Update()
        {
            if (!IsRecording || TrainingConfig == null) return;

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