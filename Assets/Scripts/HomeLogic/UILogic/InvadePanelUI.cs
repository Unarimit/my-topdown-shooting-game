using Assets.Scripts.Common.Test;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic
{
    internal class InvadePanelUI : HomeUIBase
    {
        [SerializeField]
        RectTransform m_scrollPanel;
        [SerializeField]
        RectTransform m_scrollPanelSmall;

        protected override void Awake()
        {
            base.Awake();
            m_scrollPanel.gameObject.SetActive(false);
            m_scrollPanelSmall.gameObject.SetActive(false);
        }

        private void Start()
        {
            StartCoroutine(coroTask());
            m_scrollPanelSmall.GetComponent<Button>().onClick.AddListener(() => _rootUI.SwitchPage(HomePage.ActionView));

        }
        private IEnumerator coroTask()
        {
            m_scrollPanel.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);

            m_scrollPanel.DOSizeDelta(m_scrollPanelSmall.sizeDelta, 0.2f);
            m_scrollPanel.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().DOFade(0, 0.2f);
            yield return new WaitForSeconds(0.2f);

            m_scrollPanel.DOAnchorPos(m_scrollPanelSmall.anchoredPosition, 0.5f);
            yield return new WaitForSeconds(0.5f);

            m_scrollPanel.gameObject.SetActive(false);
            m_scrollPanelSmall.gameObject.SetActive(true);

        }
    }
}
