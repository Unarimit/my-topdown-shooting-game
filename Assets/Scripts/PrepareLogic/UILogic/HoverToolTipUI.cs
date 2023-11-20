using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.PrepareLogic.UILogic
{
    public class HoverToolTipUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string ToolTipText;
        public float EnterDelay;

        private GameObject _tipGo;
        private RectTransform _tipPanel;
        private TextMeshProUGUI _tipText;
        private void Start()
        {
            _tipGo = Instantiate(ResourceManager.Load<GameObject>("UIs/TipPanel"), UIManager.Instance.CanvasRoot);
            _tipPanel = _tipGo.GetComponent<RectTransform>();
            _tipText = _tipGo.transform.Find("TextTMP").GetComponent<TextMeshProUGUI>();
            _tipGo.SetActive(false);

            _tipText.text = ToolTipText;
            _tipPanel.position = new Vector2(transform.position.x, transform.position.y);
            _tipPanel.sizeDelta = new Vector2(_tipText.preferredWidth, _tipText.preferredHeight);

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _tipGo.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tipGo.SetActive(false);
        }
    }
}
