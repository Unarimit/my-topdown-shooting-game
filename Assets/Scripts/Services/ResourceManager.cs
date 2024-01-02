using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    internal static class ResourceManager
    {
        public static T Load<T>(string resName) where T : Object
        {
            var res = Resources.Load<T>(resName);
            if (res == null)
            {
                Debug.LogError($"ResourceManager can not find {resName}");
            }
            return res;
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
