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
        private void OnDestroy()
        {
            HomeVM.Save();
        }


        public class ViewModel
        {
            /// <summary> 全局时间 </summary>
            public int GTime { get; private set; } // 变化会涉及创景切换，所以不做数据绑定

            /// <summary> 人口 </summary>
            public MyBinded<int> Population { get; private set; } = new MyBinded<int>();
            /// <summary> 电力 </summary>
            public MyBinded<int> ResElectric { get; private set; } = new MyBinded<int>();
            /// <summary> 铁 </summary>
            public MyBinded<int> ResIron { get; private set; } = new MyBinded<int>(); 
            /// <summary> 弹药 </summary>
            public MyBinded<int> ResAmmo { get; private set; } = new MyBinded<int>();
            /// <summary> 铝 </summary>
            public MyBinded<int> ResAl { get; private set; } = new MyBinded<int>();
            /// <summary> 抽卡道具 </summary>
            public MyBinded<int> ResGacha { get; private set; } = new MyBinded<int>();
            public ViewModel()
            {
                GTime = MyServices.Database.Inventory[MyConfig.ItemTable.GTime.ToString()];
                Population.Data = MyServices.Database.Operators.Count;
                ResElectric.Data = MyServices.Database.Inventory[MyConfig.ItemTable.Electric.ToString()];
                ResIron.Data = MyServices.Database.Inventory[MyConfig.ItemTable.Iron.ToString()];
                ResAmmo.Data = MyServices.Database.Inventory[MyConfig.ItemTable.Ammo.ToString()];
                ResAl.Data = MyServices.Database.Inventory[MyConfig.ItemTable.Al.ToString()];
                ResGacha.Data = MyServices.Database.Inventory[MyConfig.ItemTable.Red.ToString()];
            }
            public void Save()
            {
                MyServices.Database.Inventory[MyConfig.ItemTable.Electric.ToString()] = ResElectric.Data;
                MyServices.Database.Inventory[MyConfig.ItemTable.Iron.ToString()] = ResIron.Data;
                MyServices.Database.Inventory[MyConfig.ItemTable.Ammo.ToString()] = ResAmmo.Data;
                MyServices.Database.Inventory[MyConfig.ItemTable.Al.ToString()] = ResAl.Data;
                MyServices.Database.Inventory[MyConfig.ItemTable.Red.ToString()] = ResGacha.Data;
            }
        }
    }
}
