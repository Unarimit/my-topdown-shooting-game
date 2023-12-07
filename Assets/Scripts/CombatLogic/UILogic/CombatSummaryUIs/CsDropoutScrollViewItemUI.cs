using Assets.Scripts.Common;
using Assets.Scripts.Entities;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CombatLogic.UILogic.CombatSummaryUIs
{
    /// <summary>
    /// 结算界面的掉落物UI，主要是动画显示
    /// </summary>
    internal class CsDropoutScrollViewItemUI : MonoBehaviour
    {
        public RawImage m_ItemRawImage;
        public TextMeshProUGUI m_ItemAmountTMP;
        private void Start()
        {
            // TODO：根据初始化时间（常量化）和下标重写这段方法。
            transform.localScale = Vector3.zero;
            DOVirtual.DelayedCall(1.2f, () =>
            {
                transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce);
            });
            
        }
        internal void Inject(GameItem item, int amount)
        {
            m_ItemRawImage.texture = ItemHelper.GetItemTexture(item.ItemId);
            m_ItemAmountTMP.text = amount.ToString();
            HoverToolTipUI.CreateHoverToolTip(transform, $"{item.ItemName}\n{item.Description}", 0, 2f);
        }
    }
}
