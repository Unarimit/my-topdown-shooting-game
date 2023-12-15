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
            m_topAreaBtn.onClick.AddListener(() => OnClick(CameraPos.TopView));
            m_coreAreaBtn.onClick.AddListener(() => OnClick(CameraPos.CoreView));
            m_battleAreaBtn.onClick.AddListener(() => OnClick(CameraPos.BattleView));
            m_mainViewBtn.onClick.AddListener(() => OnClick(CameraPos.MainView));
        }

        private void OnClick(CameraPos pos)
        {
            if (pos == CameraManager.Instance.CurCameraPos) return;
            if (CameraManager.Instance.IsFinishTween is false) return;
            StartCoroutine(CameraManager.Instance.SwitchCamera(pos));
        }
    }
}
