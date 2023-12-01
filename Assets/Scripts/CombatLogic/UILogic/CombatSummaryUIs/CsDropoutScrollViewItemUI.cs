using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.UILogic.CombatSummaryUIs
{
    /// <summary>
    /// 结算界面的掉落物UI，主要是动画显示
    /// </summary>
    internal class CsDropoutScrollViewItemUI : MonoBehaviour
    {
        private void Start()
        {
            // TODO：根据初始化时间（常量化）和下标重写这段方法。
            transform.localScale = Vector3.zero;
            DOVirtual.DelayedCall(1.2f, () =>
            {
                transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce);
            });
            
        }
    }
}
