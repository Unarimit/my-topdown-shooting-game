using Assets.Scripts.Entities.Level;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic.ActionUIs
{
    internal class ActionScrollViewUI : MonoBehaviour
    {
        public GameObject m_ItemPrefab;
        private Transform _contentTrans;
        List<ActionScrollViewItemUI> items;
        public void Inject(IList<LevelRule> levelRules, ActionUI actionUI)
        {
            if(_contentTrans == null) _contentTrans = transform.Find("Viewport").Find("Content");
            items = new List<ActionScrollViewItemUI>(levelRules.Count);
            foreach (var x in levelRules)
            {
                var go = Instantiate(m_ItemPrefab, _contentTrans);
                var comp = go.GetComponent<ActionScrollViewItemUI>();

                comp.Inject(x, actionUI, this);
                items.Add(comp);
            }
            items[0].SetSelect(true);
        }

        public void DeSelectAll()
        {
            foreach(var x in items)
            {
                x.SetSelect(false);
            }
        }
    }
}
