using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Mechas;
using System.Collections.Generic;

namespace Assets.Scripts.Services.Interface
{
    internal interface IGameDatabase
    {

        /// <summary>
        /// 玩家拥有的Op
        /// </summary>
        public IList<Operator> Operators { get; }

        /// <summary>
        /// 玩家拥有的机甲
        /// </summary>
        public IList<MechaBase> Mechas { get; }

        /// <summary>
        /// 所有关卡规则
        /// </summary>
        public IList<LevelRule> LevelRules { get; }

        /// <summary>
        /// 仓库（itemId, amount)
        /// </summary>
        public IDictionary<string, int> Inventory { get; }

        // 还差skill，但skill是否需要存入数据库呢？


        /// <summary>
        /// 当前关卡信息
        /// </summary>
        public LevelInfo CurLevel { get; set; }

        /// <summary>
        /// 当前加载的模型信息
        /// </summary>
        public List<string> ModelList { get; }

    }
}
