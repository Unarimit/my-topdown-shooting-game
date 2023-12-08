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
        public List<Operator> Operators { get; }

        /// <summary>
        /// 玩家拥有的机甲
        /// </summary>
        public List<MechaBase> Mechas { get; }

        /// <summary>
        /// 所有关卡规则
        /// </summary>
        public List<LevelRule> LevelRules { get; }

        // skill 和 item


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
