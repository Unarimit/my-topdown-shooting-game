using Assets.Scripts.CombatLogic;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Test;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.Entities.Level;
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
                if (HomeVM.TestResource(c.ItemId, -c.Amount) is false)
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
                HomeVM.ChangeResource(c.ItemId, -c.Amount);
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
                    HomeVM.ChangeResource(x.Key, x.Value);
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
            return MyServices.Database.Operators;
        }

        private void OnDestroy()
        {
            HomeVM.Submit();
        }


        public class ViewModel
        {
            /// <summary> 全局时间 </summary>
            public int GTime { get; private set; } // 变化会涉及创景切换，所以不做数据绑定

            public bool IsDay { get; private set; }

            public bool IsInInvade { get; private set; }

            public bool OperatorListDirtyMark { get; set; }

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
                IsDay = MyServices.GameDataHelper.IsDay();
                IsInInvade = MyServices.GameDataHelper.IsInvation();
                Population.Data = MyServices.Database.Operators.Count;
                ResElectric.Data = MyServices.Database.Inventory[MyConfig.ItemTable.Electric.ToString()];
                ResIron.Data = MyServices.Database.Inventory[MyConfig.ItemTable.Iron.ToString()];
                ResAmmo.Data = MyServices.Database.Inventory[MyConfig.ItemTable.Ammo.ToString()];
                ResAl.Data = MyServices.Database.Inventory[MyConfig.ItemTable.Al.ToString()];
                ResGacha.Data = MyServices.Database.Inventory[MyConfig.ItemTable.Red.ToString()];
            }
            /// <summary> 提交到数据库 </summary>
            public void Submit()
            {
                MyServices.Database.Inventory[MyConfig.ItemTable.Electric.ToString()] = ResElectric.Data;
                MyServices.Database.Inventory[MyConfig.ItemTable.Iron.ToString()] = ResIron.Data;
                MyServices.Database.Inventory[MyConfig.ItemTable.Ammo.ToString()] = ResAmmo.Data;
                MyServices.Database.Inventory[MyConfig.ItemTable.Al.ToString()] = ResAl.Data;
                MyServices.Database.Inventory[MyConfig.ItemTable.Red.ToString()] = ResGacha.Data;
            }
            /// <summary>
            /// 在执行资源改变之前，先执行Test方法 <see cref="TestResource" /> 
            /// </summary>
            public void ChangeResource(string itemId, int diff)
            {
                var res = GetResourceById(itemId);
                res.Data += diff;
            }

            public bool TestResource(string itemId, int diff)
            {
                var res = GetResourceById(itemId);
                if (res.Data + diff < 0) return false;
                else return true;
            }

            private MyBinded<int> GetResourceById(string itemId)
            {
                var itemEnum = Enum.Parse<MyConfig.ItemTable>(itemId);
                MyBinded<int> res;
                if (itemEnum == MyConfig.ItemTable.Electric)
                {
                    res = ResElectric;
                }
                else if (itemEnum == MyConfig.ItemTable.Iron)
                {
                    res = ResIron;
                }
                else if (itemEnum == MyConfig.ItemTable.Ammo)
                {
                    res = ResAmmo;
                }
                else if (itemEnum == MyConfig.ItemTable.Al)
                {
                    res = ResAl;
                }
                else if (itemEnum == MyConfig.ItemTable.Red)
                {
                    res = ResGacha;
                }else
                {
                    throw new Exception("un match item id");
                }
                return res;
            }
        }
    }
}
