using System.Collections.Generic;

namespace Assets.Scripts.Entities.Level
{
    public enum CombatStatu
    {
        Ing,
        Win,
        Loss
    }
    /// <summary>
    /// 非数据类，用于传递结算用的实时信息
    /// </summary>
    public class CombatLevelResult
    {
        /// <summary>
        /// 参与的战斗规则
        /// </summary>
        public CombatLevelRule LevelRule {get; set;}
        /// <summary>
        /// 战斗状态
        /// </summary>
        public CombatStatu CombatStatu { get; set; } = CombatStatu.Ing;
        /// <summary> 战利品 </summary>
        public Dictionary<string, int> Loot { get; set; } = new Dictionary<string, int>();
        /// <summary> 参与战斗的干员 </summary>
        public List<Operator> JoinOperator { get; set; } = new List<Operator>();

    }
}
