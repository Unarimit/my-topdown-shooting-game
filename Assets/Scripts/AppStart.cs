using UnityEngine;

namespace Assets.Scripts
{
    public class AppStart
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeBeforeSceneLoad()
        {
            // 此处代码会在首场景加载前执行
            Debug.Log("App Start, loading something needs preload");
            // 示例：创建常驻对象
            MyServices.LoadOnAppStart();
        }
    }
}