using Assets.Scripts.CombatLogic.UILogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        private Dictionary<string, SubUIBase> windows;
        private CombatContextManager _context => CombatContextManager.Instance;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");

            windows = new Dictionary<string, SubUIBase>();

            var uis = FindObjectsOfType<SubUIBase>();
            foreach (var x in uis)
            {
                windows.Add(x.name, x);
                x.SetVisible(false);
            }
        }
        public void Init()
        {
            foreach(var x in windows.Values)
            {
                x.SetVisible(true);
            }

            windows[ReviveCountdownPanel].SetVisible(false);
        }

        const string ReviveCountdownPanel = "ReviveCountdownPanel";
        const string BreakHUD = "BreakHUDImg";
        public void ShowReviveCountdown()
        {
            windows[ReviveCountdownPanel].SetVisible(true);
        }
        /// <summary>
        /// 结束战斗UI
        /// </summary>
        public void TweenQuit()
        {
            foreach (var x in windows.Values)
            {
                x.TweenQuit(0.5f);
            }
        }
        public void TweenEnter()
        {
            foreach (var x in windows.Values)
            {
                x.TweenEnter(0.5f);
            }
        }

        private void OnGUI()
        {
            CheckBreakHUD();
        }

        private void CheckBreakHUD()
        {
            windows[BreakHUD].SetVisible( // 当生命小于50%时，显示生命量少效果的HUD
                (float)_context.CombatVM.Player.CurrentHP / _context.CombatVM.Player.MaxHP < 0.5);
        }
        
    }
}
