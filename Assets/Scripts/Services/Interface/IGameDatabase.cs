using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.Entities.Mechas;
using System.Collections.Generic;

namespace Assets.Scripts.Services.Interface
{
    internal interface IGameDatabase
    {
        #region 和单个存档相关的数据
        /// <summary> 玩家拥有的Op </summary>
        public IList<Operator> Operators { get; }

        /// <summary> 玩家拥有的机甲 </summary>
        public IList<MechaBase> Mechas { get; }

        /// <summary> 仓库（itemId, amount) </summary>
        public IDictionary<string, int> Inventory { get; }

        /// <summary> 家园的建筑区域记录 </summary>
        public BuildingArea BuildingArea { get; }

        /// <summary> 当前关卡信息 </summary>
        public LevelInfo CurLevel { get; set; }
        #endregion

        #region 游戏配置数据
        /// <summary> 所有关卡规则 </summary>
        public IList<LevelRule> LevelRules { get; }
        /// <summary> 所有建筑信息 </summary>
        public IList<Building> Buildings { get; }

        // 还差skill，但skill是否需要存入数据库呢？
        // A: 需要的，因为skill数据也将随启动流程加载，尽量统一管理

        /// <summary> 当前加载的模型信息 </summary>
        public List<string> ModelList { get; }
        #endregion
    }
}
