using Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.EnviormentLogic
{
    internal class StrategyMapController : MonoBehaviour
    {
        public Canvas m_MapRenderCanvas;
        public bl_MiniMap m_MiniMap;
        public CinemachineVirtualCamera m_VirtualCamera;
        public bool IsDestory { get; private set; } = false;

        private CombatContextManager _context = CombatContextManager.Instance;

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

            _context.CombatVM.PlayerChangeEvent -= setPlayer;

            m_MiniMap.SetActive(false);
            Destroy(gameObject, 1);

        }

        Camera mapCam;
        private void Awake()
        {
            mapCam = Camera.main.transform.Find("UICamera").GetComponent<Camera>();
            setPlayer();
            m_MiniMap.minimapRig.position = new Vector3(_context.CombatVM.Level.Map.Length/2, 0, _context.CombatVM.Level.Map[0].Length / 2);;
            m_MiniMap.SetAsActiveMiniMap();

            _context.CombatVM.PlayerChangeEvent += setPlayer;
        }
        private void Start()
        {
            m_MapRenderCanvas.worldCamera = mapCam;
            m_MiniMap.SetToFullscreenSize();
        }
        private void setPlayer()
        {
            m_MiniMap.Target = _context.CombatVM.PlayerTrans;
        }

    }
}
