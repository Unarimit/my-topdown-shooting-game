using Assets.Scripts.Entities.Mechas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.PrepareLogic.UILogic.TeammateUIs
{
    internal class SelectableMechaItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool IsSelect
        {
            get => _isSelect;
            set
            {
                _isSelect = value;
                if (_selectRim == null) return;
                _selectRim.color = value ? Color.yellow : new Color(0, 0, 0, 0);
            }
        }
        public MechaBase Mecha { get; private set; }
        private bool _isSelect;
        private int _index;
        private Image _selectRim;
        private CharacterEditorUI.MechaSelectPanel _panel;
        public void Inject(MechaBase mecha, int i, CharacterEditorUI.MechaSelectPanel panel)
        {
            GetComponent<ImageButtonUI>().Button.onClick.AddListener(OnClick);
            GetComponent<ImageButtonUI>().RawImage.texture = Resources.Load<Texture2D>("Textures/" + mecha.IconUrl);
            _selectRim = GetComponent<Image>();

            Mecha = mecha;
            IsSelect = false;
            _index = i;
            _panel = panel;
        }
        private void OnClick()
        {
            _panel.ItemOnClick(_index);
            // TODO：通过事件丢给上一层去判断
        }
        private string GenerateText()
        {
            return Mecha.GetMechaType().ToString() + "\n" + Mecha.ToString();
        }


        // Tip Panel Logic
        private GameObject _tipGo;
        private RectTransform _tipPanel;
        private TextMeshProUGUI _tipText;
        private void Start()
        {
            _tipGo = Instantiate(Resources.Load<GameObject>("Effects/TipPanel"), UIManager.Instance.CanvasRoot);
            _tipPanel = _tipGo.GetComponent<RectTransform>();
            _tipText = _tipGo.transform.Find("TextTMP").GetComponent<TextMeshProUGUI>();
            _tipGo.SetActive(false);

            _tipText.text = GenerateText();
            StartCoroutine(setSizeDelay());
        }
        IEnumerator setSizeDelay() // 等待布局计算
        {
            yield return new WaitForEndOfFrame();
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
