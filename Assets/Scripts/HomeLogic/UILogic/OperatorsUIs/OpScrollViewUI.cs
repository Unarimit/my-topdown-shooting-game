using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic.OperatorsUIs
{
    /// <summary>
    /// 家园中的展示角色信息，主要展示体力状态
    /// </summary>
    internal class OpScrollViewUI : MonoBehaviour
    {
        [SerializeField]
        GameObject m_itemPrefab;
        public void Inject(IList<Operator> operators)
        {
            var contentTrans = transform.Find("Viewport").Find("Content");
            // clear old
            for(int i = 0; i < contentTrans.childCount; i++)
            {
                 Destroy(contentTrans.GetChild(i).gameObject);
            }

            foreach (var op in operators)
            {
                var go = Instantiate(m_itemPrefab, contentTrans);
                go.SetActive(true);
                go.GetComponent<OpScrollViewItemUI>().Inject(op);
            }
        }
    }
}
