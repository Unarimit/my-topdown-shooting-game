using Assets.Scripts.Common.EscMenu;
using Assets.Scripts.Common.Interface;
using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.HomeLogic.Environment;
using Assets.Scripts.HomeLogic.Interface;
using Assets.Scripts.HomeLogic.Placement;
using Assets.Scripts.HomeLogic.UILogic.BuildingUIs;
using Assets.Scripts.HomeLogic.UILogic.GachaUIs;
using Assets.Scripts.HomeLogic.UILogic.GachaUIs.GachaMe;
using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        // more deep
        CoreCharacterView,
        CoreMechaView,

        GachaCharacterView,
    }
    internal class UIManager : MonoBehaviour
    {
        [SerializeField]
        Transform m_canvas;

        public static UIManager Instance;
        public HomePage CurHomePage { get; private set; }
        public Stack<IOverlayUI> OverlayStack { get; private set; }
        Dictionary<HomePage, List<ISwitchUI>> switchUIs;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");

            OverlayStack = new Stack<IOverlayUI>();

            // register uis
            switchUIs = new Dictionary<HomePage, List<ISwitchUI>>();
            switchUIs.Add(HomePage.MainView, new List<ISwitchUI>());
            switchUIs.Add(HomePage.TopView, new List<ISwitchUI>());
            switchUIs.Add(HomePage.BattleView, new List<ISwitchUI>());
            switchUIs.Add(HomePage.CoreView, new List<ISwitchUI>());
            switchUIs.Add(HomePage.CoreCharacterView, new List<ISwitchUI>());
            switchUIs.Add(HomePage.CoreMechaView, new List<ISwitchUI>());
            switchUIs.Add(HomePage.GachaCharacterView, new List<ISwitchUI>());

            switchUIs[HomePage.CoreView].Add(m_canvas.Find("GachaPanel").GetComponent<GachaPanelUI>());
            switchUIs[HomePage.MainView].Add(m_canvas.Find("OverlayPanel").GetComponent<OverlayPanelUI>());
            switchUIs[HomePage.CoreCharacterView].Add(m_canvas.Find("GachaCharacterPanel").GetComponent<GachaCharacterPanelUI>());
            switchUIs[HomePage.CoreMechaView].Add(m_canvas.Find("GachaMechaPanel").GetComponent<GachaMechaPanelUI>());

        }

        public void Inject(PlacementManager pm)
        {
            var buildingUI = m_canvas.Find("BuildingPanel").GetComponent<BuildingUI>();
            buildingUI.Inject(pm);
            switchUIs[HomePage.TopView].Add(pm);
            switchUIs[HomePage.TopView].Add(buildingUI);
            switchUIs[HomePage.BattleView].Add(pm);
            switchUIs[HomePage.BattleView].Add(buildingUI);
        }
        public void DisplayOutput(Dictionary<string, int> sum)
        {
            m_canvas.Find("OverlayPanel").Find("StatisticPanel").GetComponent<StatisticPanelUI>().Display(sum);
        }


        public void SwitchPage(HomePage page)
        {
            if (page == CurHomePage) return;
            if (CameraManager.Instance.IsFinishTween is false) return;

            // 退出当前UI页面
            foreach (var x in switchUIs[CurHomePage]) x.Quit();
            CurHomePage = page;

            // 调整相机
            StartCoroutine(CameraManager.Instance.SwitchCamera(page));


            // 进入下一个UI页面
            DOVirtual.DelayedCall(0.5f, () =>
            {
                foreach (var x in switchUIs[CurHomePage]) x.Enter();
            });

        }

        public void OnClick(InputValue value)
        {
            foreach (var x in switchUIs[CurHomePage]) x.OnClick();
        }
        public void OnEscMenu(InputValue value)
        {
            // 清理弹出窗口栈
            if(OverlayStack.Count != 0)
            {
                var t = OverlayStack.Pop();
                if (t.Equals(null) is not true)
                {
                    t.Quit(); // 正在退出时也会出问题，但如何考虑这个问题？
                }
                else
                {
                    OnEscMenu(value); // 懒得写循环出队，就递归
                }
                return;
            }

            // 尝试呼出EscMenu
            var ui = EscMenuUI.OpenEscMenuUI();
            OverlayStack.Push(ui);
        }

    }
}
