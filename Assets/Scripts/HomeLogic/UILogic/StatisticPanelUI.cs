using Assets.Scripts.Common;
using DG.Tweening;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic
{
    /// <summary>
    /// 生产报告 Panel
    /// </summary>
    internal class StatisticPanelUI : HomeUIBase
    {
        [SerializeField]
        TextMeshProUGUI m_statisticTMP;
        [SerializeField]
        Button m_cancelBtn;

        public void Display(Dictionary<string, int> sum)
        {
            gameObject.SetActive(true);
            m_cancelBtn.onClick.AddListener(Quit);

            var sb = new StringBuilder();
            foreach(var x in sum)
            {
                sb.AppendLine($"{ItemHelper.GetItem(x.Key).ItemName}: {x.Value}");
            }
            m_statisticTMP.text = sb.ToString();

            GetComponent<CanvasGroup>().alpha = 1;
        }

    }
}
