﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Entities.Level
{
    public enum MapType
    {
        Small, Middle, Big, Invasion
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
        public Dropout[] Dropouts;
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
        public Dropout[] Dropouts;
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
    public struct Dropout
    {
        /// <summary>
        /// 掉落物体
        /// </summary>
        public readonly GameItem DropItem;
        /// <summary>
        /// 掉落最小数量 计算公式为 if(Possible > Random(0, 1)) Random.Range(AmountMin, AmountMax)
        /// </summary>
        public readonly int AmountMin;
        public readonly int AmountMax;
        public readonly float Possible;

        public Dropout(GameItem dropItem, int amountMin = 1, int amountMax = 1, float possible = 1)
        {
            DropItem = dropItem;
            AmountMin = amountMin;
            AmountMax = amountMax;
            Possible = possible;
        }

        public int GetDropoutAmount()
        {
            if (Possible > Random.Range(0f, 1.0f))
            {
                return Random.Range(AmountMin, AmountMax);
            }
            else return 0;
        }
    }
    /// <summary>
    /// 关卡生成规则
    /// </summary>
    public class CombatLevelRule : LevelRule
    {
        public CombatLevelRule()
        {
            JumpScene = Services.MyConfig.Scene.Prepare;
        }
        /// <summary>
        /// 地图大小类型
        /// </summary>
        public MapType MapType;
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
        /// 是否允许家园建筑加入战斗
        /// </summary>
        public bool AllowHomeBuilding;
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
