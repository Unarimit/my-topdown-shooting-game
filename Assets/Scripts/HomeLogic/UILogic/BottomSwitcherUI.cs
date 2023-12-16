using Assets.Scripts.HomeLogic.Environment;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic
{
    internal class BottomSwitcherUI : HomeUIBase
    {
        [SerializeField]
        private Button m_topAreaBtn;
        [SerializeField]
        private Button m_coreAreaBtn;
        [SerializeField]
        private Button m_battleAreaBtn;
        [SerializeField]
        private Button m_mainViewBtn;
        [SerializeField]
        private Button m_actionBtn;

        private void Start()
        {
            m_topAreaBtn.onClick.AddListener(() => _rootUI.OnClick(HomePage.TopView));
            m_coreAreaBtn.onClick.AddListener(() => _rootUI.OnClick(HomePage.CoreView));
            m_battleAreaBtn.onClick.AddListener(() => _rootUI.OnClick(HomePage.BattleView));
            m_mainViewBtn.onClick.AddListener(() => _rootUI.OnClick(HomePage.MainView));
        }

    }
}
