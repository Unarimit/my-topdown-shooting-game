using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using Assets.Scripts.PrepareLogic.PrepareEntities;
using Assets.Scripts.PrepareLogic.EffectLogic;
using TMPro;
using Assets.Scripts.PrepareLogic.UILogic.TeammateUIs;
using Assets.Scripts.Entities.Mechas;

namespace Assets.Scripts.PrepareLogic.UILogic
{
    public class CharacterEditorUI : MonoBehaviour
    {
        public RawImage m_EditCanvasRawImage;
        public RectTransform m_InfoPanelTrans;
        public GameObject m_SelectableMechaPrefab;

        // 数据模型
        private PrepareOperator _model;

        // 干员信息，名称、属性、技能图标等
        private TextMeshProUGUI _operatorName;
        private TextMeshProUGUI _operatorProperties;
        private RawImage _operatorGunImg;
        private RawImage _operatorSkillImg;

        // MechaInfo中的总结信息
        private TextMeshProUGUI _mechaBuffs;
        // MechaInfo中的部位信息
        private List<MechaPanel> _mechaPanels;
        // MechaInfo中的按钮
        private Button _mechaButton;

        // 选择Mecha的面板
        private MechaSelectPanel _mechaSelectPanel;
        private PrepareContextManager _context => PrepareContextManager.Instance;

        class MechaPanel
        {
            public RawImage PartRawImage;
            public TextMeshProUGUI PartName;
            public TextMeshProUGUI PartProperties;
        }

        class MechaSelectPanel
        {
            private Transform Transform;
            private CanvasGroup CanvasGroup;
            private Transform ContentTrans;
            private Button OKButton;
            private List<SelectableMecha> SelectableMechas = new List<SelectableMecha>();
            public MechaSelectPanel(Transform MechaSelectPanel)
            {
                Transform = MechaSelectPanel;
                CanvasGroup = MechaSelectPanel.GetComponent<CanvasGroup>();
                ContentTrans = MechaSelectPanel.Find("Scroll View").Find("Viewport").Find("Content");
                OKButton = MechaSelectPanel.Find("Button").GetComponent<Button>();
                OKButton.onClick.AddListener(SelectPanelQuit);
            }
            public void SelectPanelEnter(GameObject selectableMechaPrefab, PrepareOperator model)
            {
                Transform.gameObject.SetActive(true);
                CanvasGroup.alpha = 0;
                CanvasGroup.DOFade(1, 0.2f);
                InitContent(selectableMechaPrefab, model);
            }

            private void InitContent(GameObject selectableMechaPrefab, PrepareOperator model)
            {
                SelectableMechas.Clear();
                int len = ContentTrans.childCount;
                for (int i = 0; i < len; i++)
                {
                    Destroy(ContentTrans.GetChild(i).gameObject);  
                }

                SelectableMechas.Add(new SelectableMecha(selectableMechaPrefab, model.OpInfo.McHead, ContentTrans, SelectableMechas.Count));
                SelectableMechas.Add(new SelectableMecha(selectableMechaPrefab, model.OpInfo.McBody, ContentTrans, SelectableMechas.Count));
                SelectableMechas.Add(new SelectableMecha(selectableMechaPrefab, model.OpInfo.McLeg, ContentTrans, SelectableMechas.Count));

                for (int i = 0; i < 3; i++) SelectableMechas[i].IsSelect = true;

                foreach (var x in TestDB.GetMechas())
                {
                    SelectableMechas.Add(new SelectableMecha(selectableMechaPrefab, x, ContentTrans, SelectableMechas.Count));
                }
            }

            private void SelectPanelQuit()
            {
                Transform.gameObject.SetActive(false);
            }
        }

        class SelectableMecha
        {
            public bool IsSelect { 
                get => _isSelect; 
                set {
                    _isSelect = value;
                    if (SelectRim == null) return;
                    SelectRim.color = value ? Color.yellow : new Color(0, 0, 0, 0);
                } 
            }
            private bool _isSelect;
            public int index;
            public Image SelectRim;
            public SelectableMecha(GameObject selectableMechaPrefab, MechaBase mecha, Transform parent, int i)
            {
                var go = Instantiate(selectableMechaPrefab, parent);
                go.GetComponent<ImageButtonUI>().Button.onClick.AddListener(OnClick);
                go.GetComponent<ImageButtonUI>().RawImage.texture = Resources.Load<Texture2D>("Textures/" + mecha.IconUrl);
                SelectRim = go.GetComponent<Image>();
                IsSelect = false;
                index = i;
            }
            private void OnClick()
            {

            }
        }


