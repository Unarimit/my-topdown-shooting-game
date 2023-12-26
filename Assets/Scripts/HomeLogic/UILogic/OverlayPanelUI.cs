﻿
using Assets.Scripts.HomeLogic.Interface;
using Assets.Scripts.HomeLogic.UILogic.BagUIs;
using Assets.Scripts.HomeLogic.UILogic.OperatorsUIs;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic
{
    internal class OverlayPanelUI : HomeUIBase, ISwitchUI
    {
        [SerializeField]
        private Button m_bagBtn;
        [SerializeField]
        private Button m_characterBtn;

        private BagPanelUI bagPanelUI;
        private OperatorsPanelUI opsPanelUI;
        CanvasGroup canvasGroup;
        private void Awake()
        {
            bagPanelUI = transform.Find("BagPanel").GetComponent<BagPanelUI>();
            opsPanelUI = transform.Find("OperatorsPanel").GetComponent<OperatorsPanelUI>();
            canvasGroup = GetComponent<CanvasGroup>();
        }
        private void OnEnable()
        {
            m_bagBtn.onClick.AddListener(openBag);
            m_characterBtn.onClick.AddListener(openCharacter);

        }
        private void OnDisable()
        {
            m_bagBtn.onClick.RemoveListener(openBag);
            m_characterBtn.onClick.RemoveListener(openCharacter);
        }

        private void openBag()
        {
            bagPanelUI.Enter();
        }
        private void openCharacter()
        {
            opsPanelUI.Enter();
        }

        public void OnClick()
        {

        }

        public override void Enter()
        {
            base.Enter();
            canvasGroup.alpha = 1;
        }

        public override void Quit()
        {
            canvasGroup.DOFade(0, 0.5f).OnComplete(base.Quit);
        }
    }
}
