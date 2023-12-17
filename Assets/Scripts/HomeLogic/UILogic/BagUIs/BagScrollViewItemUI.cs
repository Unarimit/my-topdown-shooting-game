using Assets.Scripts.Common;
using Assets.Scripts.Entities;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic.BagUIs
{
    internal class BagScrollViewItemUI : MonoBehaviour
    {
        public RawImage m_ItemRawImage;
        public TextMeshProUGUI m_ItemAmountTMP;
        internal void Inject(GameItem item, int amount)
        {
            m_ItemRawImage.texture = ItemHelper.GetItemTexture(item.ItemId);
            m_ItemAmountTMP.text = amount.ToString();
            HoverToolTipUI.CreateHoverToolTip(transform, $"{item.ItemName}\n{item.Description}", 0);
        }
    }
}
