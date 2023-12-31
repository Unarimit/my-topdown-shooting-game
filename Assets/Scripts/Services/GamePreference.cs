
namespace Assets.Scripts.Services
{
    /// <summary>
    /// 环境设置菜单的属性
    /// </summary>
    internal static class GamePreference
    {
        // *************  表现设置  ***************
        /// <summary> 是否允许角色布料模拟 </summary>
        public static bool UseMagicaBone { get; set; } = true;

        public static bool UseBloom { get;set; } = true;

        // *************  互动设置  ***************

        /// <summary> 抽卡长按时间 </summary>
        public static float GachaPressTime { get; set; } = 3;
    }
}
