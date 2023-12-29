using Assets.Scripts.Entities.Buildings;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic.OverlayBuildingUIs
{
    internal class ObProduceScrollViewUI : MonoBehaviour
    {
        [SerializeField]
        GameObject m_itemPrefab;
        internal void Inject(Produce[] produces)
        {
            var contentTrans = transform.Find("Viewport").Find("Content");
            // clear old
            for (int i = 0; i < contentTrans.childCount; i++)
            {
                Destroy(contentTrans.GetChild(i).gameObject);
            }

            foreach (var p in produces)
            {
                var go = Instantiate(m_itemPrefab, contentTrans);
                go.SetActive(true);
                go.GetComponent<ObProduceScrollViewItemUI>().Inject(p);
            }
        }
    }
}
