using Assets.Scripts.Entities;
using Assets.Scripts.HomeLogic.Interface;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.ActionUIs
{
    internal class ActionUI : HomeUIBase, ISwitchUI
    {
        [SerializeField]
        ActionScrollViewUI m_scrollView;
        [SerializeField]
        Button m_goBtn;
        [SerializeField]
        Button m_returnBtn;

        // action Info
        [SerializeField]
        RawImage m_actionInfoImage;
        [SerializeField]
        TextMeshProUGUI m_actionInfoDesc;

        LevelRule curSelect;
        private void Start()
        {
            m_scrollView.Inject(_context.GetLevelRules(), this);
            m_returnBtn.onClick.AddListener(() => _rootUI.SwitchPage(HomePage.MainView));
            m_goBtn.onClick.AddListener(() => _context.GoToLevel(curSelect));
            OnActionSelect(_context.GetLevelRules()[0]);
        }
        private void OnDestroy()
        {
            m_returnBtn.onClick.RemoveAllListeners();
            m_goBtn.onClick.RemoveAllListeners();
        }

        public void OnActionSelect(LevelRule levelRule)
        {
            curSelect = levelRule;
            m_actionInfoDesc.text = levelRule.Description;
        }

        public void OnClick()
        {
            // do nothing
        }
    }
}
