using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.HomeLogic.Interface;
using System.Collections.Generic;
using TowerDefense.Towers.Placement;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.Placement
{
    internal class PlacementManager : MonoBehaviour, ISwitchUI
    {
        /// <summary> 放置区域 </summary>
        TowerPlacementGrid[] placementArea;
        Transform placeTrans;
        private void Awake()
        {
            placementArea = new TowerPlacementGrid[7];
            var taTrans = transform.Find("TAPlacement");
            placementArea[0] = taTrans.Find("Tile (0,0)").GetComponent<TowerPlacementGrid>();
            placementArea[1] = taTrans.Find("Tile (0,1)").GetComponent<TowerPlacementGrid>();
            placementArea[2] = taTrans.Find("Tile (1,0)").GetComponent<TowerPlacementGrid>();
            placementArea[3] = taTrans.Find("Tile (1,1)").GetComponent<TowerPlacementGrid>();
            placementArea[4] = taTrans.Find("Tile (2,0)").GetComponent<TowerPlacementGrid>();
            placementArea[5] = taTrans.Find("Tile (2,1)").GetComponent<TowerPlacementGrid>();

            placementArea[6] = transform.Find("BAPlacement").Find("Tile (0,0)").GetComponent<TowerPlacementGrid>();

            placeTrans = transform.Find("Buildings");
        }
        /// <summary>
        /// 在流程初始化时调用，确保在awake后执行
        /// </summary>
        public void Init(BuildingArea area, IDictionary<string, Building> buildingMap)
        {
            if(placementArea == null)
            {
                Debug.LogWarning("plz assure init function is clalled afeter awake function");
                return;
            }

            foreach(var info in area.PlaceInfos)
            {
                var b = buildingMap[info.BuildingId];
                var go = Instantiate(ResourceManager.Load<GameObject>("Buildings/" + b.ModelUrl), placeTrans);
                go.transform.position = placementArea[info.AreaIndex].GridToWorld(info.PlacePosition, b.Dimensions);
            }
            foreach(var x in placementArea)
            {
                x.gameObject.SetActive(false);
            }
        }

        public void Enter()
        {
            foreach (var x in placementArea)
            {
                x.gameObject.SetActive(true);
            }
        }

        public void Quit()
        {
            foreach (var x in placementArea)
            {
                x.gameObject.SetActive(false);
            }
        }

    }
}
