using Assets.Scripts.Entities.Buildings;
using System;

namespace Assets.Scripts.Entities.Save
{
    /// <summary>
    /// 存档摘要类
    /// </summary>
    [Serializable]
    internal class SaveAbstract
    {
        /// <summary> 存档id </summary>
        public string SaveId;
        /// <summary> 存档名称 </summary>
        public string SaveName;
        /// <summary> 存档时间 </summary>
        public DateTime SaveTime;
        /// <summary> 存档描述 </summary>
        public string SaveDesc;
        /// <summary> 存档游戏天数 </summary>
        public int SaveDay;

        public Produce[] Resource;
    }
}
