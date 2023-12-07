using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.CombatLogic.LevelLogic;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CombatLogic.UILogic.CombatSummaryUIs
{
    internal class CombatSummaryCanvasUI : MonoBehaviour
    {

        public static CombatSummaryCanvasUI CreateAndShowCombatSummaryCanvasUI(List<CombatOperator> sortedOperators,
            bool isWin, Dictionary<string, int> dropouts)
        {
            var prefab = ResourceManager.Load<GameObject>("UIs/CombatSummaryCanvas");
            var go = Instantiate(prefab);
            var con = go.GetComponent<CombatSummaryCanvasUI>();
            con.Inject(sortedOperators, isWin, dropouts);

            return con;
        }

        public void Inject(List<CombatOperator> sortedOperators, bool isWin, Dictionary<string, int> dropouts)
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

            // data and anime anime
            tweenResultText();
            tweenRankPanel(sortedOperators);
            tweenResoucePanel(dropouts);

        }

        // awake 注册组件
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
        /// <summary>
        /// result过渡动画
        /// </summary>
        private void tweenResultText()
        {
            var initSize = resultTextWarp.rect.size;
            resultTextWarp.sizeDelta = new Vector2(0, resultTextWarp.rect.height);
            resultTextWarp.DOSizeDelta(initSize, 0.5f);
        }
        /// <summary>
        /// 排名过渡动画
        /// </summary>
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
        /// <summary>
        /// 资源栏过渡动画
        /// </summary>
        private void tweenResoucePanel(Dictionary<string, int> dropouts)
        {
            var initAnchor = resourcePanel.anchoredPosition;
            resourcePanel.anchoredPosition = new Vector2(initAnchor.x+resourcePanel.rect.width, initAnchor.y);
            DOVirtual.DelayedCall(1, () =>
            {
                resourcePanel.DOAnchorPos(initAnchor, 0.2f);
            });

            resourcePanel.Find("DropoutSPanel").Find("Scroll View").GetComponent<CsDropoutScrollViewUI>().Inject(dropouts);
        }
    }
}
