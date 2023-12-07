using Assets.Scripts.Common;
using Assets.Scripts.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PrepareLogic.UILogic.LevelUIs
{
    internal class LevelDropoutScrollViewItemUI : MonoBehaviour
    {
        public RawImage m_ItemRawImage;
        public TextMeshProUGUI m_ItemNameTMP;

        internal void Inject(GameItem item)
        {
            m_ItemRawImage.texture = ItemHelper.GetItemTexture(item.ItemId);
            m_ItemNameTMP.text = item.ItemName;
            HoverToolTipUI.CreateHoverToolTip(transform, item.Description, 0);
        }
    }
}
