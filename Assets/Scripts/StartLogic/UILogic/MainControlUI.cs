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
            m_loadSaveButton.onClick.AddListener(() => transform.Find("SaveDataPanel").gameObject.SetActive(true));
        }
        private void OnDestroy()
        {
            m_loadSaveButton.onClick.RemoveAllListeners();
        }
    }
}
