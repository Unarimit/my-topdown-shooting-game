
using Assets.Scripts.HomeLogic.UILogic.BagUIs;
using Assets.Scripts.HomeLogic.UILogic.OperatorsUIs;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic
{
    internal class OverlayPanelUI : HomeUIBase
    {
        [SerializeField]
        private Button m_bagBtn;
        [SerializeField]
        private Button m_characterBtn;

        private BagPanelUI bagPanelUI;
        private OperatorsPanelUI opsPanelUI;
        private void Awake()
        {
            bagPanelUI = transform.Find("BagPanel").GetComponent<BagPanelUI>();
            opsPanelUI = transform.Find("OperatorsPanel").GetComponent<OperatorsPanelUI>();
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
    }
}
