using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    [CreateAssetMenu(fileName = "NewItemList", menuName = "Create ItemList")]
    internal class ItemScriptableObject : ScriptableObject
    {
        public List<GameItem> GameItems;
    }
}
