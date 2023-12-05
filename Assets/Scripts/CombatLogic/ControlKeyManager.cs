using Assets.Scripts.CombatLogic.ContextExtends;
using Assets.Scripts.CombatLogic.EnviormentLogic;
using Assets.Scripts.Common.EscMenu;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.CombatLogic
{
    /// <summary>
    /// 不与人物相关的控制指令
    /// </summary>
    internal class ControlKeyManager : MonoBehaviour
    {
        private bool isEscMenu = false;
        private CombatContextManager _context;
        private void Awake()
        {
            _context = GetComponent<CombatContextManager>();
        }

        public void OnEscMenu(InputValue value)
        {
            if (isEscMenu) return;
            isEscMenu = true;
            Time.timeScale = 0;
            var ui = EscMenuUI.OpenEscMenuUI();
            ui.ReturnBtn.onClick.AddListener(() =>
            {
                isEscMenu = false;
                Time.timeScale = 1;
            });
        }

        private bool isStrategyMap = false;
        private StrategyMapController strategyMapController;
        public void OnStrategyMap(InputValue value)
        {
            if (strategyMapController != null && strategyMapController.IsDestory) return;
            if (isStrategyMap is true)
            {
                isStrategyMap = false;
                _context.ActiveAllCharacter(true);
                GetComponent<UIManager>().TweenEnter();
                strategyMapController.Destroy();
            }
            else
            {
                isStrategyMap = true;
                GetComponent<UIManager>().TweenQuit();
                _context.ActiveAllCharacter(false);
                strategyMapController = StrategyMapController.CreateStrategyMap();
            }
            
        }
    }
}
