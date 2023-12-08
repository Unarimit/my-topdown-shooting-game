using Assets.Scripts.Entities;
using Assets.Scripts.Services;
using Assets.Scripts.Services.Interface;

namespace Assets.Scripts
{
    /// <summary>
    /// 服务提供者（定位器）
    /// </summary>
    internal static class MyServices
    {
        public static IGameDatabase Database { get; }
        static MyServices()
        {
            // 测试使用
            Database = new TestDatabase();
        }
    }
}
