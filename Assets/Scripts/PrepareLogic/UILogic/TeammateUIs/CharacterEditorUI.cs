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

namespace Assets.Scripts.PrepareLogic.UILogic
{
    public class CharacterEditorUI : MonoBehaviour
    {
        public RawImage m_RawImage;
        public RectTransform m_InfoPanelTrans;

        private TextMeshProUGUI _operatorName;
        private TextMeshProUGUI _operatorProperties;

        private TextMeshProUGUI _mechaBuffs;
        private PrepareOperator _model;
        private List<MechaPanel> _mechaPanels;
        class MechaPanel
        {
            public RawImage PartRawImage;
            public TextMeshProUGUI PartName;
            public TextMeshProUGUI PartProperties;
        }

        private void Awake()
        {
            _operatorName = transform.Find("InfoPanel").Find("OperatorPanel").Find("OperatorNameTMP").GetComponent<TextMeshProUGUI>();
            _operatorProperties = transform.Find("InfoPanel").Find("OperatorPanel").Find("PropertiesTMP").GetComponent<TextMeshProUGUI>();
            _mechaBuffs = transform.Find("InfoPanel").Find("MechaPanel").Find("BuffsTMP").GetComponent<TextMeshProUGUI>();

            _mechaPanels = new List<MechaPanel>();
            _mechaPanels.Add(FindMechaPart(transform.Find("InfoPanel").Find("MechaPanel").Find("MechaHeadPartPanel")));
            _mechaPanels.Add(FindMechaPart(transform.Find("InfoPanel").Find("MechaPanel").Find("MechaBodyPartPanel")));
            _mechaPanels.Add(FindMechaPart(transform.Find("InfoPanel").Find("MechaPanel").Find("MechaLegPartPanel")));
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
            m_RawImage.color = new Color(1, 1, 1, 0);
            m_RawImage.DOFade(1, 0.5f);
        }

        public IEnumerator QuitAsync()
        {
            m_RawImage.DOFade(0, 0.2f);
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
