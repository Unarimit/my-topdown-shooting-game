using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CombatLogic.UILogic
{
    public class GameOverUI : SubUIBase
    {
        public TextMeshProUGUI EndText;
        public Button ReplayButton;

        public static GameOverUI Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }
        private void Start()
        {
            ReplayButton.onClick.AddListener(ButtonOnClickEvent);
        }
        public void ButtonOnClickEvent()
        {
            CombatContextManager.Instance.QuitScene();
        }
        public void ShowWinText()
        {
            EndText.text = "Mission Complete!";
            EndText.color = Color.yellow;
            gameObject.SetActive(true);
        }
        public void ShowLossText()
        {
            EndText.text = "Mission Failed!";
            EndText.color = Color.red;
            gameObject.SetActive(true);
        }
    }
}
