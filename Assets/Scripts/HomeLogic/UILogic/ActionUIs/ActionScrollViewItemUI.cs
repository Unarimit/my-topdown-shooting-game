using Assets.Scripts.Entities.Level;
using Michsky.UI.Shift;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.ActionUIs
{
    internal class ActionScrollViewItemUI : MonoBehaviour
    {
        [SerializeField]
        RawImage m_combatImg;
        [SerializeField]
        RawImage m_homeImg;

        MainPanelButton button;
        bool _isSelect;
        public void Inject(LevelRule levelRule, ActionUI actionUI, ActionScrollViewUI container)
        {
            button = transform.GetComponent<MainPanelButton>();
            button.buttonText = levelRule.LevelName;
            gameObject.SetActive(true);
            GetComponent<Button>().onClick.AddListener(() =>
            {
                container.DeSelectAll();
                SetSelect(true);
                actionUI.OnActionSelect(levelRule);
            });

            if(levelRule is CombatLevelRule)
            {
                m_combatImg.gameObject.SetActive(true);
                m_homeImg.gameObject.SetActive(false);
            }
            else
            {
                m_combatImg.gameObject.SetActive(false);
                m_homeImg.gameObject.SetActive(true);
            }
        }
        private void OnEnable()
        {
            // 这个组件会刷新Press状态
            if(_isSelect is true) button.SetKeyPress(_isSelect);
        }

        public void SetSelect(bool isSelect)
        {
            _isSelect = isSelect;
            button.SetKeyPress(isSelect);
        }
    }
}
