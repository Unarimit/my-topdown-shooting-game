using Assets.Scripts.CombatLogic.CombatEntities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.UILogic.StrategyMap
{
    internal class SmOperatorScrollViewUI : MonoBehaviour
    {
        public GameObject m_ItemPrefab;

        Transform contentTrans;
        public void Inject(List<CombatOperator> cops)
        {
            if(contentTrans == null) contentTrans = transform.Find("Viewport").Find("Content");
            foreach (var x in cops)
            {
                var go = Instantiate(m_ItemPrefab, contentTrans);
                go.GetComponent<SmOperatorScrollViewItemUI>().Inject(x);
            }
        }
    }
}
