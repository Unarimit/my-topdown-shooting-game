using Assets.Scripts.PrepareLogic.UILogic;
using Assets.Scripts.PrepareLogic.UILogic.TeammateUIs;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic
{
    public enum TeammatePortraitPage
    {
        ChoosePage,
        EditPage
    }
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        public Transform CanvasRoot;

        public TeammatePortraitPage Page = TeammatePortraitPage.ChoosePage;

        private Dictionary<string, PrepareUIBase> windows;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }
        private void Start()
        {
            windows = new Dictionary<string, PrepareUIBase>();
            var uis = FindObjectsOfType<PrepareUIBase>();
            foreach (var x in uis)
            {
                windows.Add(x.name, x);
            }

        }

        
        public void SwithPage()
        {
            if(Page == TeammatePortraitPage.ChoosePage)
            {
                Page = TeammatePortraitPage.EditPage;
            }
            else
            {
                Page = TeammatePortraitPage.ChoosePage;
            }

            windows["TeammatePanel"].Refresh();
            windows["EnemyPanel"].Refresh();
        }
        
    }
}
