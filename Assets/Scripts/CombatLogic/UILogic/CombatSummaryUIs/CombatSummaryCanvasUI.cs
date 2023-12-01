using Assets.Scripts.CombatLogic.CombatEntities;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CombatLogic.UILogic.CombatSummaryUIs
{
    internal class CombatSummaryCanvasUI : MonoBehaviour
    {

        public static CombatSummaryCanvasUI CreateAndShowCombatSummaryCanvasUI(List<CombatOperator> sortedOperators, bool isWin)
        {
            var prefab = ResourceManager.Load<GameObject>("UIs/CombatSummaryCanvas");
            var go = Instantiate(prefab);
            var con = go.GetComponent<CombatSummaryCanvasUI>();
            con.Inject(sortedOperators, isWin);

            return con;
        }

        public void Inject(List<CombatOperator> sortedOperators, bool isWin)
        {
            // result
            if(isWin is true)
            {
                var temp = transform.Find("ResultPanel").Find("Win");
                temp.gameObject.SetActive(true);
                resultTextWarp = temp.Find("ResultWrapPanel").GetComponent<RectTransform>();
            }
            else
            {
                var temp = transform.Find("ResultPanel").Find("Loss");
                temp.gameObject.SetActive(true);
                resultTextWarp = temp.Find("ResultWrapPanel").GetComponent<RectTransform>();
            }
            // rank


            // anime
            tweenResultText();
            tweenRankPanel(sortedOperators);
            tweenResoucePanel();

        }

        RectTransform resultTextWarp;
        RectTransform rankPanel;
        RectTransform resourcePanel;
        CsRankScrollViewUI rankPanelScrollView;
        private void Awake()
        {
            // panels
            rankPanel = transform.Find("RankPanel").GetComponent<RectTransform>();
            resourcePanel = transform.Find("ResourcePanel").GetComponent<RectTransform>();
            rankPanelScrollView = rankPanel.Find("Scroll View").GetComponent<CsRankScrollViewUI>();
            // button
            resourcePanel.Find("ConfirmButton").GetComponent<Button>().onClick.AddListener(() =>
            {
                CombatContextManager.Instance.QuitScene();
            });

        }
        private void tweenResultText()
        {
            var initSize = resultTextWarp.rect.size;
            resultTextWarp.sizeDelta = new Vector2(0, resultTextWarp.rect.height);
            resultTextWarp.DOSizeDelta(initSize, 0.5f);
        }
        private void tweenRankPanel(List<CombatOperator> sortedOperators)
        {
            var rankCg = rankPanel.GetComponent<CanvasGroup>();
            rankCg.alpha = 0;
            DOVirtual.DelayedCall(2, () =>
            {
                rankPanelScrollView.Inject(sortedOperators);
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
