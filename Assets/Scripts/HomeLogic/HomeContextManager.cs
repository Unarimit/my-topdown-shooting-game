using Assets.Scripts.Common;
using Assets.Scripts.Common.EscMenu;
using Assets.Scripts.Entities;
using Assets.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.HomeLogic
{
    internal class HomeContextManager : MonoBehaviour
    {
        public static HomeContextManager Instance;

        public ViewModel HomeVM { get; private set; }
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
            Time.timeScale = 1;

            HomeVM = new ViewModel();
        }

        public IList<LevelRule> GetLevelRules()
        {
            return MyServices.Database.LevelRules;
        }

        public void GoToLevel(LevelRule rule)
        {
            MyServices.Database.CurLevel = LevelGenerator.GeneratorLevelInfo(rule);
            
            StartCoroutine(SceneLoadHelper.MyLoadSceneAsync("Prepare"));
            
        }

        private bool isEscMenu = false;
        public void OnEscMenu(InputValue value)
        {
            if (isEscMenu) return;
            isEscMenu = true;
            var ui = EscMenuUI.OpenEscMenuUI();
            ui.ReturnBtn.onClick.AddListener(() =>
            {
                isEscMenu = false;
            });
        }


        public class ViewModel
        {
            /// <summary> 人口 </summary>
            public int Population { get; private set; }
            /// <summary> 电力 </summary>
            public int ResElectric { get; private set; }
            /// <summary> 铁 </summary>
            public int ResIron { get; private set; }
            /// <summary> 弹药 </summary>
            public int ResAmmo { get; private set; }
            /// <summary> 铝 </summary>
            public int ResAl { get; private set; }
            /// <summary> 抽卡道具 </summary>
            public int ResGacha { get; private set; }
            public ViewModel()
            {
                Population = MyServices.Database.Operators.Count;
                ResElectric = MyServices.Database.Inventory[MyConfig.ItemTable.Electric.ToString()];
                ResIron = MyServices.Database.Inventory[MyConfig.ItemTable.Iron.ToString()];
                ResAmmo = MyServices.Database.Inventory[MyConfig.ItemTable.Ammo.ToString()];
                ResAl = MyServices.Database.Inventory[MyConfig.ItemTable.Al.ToString()];
                ResGacha = MyServices.Database.Inventory[MyConfig.ItemTable.Red.ToString()];
            }
        }
    }
}
