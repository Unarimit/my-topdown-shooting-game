using Assets.Scripts.Common;
using Assets.Scripts.Entities.Buildings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.BuildingUIs
{
    internal class BuildingScrollViewItemUI : MonoBehaviour
    {
        [SerializeField]
        RawImage m_buildingRawImage;
        [SerializeField]
        TextMeshProUGUI m_buildingText;
        public void Inject(Building building, BuildingUI bUI)
        {
            // 注册点击事件
            GetComponent<Button>().onClick.AddListener(() => bUI.OnSelect(building));

            // 配置UI内容
            m_buildingRawImage.texture = PhotographyManager.GetBuildingIcon(building);
            m_buildingText.text = building.Name.ToString();
            HoverToolTipUI.CreateHoverToolTip(transform, building.GetInfo(), 0);
        }

    }
}
