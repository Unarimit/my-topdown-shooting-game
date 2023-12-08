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
        public MechaBase Mecha { get; private set; }
        public bool IsSelect
        {
            get => _isSelect;
            set
            {
                _isSelect = value;
                if (_selectRim == null) return;
                _selectRim.color = value ? Color.yellow : Color.white;
            }
        }
        public bool CanSelete
        {
            get => _canSelect;
            set
            {
                _canSelect = value;
                _selectRim.color = value ? Color.white : Color.gray;
            }
        }


        private bool _isSelect;
        private bool _canSelect;
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
            CanSelete = true;
        }
        private void OnClick()
        {
            if(_canSelect is true) _panel.ItemOnClick(_index);
            else TipsUI.GenerateNewTips("不可以选择已经被选择的Mecha");
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
