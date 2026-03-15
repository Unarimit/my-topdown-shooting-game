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
    /// 训练结果数据，用于记录每场对战的结果（精简CSV格式）
    /// </summary>
    [Serializable]
    public class TrainingBattleResult
    {
        // CSV核心字段（12个）
        public string ConfigName;
        public string Timestamp;
        public string TeamBehaviorTreeName;
        public string EnemyBehaviorTreeName;
        public CombatStatu Result;
        public float BattleDuration;
        public int TeamCasualties;
        public int EnemyCasualties;
        public int TeamTotalHP;
        public int EnemyTotalHP;
        public int IdleAgentCount;
        public int SlackingAgentCount;
        public float TeamAverageActivity;

        // 内部计算用（不输出到CSV）
        [NonSerialized] public int TeamMaxTotalHP;
        [NonSerialized] public int EnemyMaxTotalHP;
        [NonSerialized] public List<OperatorBattleStats> TeamOperatorStats = new List<OperatorBattleStats>();
        [NonSerialized] public List<OperatorBattleStats> EnemyOperatorStats = new List<OperatorBattleStats>();
        [NonSerialized] public List<AIAgentBehaviorStats> TeamBehaviorStats = new List<AIAgentBehaviorStats>();
        [NonSerialized] public List<AIAgentBehaviorStats> EnemyBehaviorStats = new List<AIAgentBehaviorStats>();

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
    /// 单个干员的战斗统计（精简版）
    /// </summary>
    [Serializable]
    public class OperatorBattleStats
    {
        public string OperatorName;
        public int RemainingHP;
        public bool IsDead;
        public int DamageDealt;
        public int DamageTaken;
    }

    /// <summary>
    /// AI Agent行为统计数据（完整版）
    /// </summary>
    [Serializable]
    public class AIAgentBehaviorStats
    {
        // 基础信息
        public string AgentName;
        public int Team;
        public bool IsDead;

        // 伤害统计
        public int DamageDealt;
        public int DamageTaken;

        // 移动统计
        public float TotalDistanceMoved;
        public float StationaryPercentage;

        // 战斗参与
        public float CombatParticipationRate;
        public int TimesInAttackRange;

        // 评分
        public float ActivityScore;
        public float CombatScore;

        public bool IsIdling => StationaryPercentage > 50f && CombatParticipationRate < 20f;
        public bool IsSlacking => ActivityScore < 30f || CombatScore < 20f;

        public string GetBehaviorAnalysis()
        {
            var state = IsIdling ? "[发呆] " : IsSlacking ? "[划水] " : "[正常] ";
            return $"{AgentName}: {state}移动{TotalDistanceMoved:F1}m 静止{StationaryPercentage:F1}% 参与率{CombatParticipationRate:F1}%";
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