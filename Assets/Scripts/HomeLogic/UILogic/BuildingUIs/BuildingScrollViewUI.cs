using Assets.Scripts.Entities.Buildings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic.BuildingUIs
{
    internal class BuildingScrollViewUI : MonoBehaviour
    {
        [SerializeField]
        GameObject m_item;
        public void Inject(IList<Building> buildings, BuildingUI bUI)
        {
            var contentTrans = transform.Find("Viewport").Find("Content");
            // clear old
            for (int i = 0; i < contentTrans.childCount; i++)
            {
                Destroy(contentTrans.GetChild(i).gameObject);
            }

            foreach (var x in buildings)
            {
                var go = Instantiate(m_item, contentTrans);
                go.GetComponent<BuildingScrollViewItemUI>().Inject(x, bUI);
                go.SetActive(true);
            }
        }
    }
}
