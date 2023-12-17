using Assets.Scripts.Common;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic.BagUIs
{
    internal class BagScrollViewUI : MonoBehaviour
    {
        public GameObject m_ItemPrefab;
        public void Inject(IDictionary<string, int> inventory)
        {
            var contentTrans = transform.Find("Viewport").Find("Content");
            foreach (var s in inventory)
            {
                var x = ItemHelper.GetItem(s.Key);
                if (x.IsDisplay is false) continue;

                var go = Instantiate(m_ItemPrefab, contentTrans);
                go.SetActive(true);
                go.GetComponent<BagScrollViewItemUI>().Inject(x, s.Value);
            }
        }
    }
}
