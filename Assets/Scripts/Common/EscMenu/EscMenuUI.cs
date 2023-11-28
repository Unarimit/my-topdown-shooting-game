using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Common.EscMenu
{
    internal class EscMenuUI : MonoBehaviour
    {
        // 退出时会销毁这些button对象和他们的事件监听吗？
        [HideInInspector]
        public Button ReturnBtn;
        [HideInInspector]
        public Button EnviormentBtn;
        [HideInInspector]
        public Button QuitCombatBtn;
        [HideInInspector]
        public Button ReturnStartBtn;
        [HideInInspector]
        public RectTransform MenuPanelTrans;
        internal static EscMenuUI OpenEscMenuUI()
        {
            var prefab = ResourceManager.Load<GameObject>("UIs/EscMenuCanvas");
            var go = Instantiate(prefab);

            return go.GetComponent<EscMenuUI>();
        }
        private Vector2 sizeDelta;
        private void Awake()
        {
            ReturnBtn = transform.Find("MenuPanel").Find("ReturnBtn").GetComponent<Button>();
            QuitCombatBtn = transform.Find("MenuPanel").Find("QuitCombatBtn").GetComponent<Button>();
            EnviormentBtn = transform.Find("MenuPanel").Find("EnviormentBtn").GetComponent<Button>();
            ReturnStartBtn = transform.Find("MenuPanel").Find("ReturnStartBtn").GetComponent<Button>();
            MenuPanelTrans = transform.Find("MenuPanel").GetComponent<RectTransform>();

            sizeDelta = MenuPanelTrans.sizeDelta;
            MenuPanelTrans.sizeDelta = Vector2.zero;

            ReturnBtn.onClick.AddListener(quit);
            ReturnStartBtn.onClick.AddListener(quitToStart);
        }
        private void Start()
        {
            // 菜单出现受timescale影响会出问题
            MenuPanelTrans.DOSizeDelta(sizeDelta, 0.2f).SetUpdate(UpdateType.Normal, true);
        }
        private void quit()
        {
            MenuPanelTrans.DOSizeDelta(Vector2.zero, 0.2f).OnComplete(() => Destroy(gameObject));
        }
        private void quitToStart()
        {
            SlideUI.CreateSlideUI();
            SceneManager.LoadScene("Start");
        }
    }
}
