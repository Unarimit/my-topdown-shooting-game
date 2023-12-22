using Assets.Scripts.Common;
using Assets.Scripts.Entities;
using Assets.Scripts.HomeLogic.Environment;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.GachaUIs
{
    internal class GachaNewOperatorInfoUI : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_operatorName;
        [SerializeField]
        TextMeshProUGUI m_operatorProperties;
        [SerializeField]
        TextMeshProUGUI m_operatorType;

        [SerializeField]
        RawImage m_operatorWeaponImg;
        [SerializeField]
        RawImage m_operatorSkillImg;
        [SerializeField]
        RawImage m_operatorPortraitImg;
        [SerializeField]
        RawImage m_operatorHeadIconImg;

        public Operator Operater { get; set; }

        private void Awake()
        {
            Operater = null;
        }
        private void OnEnable()
        {
            if(Operater == null) gameObject.SetActive(false);
            else Init(Operater);
        }
        public void Init(Operator op)
        {
            m_operatorName.text = $"OperatorName:\n{op.Name}";
            m_operatorProperties.text = $"Properties:\n" +
                $"Red:\t{op.PropRed}\n" +
                $"Gre:\t{op.PropGreen}\n" +
                $"Blu:\t{op.PropBlue}";
            m_operatorType.text = op.Type.ToString();

            m_operatorWeaponImg.texture = ResourceManager.Load<Texture2D>("Skills/" + MyServices.Database.CombatSkills[op.WeaponSkillId].IconUrl);
            m_operatorSkillImg.texture = ResourceManager.Load<Texture2D>("Skills/" + MyServices.Database.CombatSkills[op.MainSkillId].IconUrl);


            m_operatorPortraitImg.texture = PhotographyManager.GetOperatorPortrait(op);
            m_operatorHeadIconImg.texture = PhotographyManager.GetOperatorHeadIcon(op);
        }
        private void OnDisable()
        {
            if(Operater != null)
            {
                Operater = null;
                GachaViewManager.Instance.Quit();
            }
        }
    }
}
