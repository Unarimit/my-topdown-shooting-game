using Assets.Scripts.CombatLogic;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Test;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.Entities.Level;
using Assets.Scripts.HomeLogic.UILogic;
using Assets.Scripts.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

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
        [MyTest]
        public void TestNextDay()
        {
            MyServices.Database.Inventory[MyConfig.ItemTable.GTime.ToString()] += 1;
            MyServices.Database.OnNewDay = true;
            SceneLoadHelper.MyLoadSceneAsync("Home");
        }
        [MyTest]
        public void TestNextInvadeDay()
        {
            MyServices.Database.Inventory[MyConfig.ItemTable.GTime.ToString()] = (MyServices.Database.Inventory[MyConfig.ItemTable.GTime.ToString()] / 7 + 1) * 7;
            MyServices.Database.OnNewDay = true;
            SceneLoadHelper.MyLoadSceneAsync("Home");
        }

        public IList<LevelRule> GetLevelRules()
        {
            var list = new List<LevelRule>();
            foreach(var x in MyServices.Database.LevelRules)
            {
                if(x.IsActive is true)
                {
                    if(x.IsOnly is true)
                    {
                        list.Clear();
                        list.Add(x);
                        break;
                    }
                    else
                    {
                        list.Add(x);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 如果可以支付开销，则支付。不能则不变动数据，返回false
        /// </summary>
        public bool TryAffordCost(Building building)
        {
            if (IsCanAfford(building.Costs) is false) return false;
            Afford(building.Costs);
            return true;
        }
        /// <summary>
        /// 是否支付得起开销
        /// </summary>
        public bool IsCanAfford(IEnumerable<Produce> produces)
        {
            foreach (var c in produces)
            {
                if (MyServices.BagDataHelper.TestItem(c.ItemId, -c.Amount) is false)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 支付开销
        /// </summary>
        public void Afford(IEnumerable<Produce> produces)
        {
            foreach (var c in produces)
            {
                MyServices.BagDataHelper.ChangeItem(c.ItemId, c.Amount);
            }
        }

        /// <summary> 应用建筑产出 </summary>
        public void TryApplyBuildingOutput(Dictionary<string, int> sum)
        {
            if (MyServices.Database.OnNewDay is true)
            {
                MyServices.Database.OnNewDay = false;

                var filterSum = new Dictionary<string, int>();
                foreach(var x in sum)
                {
                    if(ItemHelper.GetItem(x.Key).ItemType == GameItemType.Resources)
                    {
                        filterSum.Add(x.Key, x.Value);
                    }
                }

                foreach (var x in filterSum)
                {
                    MyServices.BagDataHelper.ChangeItem(x.Key, x.Value);
                }
            }
        }
        public Transform GenerateGachaDisplay(Operator opInfo, Vector3 pos, Vector3 angle)
        {
            var prefab = ResourceManager.Load<GameObject>("Characters/GachaDisplayer");
            var go = Instantiate(prefab, transform);
            GetComponent<FbxLoadManager>().LoadModel(opInfo.ModelResourceUrl, go.transform.Find("modelRoot"), false);
            go.transform.position = pos;
            go.transform.eulerAngles = angle;

            return go.transform;
        }
        public Transform GenerateGachaBaseDisplay(Operator opInfo, Vector3 pos, Vector3 angle)
        {
            var prefab = ResourceManager.Load<GameObject>("Characters/Displayer");
            var go = Instantiate(prefab, transform);
            GetComponent<FbxLoadManager>().LoadModel(opInfo.ModelResourceUrl, go.transform, false);
            go.transform.position = pos;
            go.transform.eulerAngles = angle;

            return go.transform;
        }
        public Transform GenerateDisplay(Operator opInfo, Transform transform, bool withGun)
        {
            var prefab = ResourceManager.Load<GameObject>("Characters/PureDisplayer");
            var go = Instantiate(prefab, transform);
            GetComponent<FbxLoadManager>().LoadModel(opInfo.ModelResourceUrl, go.transform, withGun);
            return go.transform;
        }

        public void GoToLevel(LevelRule rule)
        {
            if(rule.IsActive is false)
            {
                throw new ArgumentException($"The rule:{rule.LevelName} not match conditions");
            }


            if(rule is CombatLevelRule combatRule)
            {
                MyServices.Database.CurCombatLevelInfo = CombatLevelGenerator.GeneratorLevelInfo(combatRule);
            }
            else if(rule is EventLevelRule eventLevel)
            {
                // TODO: 结算
                MyServices.GameDataHelper.FinishLevel(eventLevel);
            }

            SceneLoadHelper.MyLoadSceneAsync(rule.JumpScene.ToString());
        }

        public IList<Operator> GetDecorationOperator()
        {
            return MyServices.OpDataHelper.Operators;
        }

        public void CreateHomeMessage(string title, string desc, string spriteUrl = null)
        {
            Sprite sp = null;
            if (spriteUrl != null)
            {
                sp = ResourceManager.Load<Sprite>(spriteUrl);
            }
            HomeMessageUI.CreateNewHomeMessage(title, desc, sp);
        }

        public class ViewModel
        {
            /// <summary> 全局时间 </summary>
            public int GTime { get; private set; }

            public bool IsDay { get; private set; }

            public bool IsInInvade { get; private set; }

            public bool OperatorListDirtyMark { get; set; }

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
                SetData();
                MyServices.BagDataHelper.OnBagDataChange += SetData;
            }

            ~ViewModel()
            {
                MyServices.BagDataHelper.OnBagDataChange -= SetData;
            }

            public void SetData()
            {
                GTime = MyServices.GameDataHelper.GetTime();
                IsDay = MyServices.GameDataHelper.IsDay();
                IsInInvade = MyServices.GameDataHelper.IsInvasion();
                Population = MyServices.OpDataHelper.Operators.Count;
                ResElectric = MyServices.BagDataHelper.GetItemNum(MyConfig.ItemTable.Electric.ToString());
                ResIron = MyServices.BagDataHelper.GetItemNum(MyConfig.ItemTable.Iron.ToString());
                ResAmmo = MyServices.BagDataHelper.GetItemNum(MyConfig.ItemTable.Ammo.ToString());
                ResAl = MyServices.BagDataHelper.GetItemNum(MyConfig.ItemTable.Al.ToString());
                ResGacha = MyServices.BagDataHelper.GetItemNum(MyConfig.ItemTable.Red.ToString());
            }
        }
    }
}