        private void Awake()
        {
            _operatorName = transform.Find("InfoPanel").Find("OperatorPanel").Find("OperatorNameTMP").GetComponent<TextMeshProUGUI>();
            _operatorProperties = transform.Find("InfoPanel").Find("OperatorPanel").Find("PropertiesTMP").GetComponent<TextMeshProUGUI>();
            _operatorGunImg = transform.Find("InfoPanel").Find("OperatorPanel").Find("GunRawImage").GetComponent<RawImage>();
            _operatorSkillImg = transform.Find("InfoPanel").Find("OperatorPanel").Find("SkillRawImage").GetComponent<RawImage>();
            _mechaBuffs = transform.Find("InfoPanel").Find("MechaPanel").Find("BuffsTMP").GetComponent<TextMeshProUGUI>();

            _mechaPanels = new List<MechaPanel>();
            _mechaPanels.Add(FindMechaPart(transform.Find("InfoPanel").Find("MechaPanel").Find("MechaHeadPartPanel")));
            _mechaPanels.Add(FindMechaPart(transform.Find("InfoPanel").Find("MechaPanel").Find("MechaBodyPartPanel")));
            _mechaPanels.Add(FindMechaPart(transform.Find("InfoPanel").Find("MechaPanel").Find("MechaLegPartPanel")));
            _mechaButton = transform.Find("InfoPanel").Find("MechaPanel").GetComponent<Button>();

            _mechaSelectPanel = new MechaSelectPanel(transform.Find("MechaSelectPanel"));
            

            _mechaButton.onClick.AddListener(() =>
            {
                _mechaSelectPanel.SelectPanelEnter(m_SelectableMechaPrefab, _model);
            });
        }




        public void ChooseCharacter(PrepareOperator model)
        {
            _model = model;
            EditRoomManager.Instance.SetCharacterModel(model.OpInfo.ModelResourceUrl);
            GenerateUIText();
        }

        public void Enter()
        {
            gameObject.SetActive(true);
            m_InfoPanelTrans.DOSizeDelta(new Vector2(530, 780), 0.5f);
            m_EditCanvasRawImage.color = new Color(1, 1, 1, 0);
            m_EditCanvasRawImage.DOFade(1, 0.5f);
        }

        public IEnumerator QuitAsync()
        {
            m_EditCanvasRawImage.DOFade(0, 0.2f);
            m_InfoPanelTrans.DOSizeDelta(new Vector2(530, 0), 0.2f);
            yield return new WaitForSeconds(0.2f);
            gameObject.SetActive(false);

        }


        private void GenerateUIText()
        {
            _operatorName.text = $"OperatorName:\n{_model.OpInfo.Name}";
            _operatorProperties.text = $"Properties:\n" +
                $"Red:\t{_model.OpInfo.PropRed}\n" +
                $"Gre:\t{_model.OpInfo.PropGreen}\n" +
                $"Blu:\t{_model.OpInfo.PropBlue}";
            _mechaBuffs.text = $"Buffs:\nNO BUFFS";

            _operatorGunImg.texture = _context.GetSkillIcon(_model.OpInfo.GunSkillId);
            _operatorSkillImg.texture = _context.GetSkillIcon(_model.OpInfo.MainSkillId);

            // TODO: 是否需要手动GC
            _mechaPanels[0].PartName.text = _model.OpInfo.McHead.Name;
            _mechaPanels[0].PartRawImage.texture = Resources.Load<Texture2D>("Textures/" + _model.OpInfo.McHead.IconUrl);
            _mechaPanels[0].PartProperties.text = $"ACC:\t{_model.OpInfo.McHead.Accurate}%\n" +
                $"Crt:\t{_model.OpInfo.McHead.Critical}%";

            _mechaPanels[1].PartName.text = _model.OpInfo.McBody.Name;
            _mechaPanels[1].PartRawImage.texture = Resources.Load<Texture2D>("Textures/" + _model.OpInfo.McBody.IconUrl);
            _mechaPanels[1].PartProperties.text = $"HP:\t{_model.OpInfo.McBody.HP}\n" +
                $"HPR:\t{_model.OpInfo.McBody.HPRecover}";

            _mechaPanels[2].PartName.text = _model.OpInfo.McLeg.Name;
            _mechaPanels[2].PartRawImage.texture = Resources.Load<Texture2D>("Textures/" + _model.OpInfo.McLeg.IconUrl);
            _mechaPanels[2].PartProperties.text = $"SPD:\t{_model.OpInfo.McLeg.Speed.ToString("0.00")}\n" +
                $"DOG:\t{_model.OpInfo.McLeg.Dodge}%";
        }

        private MechaPanel FindMechaPart(Transform partPanelTrans)
        {
            var res = new MechaPanel();
            res.PartRawImage = partPanelTrans.Find("PartRawImage").GetComponent<RawImage>();
            res.PartName = partPanelTrans.Find("PartNameTMP").GetComponent<TextMeshProUGUI>();
            res.PartProperties = partPanelTrans.Find("PartPropertiesTMP").GetComponent<TextMeshProUGUI>();
            return res;
        }
    }
}
