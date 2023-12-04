using Cinemachine;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.EnviormentLogic
{
    internal class StrategyMapController : MonoBehaviour
    {
        public Canvas m_MapRenderCanvas;
        public bl_MiniMap m_MiniMap;
        public CinemachineVirtualCamera m_VirtualCamera;
        public bool IsDestory = false;
        public static StrategyMapController CreateStrategyMap()
        {
            var pre = ResourceManager.Load<GameObject>("UIs/StrategyMap");
            var go = Instantiate(pre);
            return go.GetComponent<StrategyMapController>();
        }

        public void Destroy()
        {
            IsDestory = true;
            m_VirtualCamera.Priority = 0;
            m_MiniMap.SetActive(false);
            Destroy(m_MiniMap.gameObject);
            Destroy(gameObject, 1);
        }

        Camera mapCam;
        private void Awake()
        {
            mapCam = Camera.main.transform.Find("UICamera").GetComponent<Camera>();
        }
        private void Start()
        {
            m_MapRenderCanvas.worldCamera = mapCam;
            m_MiniMap.SetToFullscreenSize();
        }

    }
}
