using Assets.Scripts.Common;
using Assets.Scripts.Common.EscMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.StartLogic
{
    internal class StartContextManager : MonoBehaviour
    {
        public static StartContextManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
            Time.timeScale = 1;
        }

        // 简单的逻辑就直接用unity event控制

        public void StartNewGame() // 简单
        {
            SlideUI.CreateSlideUI();
            SceneLoadHelper.MyLoadSceneAsync("Home");
        }

        public void OpenSettings() // 简单
        {
            EnviorSettingUI.OpenEnviorSettingUI();
        }

        public void QuitGame() // 简单
        {
            Application.Quit();
        }


        public void LoadGame(string saveId)
        {
            MyServices.Database.LoadSaveData(saveId);
            SlideUI.CreateSlideUI();
            SceneLoadHelper.MyLoadSceneAsync("Home");
        }
    }
}
