using Assets.Scripts.PrepareLogic.UILogic;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            SwithPage(1);
        }

        public void SwithPage(int page)
        {
            
            StartCoroutine(SwithPageAsync(page));
            

        }
        IEnumerator SwithPageAsync(int page)
        {
            // to edit page
            // 1. invisable portrait list
            // 2. dotween anime
            // 3. visable portrait list
            // 4. change onclick event

            // 1.invisable portrait list
            windows["PortraitsScrollView"].gameObject.SetActive(false);

            // 2. dotween anime
            //DOTween.To()
            windows["TeammatePanel"].GetComponent<RectTransform>().DOSizeDelta(new Vector2(500, 780f), 1);
            windows["EnemyPanel"].GetComponent<RectTransform>().DOSizeDelta(new Vector2(0, 780f), 1);
            yield return new WaitForSeconds(1);

            // 3.visable portrait list
            windows["PortraitsScrollView"].gameObject.SetActive(true);

            // 4. change onclick event
        }
    }
}
