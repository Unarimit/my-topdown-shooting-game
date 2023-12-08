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

        // TODO: 在数据未持久化之前的临时措施
        public static Dictionary<string, Texture> IconMap = new Dictionary<string, Texture>();
        public static Texture LoadIcon(string modelUrl)
        {
            if (IconMap.ContainsKey(modelUrl)) return IconMap[modelUrl];
            else return null;
        }
        public static void AddIcon(string modelUrl, Texture texture)
        {
            if (IconMap.ContainsKey(modelUrl)) return;
            else IconMap.Add(modelUrl, texture);
        }
    }
}
