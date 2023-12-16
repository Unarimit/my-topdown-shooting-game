using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    /// <summary>
    /// Item帮助类
    /// </summary>
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
        /// <summary> 获取指定itemId的Texture </summary>
        public static Texture GetItemTexture(string itemId)
        {
            if (itemId == null || _itemMap.ContainsKey(itemId) is false)
            {
                Debug.LogWarning($"没有找到指定的Item Texture id: {itemId}");
                return null;
            }
            return ResourceManager.Load<Texture2D>(_itemMap[itemId].IconUrl);
        }
        /// <summary> 获取指定itemId的信息 </summary>
        public static GameItem GetItem(string itemId)
        {
            if(itemId == null || _itemMap.ContainsKey(itemId) is false)
            {
                Debug.LogWarning($"没有找到指定的Item Texture id: {itemId}");
                return null;
            }
            return _itemMap[itemId];
        }
    }
}
