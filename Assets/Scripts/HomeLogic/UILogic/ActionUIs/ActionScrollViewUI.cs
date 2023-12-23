using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic.ActionUIs
{
    internal class ActionScrollViewUI : MonoBehaviour
    {
        public GameObject m_ItemPrefab;
        private Transform _contentTrans;
        public void Inject(IList<LevelRule> levelRules, ActionUI actionUI)
        {
            if(_contentTrans == null) _contentTrans = transform.Find("Viewport").Find("Content");
            foreach(var x in levelRules)
            {
                var go = Instantiate(m_ItemPrefab, _contentTrans);
                go.GetComponent<ActionScrollViewItemUI>().Inject(x, actionUI);
            }
        }
    }
}
