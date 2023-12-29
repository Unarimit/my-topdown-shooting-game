using Assets.Scripts.Entities.Buildings;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic.OverlayBuildingUIs
{
    internal class ObScrollViewUI : MonoBehaviour
    {
        [SerializeField]
        GameObject m_itemPrefab;
        public void Inject(IList<PlaceInfo> places, IDictionary<string, Building> buildings)
        {
            var contentTrans = transform.Find("Viewport").Find("Content");
            // clear old
            for (int i = 0; i < contentTrans.childCount; i++)
            {
                Destroy(contentTrans.GetChild(i).gameObject);
            }

            foreach (var place in places)
            {
                var go = Instantiate(m_itemPrefab, contentTrans);
                go.SetActive(true);
                go.GetComponent<ObScrollViewItemUI>().Inject(place, buildings[place.BuildingId]);
            }
        }
    }
}
