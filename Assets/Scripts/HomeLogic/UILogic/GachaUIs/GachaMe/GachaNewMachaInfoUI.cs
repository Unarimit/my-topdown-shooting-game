using Assets.Scripts.Entities.Mechas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.GachaUIs.GachaMe
{
    internal class GachaNewMachaInfoUI : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_mechaPartName;
        [SerializeField]
        TextMeshProUGUI m_mechaPartProperties;
        [SerializeField]
        RawImage m_mechaPartRawImage;

        MechaBase me;

        private void OnEnable()
        {
            if(me == null) gameObject.SetActive(false);
        }
        public void ShowMecha(MechaBase mecha)
        {
            me = mecha;
            gameObject.SetActive(true);

            m_mechaPartName.text = mecha.Name;
            m_mechaPartRawImage.texture = ResourceManager.Load<Texture2D>("Textures/" + mecha.IconUrl);
            m_mechaPartProperties.text = mecha.ToString();
        }
        private void OnDisable()
        {
            me = null;
        }
    }
}
