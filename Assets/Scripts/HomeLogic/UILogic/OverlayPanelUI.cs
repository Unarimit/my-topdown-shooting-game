
using Assets.Scripts.HomeLogic.Interface;
using Assets.Scripts.HomeLogic.UILogic.BagUIs;
using Assets.Scripts.HomeLogic.UILogic.OperatorsUIs;
using Assets.Scripts.HomeLogic.UILogic.OverlayBuildingUIs;
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
        [SerializeField]
        private Button m_buildingBtn;

        private BagPanelUI bagPanelUI;
        private OperatorsPanelUI opsPanelUI;
        private OverlayBuildingPanelUI buildingPanelUI;
        protected override void Awake()
        {
            base.Awake();
            bagPanelUI = transform.Find("BagPanel").GetComponent<BagPanelUI>();
            opsPanelUI = transform.Find("OperatorsPanel").GetComponent<OperatorsPanelUI>();
            if(m_buildingBtn != null) buildingPanelUI = transform.Find("BuildingsPanel").GetComponent<OverlayBuildingPanelUI>();
        }
        private void OnEnable()
        {
            m_bagBtn.onClick.AddListener(openBag);
            m_characterBtn.onClick.AddListener(openCharacter);
            if (m_buildingBtn != null) m_buildingBtn.onClick.AddListener(() => buildingPanelUI.Enter());

        }
        private void OnDisable()
        {
            m_bagBtn.onClick.RemoveListener(openBag);
            m_characterBtn.onClick.RemoveListener(openCharacter);
            if (m_buildingBtn != null) m_buildingBtn.onClick.RemoveAllListeners();
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

    }
}
