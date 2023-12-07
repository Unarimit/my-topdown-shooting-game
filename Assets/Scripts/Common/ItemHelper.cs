using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    internal static class ItemHelper
    {
        private static Dictionary<string, GameItem> _itemMap;
        static ItemHelper()
        {
            _itemMap = new Dictionary<string, GameItem>();
            var items = ResourceManager.Load<ItemScriptableObject>("ItemList");
            foreach(var x in items.GameItems)
            {
                _itemMap.Add(x.ItemId, x);
            }
        }

        public static Texture GetItemTexture(string item)
        {
            if (item == null || _itemMap.ContainsKey(item) is false)
            {
                Debug.LogWarning($"没有找到指定的Item Texture id: {item}");
                return null;
            }
            return ResourceManager.Load<Texture2D>(_itemMap[item].IconUrl);
        }

        public static GameItem GetItem(string item)
        {
            if(item == null || _itemMap.ContainsKey(item) is false)
            {
                Debug.LogWarning($"没有找到指定的Item Texture id: {item}");
                return null;
            }
            return _itemMap[item];
        }
    }
}
