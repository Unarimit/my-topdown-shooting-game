using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.UILogic.CombatSummaryUIs
{
    internal class CombatSummaryCanvasUI : MonoBehaviour
    {
        RectTransform resultTextWarp;
        RectTransform rankPanel;
        RectTransform resourcePanel;
        private void Awake()
        {
            resultTextWarp = transform.Find("ResultPanel").Find("ResultWrapPanel").GetComponent<RectTransform>();
            rankPanel = transform.Find("RankPanel").GetComponent<RectTransform>();
            resourcePanel = transform.Find("ResourcePanel").GetComponent<RectTransform>();

            tweenResultText();
            tweenRankPanel();
            tweenResoucePanel();

        }
        private void tweenResultText()
        {
            var initSize = resultTextWarp.rect.size;
            resultTextWarp.sizeDelta = new Vector2(0, resultTextWarp.rect.height);
            resultTextWarp.DOSizeDelta(initSize, 0.2f);
        }
        private void tweenRankPanel()
        {
            var rankCg = rankPanel.GetComponent<CanvasGroup>();
            rankCg.alpha = 0;
            DOVirtual.DelayedCall(2, () =>
            {
                rankCg.DOFade(1, 0.5f);
            });
        }
        private void tweenResoucePanel()
        {
            var initAnchor = resourcePanel.anchoredPosition;
            resourcePanel.anchoredPosition = new Vector2(initAnchor.x+resourcePanel.rect.width, initAnchor.y);
            DOVirtual.DelayedCall(1, () =>
            {
                resourcePanel.DOAnchorPos(initAnchor, 0.2f);
            });
        }
    }
}
