using Assets.Scripts.HomeLogic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.StartLogic
{
    internal class StartSceneStartup : MonoBehaviour
    {
        Scene homeScene;
        private void Awake()
        {
            homeScene = SceneManager.LoadScene("HomeBackground", new LoadSceneParameters(LoadSceneMode.Additive));
            XLua.LuaEnv luaenv = new XLua.LuaEnv();
            luaenv.DoString("CS.UnityEngine.Debug.Log('hello world')");
            luaenv.Dispose();
        }

    }
}
