using Assets.Scripts.Common;
using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.HomeLogic.Interface;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefense.Towers.Placement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Assets.Scripts.HomeLogic.Placement
{
    internal class PlacementManager : MonoBehaviour, ISwitchUI
    {
        /// <summary> 放置区域 </summary>
        TowerPlacementGrid[] placementArea;
        Transform placeTrans;
        PlacementController controller;
        BuildingArea model;

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

            controller = transform.Find("Placement").GetComponent<PlacementController>();
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
                placeBuilding(info, buildingMap[info.BuildingId]);
            }
            foreach(var x in placementArea)
            {
                x.gameObject.SetActive(false);
            }
            model = area;
        }
        /// <summary>
        /// 传入building为null表示取消放置状态
        /// </summary>
        /// <param name="building"></param>
        public void OnBuilding(Building building)
        {
            if(building == null)
            {
                controller.gameObject.SetActive(false);
            }
            else
            {
                controller.gameObject.SetActive(true);
                changeModel(building);
                controller.m_CurrentBuilding.Building = building;
            }
        }

        public void OnClick()
        {
            StartCoroutine(AvoidUIJudegeMiss());
            // For this warning:
            // Calling IsPointerOverGameObject() from within event processing (such as from InputAction callbacks) will not work as expected;

            IEnumerator AvoidUIJudegeMiss()
            {
                yield return null;
                if (controller.m_CurrentBuilding.Building == null || EventSystem.current.IsPointerOverGameObject()) yield break;

                var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                var hits = Physics.RaycastAll(ray, 100f, LayerMask.GetMask(new string[] { "BuildingGrid" }));
                if (hits.Length == 1 && controller.m_GhostPlacementPossible is true)
                {
                    var pa = hits[0].transform.GetComponent<TowerPlacementGrid>();
                    int i = 0;
                    // 找到位置
                    for (; i < placementArea.Length; i++)
                    {
                        if (pa == placementArea[i]) break;
                    }
                    // 放置
                    var pos = pa.WorldToGrid(hits[0].point, controller.m_CurrentBuilding.Building.Dimensions);
                    var info = new PlaceInfo { AreaIndex = i, PlacePosition = pos, BuildingId = controller.m_CurrentBuilding.Building.BuildingId };
                    placeBuilding(info, controller.m_CurrentBuilding.Building);

                    // 入库
                    model.PlaceInfos = model.PlaceInfos.Append(info).ToArray();
                }
                else
                {
                    TipsUI.GenerateNewTips("请选择正确的放置位置");
                }
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

        private void changeModel(Building building)
        {
            // clear old
            var tTrans = controller.transform.Find("tower");
            for(int i = 0; i < tTrans.childCount; i++)
            {
                Destroy(tTrans.GetChild(i).gameObject);
            }

            var go = Instantiate(ResourceManager.Load<GameObject>("Buildings/" + building.ModelUrl), tTrans);
            go.transform.localPosition = Vector3.zero;
        }

        private void placeBuilding(PlaceInfo info, Building b)
        {
            var go = Instantiate(ResourceManager.Load<GameObject>("Buildings/" + b.ModelUrl), placeTrans);
            go.transform.position = placementArea[info.AreaIndex].GridToWorld(info.PlacePosition, b.Dimensions);
            placementArea[info.AreaIndex].Occupy(info.PlacePosition, b.Dimensions);
        }
    }
}
