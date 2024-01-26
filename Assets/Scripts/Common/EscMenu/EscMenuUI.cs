using Assets.Scripts.Common.Interface;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common.EscMenu
{
    internal class EscMenuUI : MonoBehaviour, IOverlayUI
    {
        // 退出时会销毁这些button对象和他们的事件监听吗？
        [HideInInspector]
        public Button ReturnBtn;
        Button enviormentBtn;
        Button quitCombatBtn;
        Button saveGameBtn;
        Button returnStartBtn;
        RectTransform menuPanelTrans;
        EnviorSettingUI enviorSettingUI = null;
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
            quitCombatBtn = transform.Find("MenuPanel").Find("QuitCombatBtn").GetComponent<Button>();
            enviormentBtn = transform.Find("MenuPanel").Find("EnviormentBtn").GetComponent<Button>();
            saveGameBtn = transform.Find("MenuPanel").Find("SaveGameBtn").GetComponent<Button>();
            returnStartBtn = transform.Find("MenuPanel").Find("ReturnStartBtn").GetComponent<Button>();
            menuPanelTrans = transform.Find("MenuPanel").GetComponent<RectTransform>();


            sizeDelta = menuPanelTrans.sizeDelta;
            menuPanelTrans.sizeDelta = Vector2.zero;

            ReturnBtn.onClick.AddListener(quit);
            returnStartBtn.onClick.AddListener(quitToStart);
            enviormentBtn.onClick.AddListener(openEnviormentSettingPanel);
            saveGameBtn.onClick.AddListener(saveGame);
        }
        private void Start()
        {
            // 菜单出现受timescale影响会出问题
            menuPanelTrans.DOSizeDelta(sizeDelta, 0.2f).SetUpdate(UpdateType.Normal, true);
        }
        private void quit()
        {
            menuPanelTrans.DOSizeDelta(Vector2.zero, 0.2f).OnComplete(() => Destroy(gameObject));
        }
        private void quitToStart()
        {
            SceneLoadHelper.MyLoadSceneAsync("Start");
        }
        private void openEnviormentSettingPanel()
        {
            if(enviorSettingUI == null) enviorSettingUI = EnviorSettingUI.OpenEnviorSettingUI();
        }
        private void saveGame()
        {
            if(MyServices.Database.SaveData() is true)
            {
                TipsUI.GenerateNewTips("保存成功");
            }
        }

        public void Quit()
        {
            if (enviorSettingUI != null) enviorSettingUI.Quit();
            quit();
        }
    }
}
