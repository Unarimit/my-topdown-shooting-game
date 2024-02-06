using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic
{
    internal class HomeMessageUI : MonoBehaviour
    {
        public TextMeshProUGUI m_Title;
        public TextMeshProUGUI m_Description;
        public Image m_ActionInfoImage;
        public Button m_ComfirmButton;

        public static HomeMessageUI CreateNewHomeMessage(string title, string desc, Sprite image = null)
        {
            var go = Instantiate(ResourceManager.Load<GameObject>("UIs/HomeMessageCanvas"));
            var comp = go.GetComponent<HomeMessageUI>();
            comp.m_Title.text = title;
            comp.m_Description.text = desc;
            if(image != null) comp.m_ActionInfoImage.sprite = image;
            return comp;
        }
        private void Awake()
        {
            m_ComfirmButton.onClick.AddListener(() => Destroy(gameObject));
        }
        private void OnDestroy()
        {
            m_ComfirmButton.onClick.RemoveAllListeners();
        }
    }
}
