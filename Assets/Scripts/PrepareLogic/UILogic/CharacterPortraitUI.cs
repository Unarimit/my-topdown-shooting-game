using Assets.Scripts.PrepareLogic.EffectLogic;
using Assets.Scripts.PrepareLogic.PrepareEntities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PrepareLogic.UILogic
{
    /// <summary>
    /// 选择角色时的一个角色框，代表这个角色框参与的所有行为
    /// </summary>
    public class CharacterPortraitUI : MonoBehaviour
    {
        public RawImage PortraitImage;
        public TextMeshProUGUI CharacterNameTMP;
        public Button Button;


        private Image _panelImage;
        private TeammatePortraitPage _page; 

        private PrepareOperator _model;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="page"></param>
        public void Inject(PrepareOperator model, TeammatePortraitPage page)
        {
            _model = model;
            PortraitImage.texture = PhotographyManager.Instance.GetCharacterPortrait(_model.OpInfo.ModelResourceUrl);
            CharacterNameTMP.text = _model.OpInfo.Name;
            _page = page;


            if (_page == TeammatePortraitPage.ChoosePage)
            {
                setChoose(_model.IsChoose);
            }
        }
        public void ChangePage(TeammatePortraitPage page)
        {
            _page = page;
            if (_page == TeammatePortraitPage.ChoosePage)
            {
                setChoose(_model.IsChoose);
            }
            else
            {
                setChoose(false);
            }
        }


        private void Awake()
        {
            _panelImage = GetComponent<Image>();
        }
        private void Start()
        {
            Button.onClick.AddListener(onClick);
            UIManager.Instance.SwtichPageEvent += ChangePage;

        }
        private void OnDestroy()
        {
            Button.onClick.RemoveAllListeners();
            UIManager.Instance.SwtichPageEvent -= ChangePage;
        }


        private void setChoose(bool choose)
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
                setChoose(_model.IsChoose);
            }
            else if(_page == TeammatePortraitPage.EditPage)
            {

            }
        }
    }
}
