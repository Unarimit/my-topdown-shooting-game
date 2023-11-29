using Assets.Scripts.PrepareLogic;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Common
{
    public class HoverToolTipUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string ToolTipText;
        public float EnterDelay = 0.5f;

        private GameObject _tipGo;
        private RectTransform _tipPanel;
        private TextMeshProUGUI _tipText;
        public static HoverToolTipUI CreateHoverToolTip(Transform transform, string toolTiptext, float enterDelay)
        {
            var res = transform.AddComponent<HoverToolTipUI>();
            res.ToolTipText = toolTiptext;
            res.EnterDelay = enterDelay;
            return res;
        }
        private void Start()
        {
            _tipGo = Instantiate(ResourceManager.Load<GameObject>("UIs/TipPanel"), transform.root);
            _tipPanel = _tipGo.GetComponent<RectTransform>();
            _tipText = _tipGo.transform.Find("TextTMP").GetComponent<TextMeshProUGUI>();
            _tipGo.SetActive(false);

            _tipText.text = ToolTipText;
            StartCoroutine(setSizeDelay());

        }
        bool enter = false;
        public void OnPointerEnter(PointerEventData eventData)
        {
            enter = true;
            DOVirtual.DelayedCall(EnterDelay, () =>
            {
                _tipGo.SetActive(enter);
            });
        }
        IEnumerator setSizeDelay() // 等待布局计算
        {
            yield return new WaitForEndOfFrame();
            _tipPanel.position = new Vector2(transform.position.x, transform.position.y);
            _tipPanel.sizeDelta = new Vector2(_tipPanel.rect.width, _tipText.preferredHeight + 20f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            enter = false;
            _tipGo.SetActive(false);
        }
    }
}
