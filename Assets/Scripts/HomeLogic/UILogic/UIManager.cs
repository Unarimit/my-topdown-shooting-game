using Assets.Scripts.Common.EscMenu;
using Assets.Scripts.Common.Interface;
using Assets.Scripts.HomeLogic.Environment;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        public Stack<IOverlayUI> OverlayStack { get; private set; }
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");

            OverlayStack = new Stack<IOverlayUI>();
        }


        public void OnClick(HomePage pos)
        {
            if (pos == CurHomePage) return;
            if (CameraManager.Instance.IsFinishTween is false) return;
            CurHomePage = pos;
            StartCoroutine(CameraManager.Instance.SwitchCamera(pos));
        }


        public void OnEscMenu(InputValue value)
        {
            // 清理弹出窗口栈
            if(OverlayStack.Count != 0)
            {
                var t = OverlayStack.Pop();
                if(t.Equals(null) is not true) t.Quit();
                
                return;
            }

            // 尝试呼出EscMenu
            var ui = EscMenuUI.OpenEscMenuUI();
            OverlayStack.Push(ui);
        }

    }
}
