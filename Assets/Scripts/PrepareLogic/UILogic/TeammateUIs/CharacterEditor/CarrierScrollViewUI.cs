using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic.UILogic.TeammateUIs.CharacterEditor
{
    internal class CarrierScrollViewUI : MonoBehaviour
    {
        public GameObject m_CarriorItemPrefab;
        private PrepareContextManager _context => PrepareContextManager.Instance;

        private List<CarrierScrollViewItemUI> _carriorItems;
        private Transform _contentTrans;
        private void Awake()
        {
            _contentTrans = transform.Find("Viewport").Find("Content");
        }
        public void GenerateHeadIcon(List<Fighter> fighters)
        {
            if(_carriorItems != null)
            {
                for(int i = 0; i < _carriorItems.Count; i++) Destroy(_carriorItems[i].gameObject);
            }
            _carriorItems = new List<CarrierScrollViewItemUI>();
            foreach (var fi in fighters)
            {
                var go = Instantiate(m_CarriorItemPrefab, _contentTrans);

                // set content
                var cp = go.GetComponent<CarrierScrollViewItemUI>();
                cp.Inject(fi);

                _carriorItems.Add(cp);
            }
        }
    }
}
