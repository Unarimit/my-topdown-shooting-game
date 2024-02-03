using UnityEngine;
using UnityEngine.SceneManagement;
using XLua;
using XLuaTest;

namespace Assets.Scripts.StartLogic
{
    internal class StartSceneStartup : MonoBehaviour
    {
        [SerializeField]
        TextAsset luaScript;
        private void Awake()
        {
            // 加载背景（根据一些存档信息）
            SceneManager.LoadScene("HomeBackground", new LoadSceneParameters(LoadSceneMode.Additive));

            // 加载lua脚本 (添加关卡和技能）
            luaLogicInject();

        }

        private void luaLogicInject()
        {
            var luaEnv = MyServices.LuaEnv;
            // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
            var scriptEnv = luaEnv.NewTable();
            LuaTable meta = luaEnv.NewTable();
            meta.Set("__index", luaEnv.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            // add injections
            scriptEnv.Set("self", this);
            scriptEnv.Set("_database", MyServices.Database);

            // do Lua logic
            luaEnv.DoString(luaScript.text, "StartupLua", scriptEnv);
        }

    }
}
