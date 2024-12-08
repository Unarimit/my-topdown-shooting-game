using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    internal static class ResourceManager
    {
        public static T Load<T>(string resPath) where T : Object
        {
            var res = Resources.Load<T>(resPath);
            if (res == null)
            {
                Debug.LogError($"ResourceManager can not find {resPath}");
            }
            return res;
        }

        public static GameObject LoadGoAndInstantiate(string resPath, Transform parent)
        {
            var res = Resources.Load<GameObject>(resPath);
            if (res == null)
            {
                Debug.LogError($"ResourceManager can not find {resPath}");
                return null;
            }
            return GameObject.Instantiate(res, parent);
        }

        /// <summary>
        /// 尝试加载，路径为空不会保存，会返回null
        /// </summary>
        public static T TryLoad<T>(string resName) where T : Object
        {
            return Resources.Load<T>(resName);
        }

    }
}
