using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Assets.Scripts.PrepareLogic.PrepareEntities;
using Assets.Scripts.PrepareLogic.EffectLogic;
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

        public class MechaSelectPanel
        {
            private Transform Transform;
            private CanvasGroup CanvasGroup;
            private Transform ContentTrans;
            private Button OKButton;
            private List<SelectableMechaItemUI> SelectableMechas = new List<SelectableMechaItemUI>();
            private List<int> ActiveMechas = new List<int> { 0, 1, 2 }; // TODO：返回时保存该设置
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

            public void ItemOnClick(int index)
            {
                int aimPos = 0;
                if (SelectableMechas[index].Mecha.GetMechaType() == MechaType.Head) aimPos = 0;
                else if(SelectableMechas[index].Mecha.GetMechaType() == MechaType.Body) aimPos = 1;
                else aimPos = 2;

                // change ui and statu
                SelectableMechas[ActiveMechas[aimPos]].IsSelect = false;
                ActiveMechas[aimPos] = index;
                SelectableMechas[ActiveMechas[aimPos]].IsSelect = true;
            }

            private void InitContent(GameObject selectableMechaPrefab, PrepareOperator model)
            {
                SelectableMechas.Clear();
                int len = ContentTrans.childCount;
                for (int i = 0; i < len; i++)
                {
                    Destroy(ContentTrans.GetChild(i).gameObject);  
                }

                SelectableMechas.Add(CreateItem(selectableMechaPrefab, model.OpInfo.McHead, SelectableMechas.Count));
                SelectableMechas.Add(CreateItem(selectableMechaPrefab, model.OpInfo.McBody, SelectableMechas.Count));
                SelectableMechas.Add(CreateItem(selectableMechaPrefab, model.OpInfo.McLeg, SelectableMechas.Count));

                for (int i = 0; i < 3; i++) SelectableMechas[i].IsSelect = true;

                foreach (var x in TestDB.GetMechas())
                {
                    SelectableMechas.Add(CreateItem(selectableMechaPrefab, x, SelectableMechas.Count));
                }
            }
            private SelectableMechaItemUI CreateItem(GameObject selectableMechaPrefab, MechaBase mecha, int i)
            {
                var go = Instantiate(selectableMechaPrefab, ContentTrans);
                var res = go.AddComponent<SelectableMechaItemUI>();
                res.Inject(mecha, i, this);
                return res;
            }

            private void SelectPanelQuit()
            {
                Transform.gameObject.SetActive(false);
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
            _mechaPanels[0].PartProperties.text = _model.OpInfo.McHead.ToString();

            _mechaPanels[1].PartName.text = _model.OpInfo.McBody.Name;
            _mechaPanels[1].PartRawImage.texture = Resources.Load<Texture2D>("Textures/" + _model.OpInfo.McBody.IconUrl);
            _mechaPanels[1].PartProperties.text = _model.OpInfo.McBody.ToString();

            _mechaPanels[2].PartName.text = _model.OpInfo.McLeg.Name;
            _mechaPanels[2].PartRawImage.texture = Resources.Load<Texture2D>("Textures/" + _model.OpInfo.McLeg.IconUrl);
            _mechaPanels[2].PartProperties.text = _model.OpInfo.McLeg.ToString();
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
