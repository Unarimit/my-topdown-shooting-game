using Assets.Scripts.Entities.Level;
using Assets.Scripts.HomeLogic.Interface;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.ActionUIs
{
    /// <summary>
    /// 作战页面
    /// </summary>
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
        TextMeshProUGUI m_actionInfoName;
        [SerializeField]
        Image m_actionInfoImage;
        [SerializeField]
        TextMeshProUGUI m_actionInfoDesc;

        LevelRule curSelect;
        Sprite defaultActionImage;

        private void Start()
        {
            defaultActionImage = m_actionInfoImage.sprite;
            // 初始化列表
            m_scrollView.Inject(_context.GetLevelRules(), this);

            // 动作事件
            m_returnBtn.onClick.AddListener(() => _rootUI.SwitchPage(HomePage.MainView));
            m_goBtn.onClick.AddListener(() => _context.GoToLevel(curSelect));

            // 初始化选择状态
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
            m_actionInfoName.text = levelRule.LevelName;
            m_actionInfoDesc.text = levelRule.Description;
            if (levelRule.ImageUrl == null) m_actionInfoImage.sprite = defaultActionImage;
            else m_actionInfoImage.sprite = ResourceManager.Load<Sprite>(levelRule.ImageUrl);
        }


        public void OnClick()
        {
            // do nothing
        }

    }
}
