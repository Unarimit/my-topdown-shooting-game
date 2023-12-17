using Assets.Scripts.Common;
using Assets.Scripts.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.PrepareLogic.UILogic.TeammateUIs.CharacterEditor
{
    internal class CarrierScrollViewItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RawImage PortraitImage;
        public TextMeshProUGUI CharacterNameTMP;
        public TextMeshProUGUI CharacterTypeTMP;
        private Fighter _model;
        public void Inject(Fighter fi)
        {
            _model = fi;
            PortraitImage.texture = PhotographyManager.GetOperatorHeadIcon(_model.Operator);
            CharacterNameTMP.text = _model.Operator.Name;
            CharacterTypeTMP.text = _model.Type.To2WordsString();
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        public void OnPointerEnter(PointerEventData eventData)
        {

        }
    }
}
