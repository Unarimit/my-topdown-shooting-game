using Assets.Scripts.Entities.Save;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.StartLogic.UILogic.SaveChooseUIs
{
    internal class SaveChooseScrollViewUI : MonoBehaviour
    {
        [SerializeField]
        GameObject m_itemPrefab;
        
        bool init = false;
        internal void Inject(IList<SaveAbstract> saves, SaveChoosePanelUI saveChoosePanelUI)
        {
            if (init is true) return;

            var contentTrans = transform.Find("Viewport").Find("Content");
            foreach (var save in saves)
            {
                var go = Instantiate(m_itemPrefab, contentTrans);
                go.SetActive(true);
                go.GetComponent<SaveChooseScrollViewItemUI>().Inject(save, saveChoosePanelUI);
            }

            init = true;
        }
    }
}
