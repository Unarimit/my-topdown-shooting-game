using Assets.Scripts.HomeLogic.Environment;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic
{
    internal enum HomePage
    {
        MainView,
        TopView,
        CoreView,
        BattleView,
    }
    internal class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        public HomePage CurHomePage { get; private set; }
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }


        public void OnClick(HomePage pos)
        {
            if (pos == CurHomePage) return;
            if (CameraManager.Instance.IsFinishTween is false) return;
            //CurHomePage = pos;
            StartCoroutine(CameraManager.Instance.SwitchCamera(pos));
        }
    }
}
