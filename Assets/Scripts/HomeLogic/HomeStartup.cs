using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.HomeLogic.Placement;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HomeLogic
{
    internal class HomeStartup : MonoBehaviour
    {
        private void Start()
        {
            // place building
            var bDic = new Dictionary<string, Building>();
            foreach(var x in MyServices.Database.Buildings)
            {
                bDic.Add(x.BuildingId, x);
            }
            var pm = transform.Find("Placement").GetComponent<PlacementManager>();
            pm.Init(MyServices.Database.BuildingArea, bDic);
            GetComponent<HomeLogic.UILogic.UIManager>().Inject(pm);
        }
    }
}
