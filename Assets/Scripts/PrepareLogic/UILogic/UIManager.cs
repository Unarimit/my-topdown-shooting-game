using Assets.Scripts.PrepareLogic.UILogic;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;


        private Dictionary<string, PrepareUIBase> windows;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");

            windows = new Dictionary<string, PrepareUIBase>();
        }
        private void Start()
        {
            var uis = FindObjectsOfType<PrepareUIBase>();
            foreach (var x in uis)
            {
                windows.Add(x.name, x);
            }
        }
    }
}
