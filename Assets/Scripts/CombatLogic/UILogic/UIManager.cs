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
        public void CombatUIFinish()
        {
            foreach (var x in windows.Values)
            {
                x.SetVisible(false);
            }
        }

        private void OnGUI()
        {
            CheckBreakHUD();
        }

        private void CheckBreakHUD()
        {
            windows[BreakHUD].SetVisible(_context.IsPlayerNoShield());
        }
        
    }
}
