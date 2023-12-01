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
        public static Dictionary<string, Texture> HeadIconMap = new Dictionary<string, Texture>();
        public static Texture LoadModelHeadIcon(string modelUrl)
        {
            if (HeadIconMap.ContainsKey(modelUrl)) return HeadIconMap[modelUrl];
            else return null;
        }
        public static void AddModelHeadIcon(string modelUrl, Texture texture)
        {
            if (HeadIconMap.ContainsKey(modelUrl)) return;
            else HeadIconMap.Add(modelUrl, texture);
        }
    }
}
