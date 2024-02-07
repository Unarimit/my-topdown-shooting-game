using UnityEngine;
using UnityEngine.SceneManagement;
using XLua;

namespace Assets.Scripts.StartLogic
{
    internal class StartSceneStartup : MonoBehaviour
    {
        [SerializeField]
        TextAsset luaLevelScript;
        [SerializeField]
        TextAsset luaSkillScript;
        private void Awake()
        {
            // 加载背景（根据一些存档信息）
            SceneManager.LoadScene("HomeBackground", new LoadSceneParameters(LoadSceneMode.Additive));

            // 加载lua脚本 (添加关卡和技能）
            luaLogicLevelInject();
            luaLogicSkillInject();

        }

        private void luaLogicLevelInject()
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
            luaEnv.DoString(luaLevelScript.text, "StartupLua", scriptEnv);
        }

        private void luaLogicSkillInject() 
        {
            var luaEnv = MyServices.LuaEnv;
            // 不配置独立环境，使其作为全局变量，可以被技能调用
            // 实际上这里可以配置table，再把table放到MyServices或Lua相关的静态类中，这样比较好。但不想那么麻烦了捏
            luaEnv.DoString(luaSkillScript.text);

        }

    }
}
