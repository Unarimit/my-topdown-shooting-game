using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common.UI.OperatorChooseUIs
{
    internal class OperatorChooseScrollViewUI : MonoBehaviour
    {
        [SerializeField]
        GameObject m_itemPrefab;
        public void Inject(OperatorChooseUI ui, IEnumerable<Operator> operators)
        {
            var contentTrans = transform.Find("Viewport").Find("Content");
            foreach (var op in operators)
            {
                var go = Instantiate(m_itemPrefab, contentTrans);
                go.SetActive(true);
                go.GetComponent<OperatorChooseScrollViewItemUI>().Inject(op, ui);
            }
        }
    }
}
