using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public enum MapSize
    {
        Small,Middle,Big
    }
    public enum InitPosition
    {
        EnemySpawnCenter, EnemySpawnScatter, MapScatter
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
        /// <summary>
        /// 初始化位置
        /// </summary>
        public InitPosition InitPosition;
    }
    public class InteractablePrefab
    {
        /// <summary>
        /// 物体Id
        /// </summary>
        public string ObjectId;
        /// <summary>
        /// 互动提示词
        /// </summary>
        public string InteractTip;
        /// <summary>
        /// 互动时间
        /// </summary>
        public float Duration;
        /// <summary>
        /// 最小数量
        /// </summary>
        public int MinAmount;
        /// <summary>
        /// 最大数量
        /// </summary>
        public int MaxAmount;
        /// <summary>
        /// 物体模型路径
        /// </summary>
        public string ModelUrl;
        /// <summary>
        /// 掉落
        /// </summary>
        public KeyValuePair<string, int>[] Dropouts;
        /// <summary>
        /// 初始化位置
        /// </summary>
        public InitPosition InitPosition;
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
        /// 可互动物品原型
        /// </summary>
        public InteractablePrefab[] InteractablePrefabs;
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
        /// <summary>
        /// 敌方出生点
        /// </summary>
        public RectInt EnemySpawn;
        /// <summary>
        /// 我出生点
        /// </summary>
        public RectInt TeamSpawn;
    }
}
