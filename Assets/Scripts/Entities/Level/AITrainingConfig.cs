using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Entities.Level
{
    /// <summary>
    /// AI训练配置，用于定义训练场景和双方阵容差距
    /// </summary>
    [CreateAssetMenu(fileName = "AITrainingConfig", menuName = "Combat/AI Training Config")]
    public class AITrainingConfig : ScriptableObject
    {
        [Header("训练基本配置")]
        [Tooltip("配置名称")]
        public string ConfigName = "Default Training";

        [Tooltip("是否使用AI模式接管第0个单位（友军队长）")]
        public bool UseAIForPlayerSlot = true;

        [Tooltip("是否显示详细训练日志")]
        public bool VerboseLogging = true;

        [Tooltip("每场对战的最大时长（秒），超过则强制结束")]
        public float MaxBattleDuration = 120f;

        [Header("地图配置")]
        [Tooltip("地图类型")]
        public MapType MapType = MapType.Small;

        [Header("友方阵容配置")]
        [Tooltip("友方干员数量")]
        public int TeamOperatorCount = 5;

        [Tooltip("友方干员等级偏移（相对于基础等级）")]
        public int TeamLevelOffset = 0;

        [Tooltip("友方装备等级偏移")]
        public int TeamEquipmentOffset = 0;

        [Header("敌方阵容配置")]
        [Tooltip("敌方干员数量")]
        public int EnemyOperatorCount = 5;

        [Tooltip("敌方干员等级偏移（相对于基础等级，正值表示更强）")]
        public int EnemyLevelOffset = 0;

        [Tooltip("敌方装备等级偏移")]
        public int EnemyEquipmentOffset = 0;

        [Tooltip("敌方AI进攻性")]
        public bool EnemyAiAggressive = true;

        [Header("行为树配置")]
        [Tooltip("友方使用的行为树（训练目标）")]
        public ExternalBehaviorTree TeamBehaviorTree;

        [Tooltip("敌方使用的行为树（对手）")]
        public ExternalBehaviorTree EnemyBehaviorTree;

        [Header("测试对手配置（用于多对手验证）")]
        [Tooltip("是否在训练中使用多个对手类型进行验证")]
        public bool UseMultipleOpponents = true;

        [Tooltip("历史最优行为树（用于对比）")]
        public ExternalBehaviorTree BestBehaviorTree;

        [Tooltip("原始基础行为树（用于对比）")]
        public ExternalBehaviorTree BaseBehaviorTree;
    }

    /// <summary>
    /// 训练结果数据，用于记录每场对战的结果
    /// </summary>
    [Serializable]
    public class TrainingBattleResult
    {
        public string ConfigName;
        public string Timestamp;
        public string TeamBehaviorTreeName;
        public string EnemyBehaviorTreeName;
        public CombatStatu Result;
        public float BattleDuration;
        public int TeamCasualties;  // 友方阵亡数
        public int EnemyCasualties; // 敌方阵亡数
        public int TeamTotalHP;     // 友方剩余总血量
        public int EnemyTotalHP;    // 敌方剩余总血量
        public int TeamMaxTotalHP;  // 友方最大总血量
        public int EnemyMaxTotalHP; // 敌方最大总血量
        
        // 详细数据
        public List<OperatorBattleStats> TeamOperatorStats = new List<OperatorBattleStats>();
        public List<OperatorBattleStats> EnemyOperatorStats = new List<OperatorBattleStats>();
        
        // AI行为详细统计 (新增)
        public List<AIAgentBehaviorStats> TeamBehaviorStats = new List<AIAgentBehaviorStats>();
        public List<AIAgentBehaviorStats> EnemyBehaviorStats = new List<AIAgentBehaviorStats>();
        
        // 行为分析摘要
        public int IdleAgentCount;      // 发呆agent数量
        public int SlackingAgentCount;  // 划水agent数量
        public float TeamAverageActivity;   // 团队平均活跃度

        public float VictoryScore => CalculateVictoryScore();

        private float CalculateVictoryScore()
        {
            if (Result != CombatStatu.Win) return 0f;
            
            // 胜利分数 = 基础胜利分 + 存活奖励 + 血量奖励 + 快速胜利奖励
            float score = 100f;
            
            // 存活奖励：每个存活单位+10分
            int teamSurvivors = TeamOperatorStats.Count - TeamCasualties;
            score += teamSurvivors * 10f;
            
            // 血量奖励：按剩余血量百分比加分
            float teamHpRatio = TeamMaxTotalHP > 0 ? (float)TeamTotalHP / TeamMaxTotalHP : 0;
            score += teamHpRatio * 50f;
            
            // 快速胜利奖励：战斗时间越短越好（最多30分）
            // 每秒扣0.25分，最多扣30分
            int timeDeduction = (int)(BattleDuration / 4); // BattleDuration / 4 = BattleDuration * 0.25
            int timeBonus = 30 - timeDeduction;
            if (timeBonus < 0) timeBonus = 0;
            score += timeBonus;
            
            return score;
        }
    }

    /// <summary>
    /// 单个干员的战斗统计
    /// </summary>
    [Serializable]
    public class OperatorBattleStats
    {
        public string OperatorName;
        public int MaxHP;
        public int RemainingHP;
        public bool IsDead;
        public int DamageDealt;
        public int DamageTaken;
        public int KillCount;
    }

    /// <summary>
    /// AI Agent详细行为统计数据
    /// 用于检测发呆、挂机等问题
    /// </summary>
    [Serializable]
    public class AIAgentBehaviorStats
    {
        // 基础信息
        public string AgentName;
        public int Team; // 0=友方, 1=敌方
        public bool IsDead;
        
        // 位置相关 - 检测发呆
        public float TotalDistanceMoved;      // 总移动距离
        public float AverageSpeed;            // 平均速度
        public float TimeStationary;          // 静止时间（秒）
        public float StationaryPercentage;    // 静止时间占比
        public Vector3 StartPosition;         // 起始位置
        public Vector3 EndPosition;           // 结束位置
        
        // 战斗参与 - 检测划水
        public float TimeInCombatRange;       // 在战斗范围内的时长
        public float CombatParticipationRate; // 战斗参与率
        public int TimesInAttackRange;        // 进入攻击范围次数
        public float TotalAimTime;            // 瞄准总时长
        
        // 决策活跃度
        public int DecisionChanges;           // 行为树决策改变次数
        public float AverageDecisionInterval; // 平均决策间隔
        public string LastActionName;         // 最后执行的动作
        
        // 行为评分
        public float ActivityScore;           // 活跃度评分 (0-100)
        public float CombatScore;             // 战斗参与评分 (0-100)
        public float EfficiencyScore;         // 效率评分 (0-100)
        
        /// <summary>
        /// 是否被判定为"发呆"
        /// </summary>
        public bool IsIdling => StationaryPercentage > 50f && CombatParticipationRate < 20f;
        
        /// <summary>
        /// 是否被判定为"划水"
        /// </summary>
        public bool IsSlacking => ActivityScore < 30f || CombatScore < 20f;
        
        public string GetBehaviorAnalysis()
        {
            var analysis = $"{AgentName}: ";
            if (IsIdling) analysis += "[发呆] ";
            else if (IsSlacking) analysis += "[划水] ";
            else analysis += "[正常] ";
            
            analysis += $"移动{TotalDistanceMoved:F1}m 静止{StationaryPercentage:F1}% 参与率{CombatParticipationRate:F1}%";
            return analysis;
        }
    }

    /// <summary>
    /// 训练会话结果，包含多轮对战
    /// </summary>
    [Serializable]
    public class TrainingSessionResult
    {
        public string SessionId;
        public string StartTime;
        public string EndTime;
        public string BehaviorTreeVersion;
        public List<TrainingBattleResult> BattleResults = new List<TrainingBattleResult>();
        
        public float AverageVictoryScore => BattleResults.Count > 0 
            ? (float)BattleResults.Average(r => r.VictoryScore) 
            : 0f;
        
        public float WinRate => BattleResults.Count > 0
            ? (float)BattleResults.Count(r => r.Result == CombatStatu.Win) / BattleResults.Count
            : 0f;
        
        public float AverageTeamCasualties => BattleResults.Count > 0
            ? (float)BattleResults.Average(r => r.TeamCasualties)
            : 0f;
        
        // 行为统计摘要
        public int TotalIdleAgents => BattleResults.Sum(b => b.IdleAgentCount);
        public int TotalSlackingAgents => BattleResults.Sum(b => b.SlackingAgentCount);
        public float AverageTeamActivity => BattleResults.Count > 0 
            ? BattleResults.Average(b => b.TeamAverageActivity) 
            : 0f;

        public string GetSummary()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== 训练会话结果 [{SessionId}] ===");
            sb.AppendLine($"行为树版本: {BehaviorTreeVersion}");
            sb.AppendLine($"测试场次: {BattleResults.Count}");
            sb.AppendLine($"胜率: {WinRate:P1}");
            sb.AppendLine($"平均胜利分: {AverageVictoryScore:F1}");
            sb.AppendLine($"平均友方阵亡: {AverageTeamCasualties:F1}");
            sb.AppendLine("详细结果:");
            foreach (var battle in BattleResults)
            {
                sb.AppendLine($"  vs {battle.EnemyBehaviorTreeName}: {battle.Result} " +
                    $"(伤亡 {battle.TeamCasualties}/{battle.EnemyCasualties}, " +
                    $"得分 {battle.VictoryScore:F1})");
            }
            return sb.ToString();
        }
    }
}