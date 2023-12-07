
using Assets.Scripts.Common;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.UILogic.CombatSummaryUIs
{
    internal class CsDropoutScrollViewUI : MonoBehaviour
    {
        public GameObject m_ItemPrefab;
        public void Inject(Dictionary<string, int> dropouts)
        {
            var contentTrans = transform.Find("Viewport").Find("Content");
            foreach (var s in dropouts)
            {
                var x = ItemHelper.GetItem(s.Key);
                if (x.IsDisplay is false) continue;

                var go = Instantiate(m_ItemPrefab, contentTrans);
                go.SetActive(true);
                go.GetComponent<CsDropoutScrollViewItemUI>().Inject(x, s.Value);
            }
        }
    }
}
