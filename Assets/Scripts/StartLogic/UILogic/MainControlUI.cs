using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.StartLogic.UILogic
{
    internal class MainControlUI : MonoBehaviour
    {
        [SerializeField]
        Button m_loadSaveButton;
        private void Awake()
        {
            if(MyServices.Database.SaveAbstracts.Count == 0)
            {
                m_loadSaveButton.interactable = false;
            }
            else
            {
                m_loadSaveButton.interactable = true;
                m_loadSaveButton.onClick.AddListener(() => transform.Find("SaveDataPanel").gameObject.SetActive(true));
            }
            
        }
        private void OnDestroy()
        {
            m_loadSaveButton.onClick.RemoveAllListeners();
        }
    }
}
