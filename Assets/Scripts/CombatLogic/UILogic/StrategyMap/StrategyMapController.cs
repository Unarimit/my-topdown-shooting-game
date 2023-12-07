using Assets.Scripts.CombatLogic.ContextExtends;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CombatLogic.UILogic.StrategyMap
{
    internal class StrategyMapController : MonoBehaviour
    {
        #region inspector public
        public Canvas m_MapRenderCanvas;
        public bl_MiniMap m_MiniMap;
        public CinemachineVirtualCamera m_VirtualCamera;
        public Image m_FadeCanvas;
        #endregion

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
            // minimap
            IsDestory = true;
            m_VirtualCamera.Priority = 0;
            _context.CombatVM.PlayerChangeEvent -= setPlayer;
            m_MiniMap.SetActive(false);

            // team Panel
            teamPanel.GetComponent<CanvasGroup>().DOFade(0, 0.5f);

            Destroy(gameObject, 1);

        }

        Camera mapCam;
        Transform teamPanel;
        private void Awake()
        {
            // miniMap
            mapCam = Camera.main.transform.Find("UICamera").GetComponent<Camera>();
            m_MiniMap.minimapRig.position = new Vector3(_context.CombatVM.Level.Map.Length / 2, 0, _context.CombatVM.Level.Map[0].Length / 2); ;
            m_MiniMap.SetAsActiveMiniMap();
            setPlayer();
            _context.CombatVM.PlayerChangeEvent += setPlayer;
            m_FadeCanvas.DOFade(0.8f, 0.5f);

            // team Panel
            transform.Find("OpStatusCanvas").GetComponent<Canvas>().worldCamera = mapCam;
            transform.Find("OpStatusCanvas").GetComponent<Canvas>().planeDistance = 19;
            teamPanel = transform.Find("OpStatusCanvas").Find("OpsPanel");
            teamPanel.Find("Scroll View").GetComponent<SmOperatorScrollViewUI>().Inject(_context.FindCombatOperators(x => x.Team == 0));
            var cg = teamPanel.GetComponent<CanvasGroup>();
            cg.alpha = 0;
            cg.DOFade(1, 0.5f);
            
        }
        private void Start()
        {
            m_MapRenderCanvas.worldCamera = mapCam;
            m_MapRenderCanvas.planeDistance = 20;
            m_MiniMap.SetToFullscreenSize();
        }
        private void setPlayer()
        {
            m_MiniMap.Target = _context.CombatVM.PlayerTrans;
        }

    }
}
