using Assets.Scripts.CombatLogic.CombatEntities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.UILogic.CombatSummaryUIs
{
    internal class CsRankScrollViewUI : MonoBehaviour
    {
        public GameObject m_ItemPrefab;

        Transform contentTrans;
        private void Awake()
        {
            contentTrans = transform.Find("Viewport").Find("Content");
        }
        public void Inject(List<CombatOperator> sortedCops)
        {
            int totalDmg = 0;
            int totalReceive = 0;
            foreach(var x in sortedCops)
            {
                totalDmg += x.StatCauseDamage;
                totalReceive += x.StatReceiveDamage;
            }
            for(int i = 0; i < sortedCops.Count; i++)
            {
                var go = Instantiate(m_ItemPrefab, contentTrans);
                go.GetComponent<CsRankScrollViewItemUI>().Inject(i, sortedCops[i], totalDmg, totalReceive);

            }
        }
    }
}
