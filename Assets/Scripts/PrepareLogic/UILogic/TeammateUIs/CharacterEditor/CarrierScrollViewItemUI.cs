using Assets.Scripts.Common;
using Assets.Scripts.Common.UI.OperatorChooseUIs;
using Assets.Scripts.Entities;
using Assets.Scripts.PrepareLogic.EffectLogic;
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
            showFighterIcon();

            // 设定逻辑在流程特别里面的地方，可视化不佳，不好修改。没有使用MVC导致的。
            GetComponent<Button>().onClick.AddListener(async () =>
            {
                _model.Operator =  await OperatorChooseUI.ChooseOperator(MyServices.Database.Operators);
                showFighterIcon();
                EditRoomManager.Instance.Refresh();
            });
            
        }

        private void showFighterIcon()
        {
            if (_model.Operator == null)
            {
                CharacterNameTMP.text = "未选择";
            }
            else
            {
                PortraitImage.texture = PhotographyManager.GetOperatorHeadIcon(_model.Operator);
                CharacterNameTMP.text = _model.Operator.Name;
            }
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
