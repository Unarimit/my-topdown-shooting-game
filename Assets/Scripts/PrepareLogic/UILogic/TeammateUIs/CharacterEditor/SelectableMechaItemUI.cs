using Assets.Scripts.Common;
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

namespace Assets.Scripts.PrepareLogic.UILogic.TeammateUIs.CharacterEditor
{
    internal class SelectableMechaItemUI : MonoBehaviour
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
            GetComponent<ImageButtonUI>().RawImage.texture = ResourceManager.Load<Texture2D>("Textures/" + mecha.IconUrl);
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
            return Mecha.Name + "\n" + Mecha.ToString();
        }


        private void Start()
        {
            // add tip panel
            HoverToolTipUI.CreateHoverToolTip(transform, GenerateText(), 0);
        }

    }
}
