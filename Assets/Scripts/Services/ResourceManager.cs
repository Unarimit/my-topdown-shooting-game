using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    internal static class ResourceManager
    {
        public static T Load<T>(string resName) where T : Object
        {
            var res = Resources.Load<T>(resName);
            if(res == null)
            {
                Debug.LogError($"ResourceManager can not find {resName}");
            }
            return res;
        }
    }
}
