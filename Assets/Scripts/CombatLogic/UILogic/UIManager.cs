using Assets.Scripts.ComputerControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.UILogic
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
        }
        private void Start()
        {
            var uis = FindObjectsOfType<SubUIBase>();
            foreach(var x in uis)
            {
                windows.Add(x.name, x);
            }
        }

        private void OnGUI()
        {
            CheckBreakHUD();
        }

        const string BreakHUD = "BreakHUDImg";
        private void CheckBreakHUD()
        {
            windows[BreakHUD].SetVisible(_context.IsPlayerNoShield());
        }
        
    }
}
