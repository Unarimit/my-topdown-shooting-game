using Assets.Scripts.Common;
using DG.Tweening;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic
{
    internal class StatisticPanelUI : HomeUIBase
    {
        [SerializeField]
        TextMeshProUGUI m_statisticTMP;
        [SerializeField]
        Button m_cancelBtn;

        CanvasGroup canvasGroup;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            m_cancelBtn.onClick.AddListener(Quit);
        }
        public void Display(Dictionary<string, int> sum)
        {
            var sb = new StringBuilder();
            foreach(var x in sum)
            {
                sb.AppendLine($"{ItemHelper.GetItem(x.Key).ItemName}: {x.Value}");
            }
            m_statisticTMP.text = sb.ToString();

            canvasGroup.alpha = 1;
        }

        public override void Quit()
        {
            canvasGroup.DOFade(0, 0.5f).OnComplete(base.Quit);
        }
    }
}
