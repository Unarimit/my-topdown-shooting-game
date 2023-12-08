using Assets.Scripts.Entities;
using Assets.Scripts.Services;
using Assets.Scripts.Services.Interface;

namespace Assets.Scripts
{
    internal static class MyServices
    {
        public static IGameDatabase Database { get; }
        static MyServices()
        {
            Database = new TestDatabase();
        }
    }
}
