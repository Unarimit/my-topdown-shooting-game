using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities
{
    public enum MapSize
    {
        Small,Middle,Big
    }
    public struct OperatorPrefab
    {
        /// <summary>
        /// 人物信息
        /// </summary>
        public Operator OpInfo;
        /// <summary>
        /// 最小数量
        /// </summary>
        public int MinAmount;
        /// <summary>
        /// 最大数量
        /// </summary>
        public int MaxAmount;
        /// <summary>
        /// 装甲升级系数
        /// </summary>
        public int MechaRandomUpgradeFactor;
        /// <summary>
        /// 使用随机的角色模型
        /// </summary>
        public bool UseRandomCModel;
        /// <summary>
        /// 掉落
        /// </summary>
        public KeyValuePair<string, int>[] Dropouts;
        /// <summary>
        /// AI进攻性
        /// </summary>
        public bool AiAgressive;
        //TODO: 摆放位置
    }
    public struct Condition
    {
        /// <summary>
        /// 要求满足的item
        /// </summary>
        public string ItemName;
        /// <summary>
        /// 要求满足的数量
        /// </summary>
        public int Amount;
        /// <summary>
        /// 描述，使用{0}缺省表示进度信息
        /// </summary>
        public string Description;
    }
    /// <summary>
    /// 关卡生成规则
    /// </summary>
    public class LevelRule
    {
        /// <summary>
        /// 关卡名称
        /// </summary>
        public string LevelName;
        /// <summary>
        /// 关卡描述
        /// </summary>
        public string Description;
        /// <summary>
        /// 地图大小类型
        /// </summary>
        public MapSize MapSize;
        /// <summary>
        /// 敌方原型
        /// </summary>
        public OperatorPrefab[] OperatorPrefabs;
        /// <summary>
        /// 获胜条件
        /// </summary>
        public Condition[] WinCondition;
        /// <summary>
        /// 失败条件
        /// </summary>
        public Condition[] LossCondition;
        /// <summary>
        /// 是否允许复活
        /// </summary>
        public bool AllowRespawn;
        /// <summary>
        /// 友方索敌阈值
        /// </summary>
        public float TeamAttackThreshold;
        /// <summary>
        /// 敌方索敌阈值
        /// </summary>
        public float EnemyAttackThreshold;
    }
}
