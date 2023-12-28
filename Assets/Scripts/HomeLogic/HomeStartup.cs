using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.HomeLogic.Environment;
using Assets.Scripts.HomeLogic.Environment.OperatorDecoration;
using Assets.Scripts.HomeLogic.Placement;
using Assets.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HomeLogic
{
    internal class HomeStartup : MonoBehaviour
    {
        [SerializeField]
        LightManager lightManager;
        private void Start()
        {
            // 根据存档防止建筑
            var bDic = new Dictionary<string, Building>();
            foreach(var x in MyServices.Database.Buildings)
            {
                bDic.Add(x.BuildingId, x);
            }
            var pm = transform.Find("Placement").GetComponent<PlacementManager>();
            pm.Init(MyServices.Database.BuildingArea, bDic);
            GetComponent<HomeLogic.UILogic.UIManager>().Inject(pm);
            GetComponent<HomeLogic.UILogic.UIManager>().SwitchPage(UILogic.HomePage.MainView);

            // 计算产出
            var sum = CalculateOutput(bDic);
            if(MyServices.Database.OnNewDay is true) GetComponent<HomeLogic.UILogic.UIManager>().DisplayOutput(sum);
            HomeContextManager.Instance.TryApplyBuildingOutput(sum);

            // 放置装饰角色
            DecorationManager.Instance.PlaceOperatorDecoration();

            // 白天还是黑夜
            if (HomeContextManager.Instance.HomeVM.IsDay)
            {
                lightManager.Day();
            }
            else
            {
                lightManager.Night();
            }

            // 处理HomeMessages事件
            while (MyServices.Database.HomeMessages.Count != 0 && MyServices.Database.HomeMessages.Peek().Day <= MyServices.GameDataHelper.DayNow)
            {
                var act = MyServices.Database.HomeMessages.Pop();
                act.MessageAction(HomeContextManager.Instance);
            }
            
        }

        private Dictionary<string, int> CalculateOutput(Dictionary<string, Building> bDic)
        {
            var sum = new Dictionary<string, int>();
            foreach(var x in MyServices.Database.BuildingArea.PlaceInfos)
            {
                var b = bDic[x.BuildingId];
                if(b is ResourceBuilding)
                {
                    foreach(var p in ((ResourceBuilding)b).Produces)
                    {
                        if (sum.ContainsKey(p.ItemId) is false) sum.Add(p.ItemId, 0);
                        sum[p.ItemId] += p.Amount;
                    }
                }
            }
            return sum;
        }
    }
}
