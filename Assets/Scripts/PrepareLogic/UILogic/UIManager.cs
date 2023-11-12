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

            windows["CharacterEditorPanel"].gameObject.SetActive(false);
        }

        //TODO: Enum it!
        private TeammatePortraitPage page = TeammatePortraitPage.ChoosePage;
        public void SwithPage()
        {
            if(page == TeammatePortraitPage.ChoosePage)
            {
                page = TeammatePortraitPage.EditPage;
                StartCoroutine(SwithToEditorPageAsync());
            }
            else
            {
                page = TeammatePortraitPage.ChoosePage;
                StartCoroutine(SwithToMainPageAsync());
            }
            

        }
        public delegate void SwtichPageHandler(TeammatePortraitPage page);
        public event SwtichPageHandler SwtichPageEvent; // invoke end anime
        IEnumerator SwithToEditorPageAsync()
        {
            // to edit page
            // 1.invisable portrait list
            windows["PortraitsScrollView"].gameObject.SetActive(false);

            // 2. dotween anime
            windows["TeammatePanel"].GetComponent<RectTransform>().DOSizeDelta(new Vector2(500, 780f), 1);
            windows["EnemyPanel"].GetComponent<RectTransform>().DOSizeDelta(new Vector2(0, 780f), 1);
            yield return new WaitForSeconds(1);

            // 3.visable portrait list
            SwtichPageEvent.Invoke(page);
            windows["PortraitsScrollView"].Enter();

            // 4. change onclick event

            // 5. set editor visiable
            windows["CharacterEditorPanel"].Enter();
        }
        IEnumerator SwithToMainPageAsync()
        {
            // 1.invisable portrait list
            windows["PortraitsScrollView"].gameObject.SetActive(false);


            // 2. set editor invisiable
            windows["CharacterEditorPanel"].Quit();
            yield return new WaitForSeconds(0.2f);

            // 3. dotween anime
            windows["TeammatePanel"].GetComponent<RectTransform>().DOSizeDelta(new Vector2(1300, 780f), 1);
            windows["EnemyPanel"].GetComponent<RectTransform>().DOSizeDelta(new Vector2(618, 780f), 1);
            yield return new WaitForSeconds(1);

            // 4.visable portrait list
            SwtichPageEvent.Invoke(page);
            windows["PortraitsScrollView"].Enter();

            // 5. change onclick event

        }
    }
}
