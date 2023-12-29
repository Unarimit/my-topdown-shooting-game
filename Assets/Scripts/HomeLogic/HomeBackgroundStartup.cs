using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.HomeLogic.Environment;
using Assets.Scripts.HomeLogic.Environment.OperatorDecoration;
using Assets.Scripts.HomeLogic.Placement;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HomeLogic
{
    /// <summary>
    /// 作为背景时的启动流程
    /// </summary>
    internal class HomeBackgroundStartup : MonoBehaviour
    {

        [SerializeField]
        LightManager lightManager;
        private void Start()
        {
            // 根据存档防止建筑
            var bDic = MyServices.Database.Buildings;
            var pm = transform.Find("Placement").GetComponent<PlacementManager>();
            pm.Init(MyServices.Database.BuildingArea, bDic);

            // 随机拍摄
            DecorationManager.Instance.EnableRandomViewCamera();


            // 白天还是黑夜
            if (HomeContextManager.Instance.HomeVM.IsDay)
            {
                lightManager.Day();
            }
            else
            {
                lightManager.Night();
            }

        }
    }
}
