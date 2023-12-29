using Assets.Scripts.Common;
using Assets.Scripts.Entities.Buildings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.OverlayBuildingUIs
{
    internal class ObProduceScrollViewItemUI : MonoBehaviour
    {
        [SerializeField]
        RawImage m_itemRawImage;
        [SerializeField]
        TextMeshProUGUI m_amountTMP;
        internal void Inject(Produce p)
        {
            m_itemRawImage.texture = ItemHelper.GetItemTexture(p.ItemId);
            m_amountTMP.text = p.Amount.ToString();
        }
    }
}
