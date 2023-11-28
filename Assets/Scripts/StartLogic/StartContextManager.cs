using Assets.Scripts.Common;
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
        private void Awake()
        {
            Time.timeScale = 1;
        }

        public void StartGame()
        {
            SlideUI.CreateSlideUI();
            SceneManager.LoadScene("home");
        }
    }
}
