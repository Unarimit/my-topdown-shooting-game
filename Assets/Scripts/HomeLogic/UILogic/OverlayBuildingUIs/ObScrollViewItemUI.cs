using Assets.Scripts.Common;
using Assets.Scripts.Entities.Buildings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.OverlayBuildingUIs
{
    internal class ObScrollViewItemUI : MonoBehaviour
    {
        [SerializeField]
        RawImage m_buildingImage;
        [SerializeField]
        TextMeshProUGUI m_bNameTMP;
        [SerializeField]
        TextMeshProUGUI m_bDescTMP;
        [SerializeField]
        TextMeshProUGUI m_bPosTMP;
        [SerializeField]
        RawImage m_buildingOperatorImage;
        [SerializeField]
        ObProduceScrollViewUI m_obProduceScrollViewUI;

        internal void Inject(PlaceInfo place, Building building)
        {
            m_buildingImage.texture = PhotographyManager.GetBuildingIcon(building);
            m_bNameTMP.text = building.Name;
            m_bDescTMP.text = building.Description;
            m_bPosTMP.text = $"区块{place.AreaIndex}-{place.PlacePosition.ToString()}";
            if(place.AdminOperator != null) m_buildingOperatorImage.texture = PhotographyManager.GetOperatorHeadIcon(place.AdminOperator);
            if (building is ResourceBuilding rb) m_obProduceScrollViewUI.Inject(rb.Produces);
        }
    }
}
