using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic.UILogic.LevelUIs
{
    internal class LevelDropoutScrollViewUI : MonoBehaviour
    {
        public GameObject m_ItemPrefab;

        public void Inject(IEnumerable<GameItem> dropouts) 
        {
            var contentTrans = transform.Find("Viewport").Find("Content");
            foreach(var x in dropouts)
            {
                if (x.IsDisplay is false) continue;
                var go = Instantiate(m_ItemPrefab, contentTrans);
                go.SetActive(true);
                go.GetComponent<LevelDropoutScrollViewItemUI>().Inject(x);
            }
        }
    }
}
