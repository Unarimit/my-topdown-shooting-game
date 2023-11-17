using Assets.Scripts.PrepareLogic.EffectLogic;
using Assets.Scripts.PrepareLogic.PrepareEntities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PrepareLogic.UILogic.TeammateUIs.CharacterSelector
{
    /// <summary>
    /// 选择角色时的一个角色框，代表这个角色框参与的所有行为
    /// </summary>
    public class CharacterPortraitScrollViewItemUI : MonoBehaviour
    {
        public RawImage PortraitImage;
        public TextMeshProUGUI CharacterNameTMP;
        public Button Button;
        public TextMeshProUGUI CharacterTypeTMP;


        private Image _panelImage;
        private TeammatePortraitPage _page; 

        private PrepareOperator _model;

        private CharacterPortraitScrollViewUI _parent;
        public void Inject(PrepareOperator model, TeammatePortraitPage page, CharacterPortraitScrollViewUI parent)
        {
            _model = model;
            PortraitImage.texture = PhotographyManager.Instance.GetCharacterPortrait(_model.OpInfo.ModelResourceUrl);
            CharacterNameTMP.text = _model.OpInfo.Name;
            CharacterTypeTMP.text = _model.OpInfo.Type.ToString();
            _page = page;
            _parent = parent;


            if (_page == TeammatePortraitPage.ChoosePage)
            {
                SetChoose(_model.IsChoose);
            }
        }
        public void ChangePage(TeammatePortraitPage page)
        {
            _page = page;
            if (_page == TeammatePortraitPage.ChoosePage)
            {
                SetChoose(_model.IsChoose);
            }
            else
            {
                SetChoose(false);
            }
        }


        private void Awake()
        {
            _panelImage = GetComponent<Image>();
        }
        private void Start()
        {
            Button.onClick.AddListener(onClick);
        }
        private void OnDestroy()
        {
            Button.onClick.RemoveAllListeners();
        }


        public void SetChoose(bool choose)
        {
            if (choose)
            {
                _panelImage.color = new Color(1, 1, 1, 150f / 255);
            }
            else
            {
                _panelImage.color = new Color(1, 1, 1, 60f / 255);
            }
        }
        private void onClick()
        {
            if(_page == TeammatePortraitPage.ChoosePage)
            {
                _model.IsChoose = !_model.IsChoose;
                SetChoose(_model.IsChoose);
            }
            else if(_page == TeammatePortraitPage.EditPage)
            {
                _parent.InEditSelect(this, _model);
                SetChoose(true);
            }
        }
    }
}
