using Assets.Scripts.Common;
using Assets.Scripts.Entities;
using Assets.Scripts.HomeLogic.Environment;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.GachaUIs.GachaOp
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

        public Queue<Operator> Operaters { get; set; }

        private void Awake()
        {
            Operaters = new Queue<Operator>();
        }
        private void OnEnable()
        {
            if (Operaters.Count == 0) gameObject.SetActive(false);
            else Init(Operaters.Peek());
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
            if (Operaters.Count != 0)
            {
                Operaters.Dequeue();
                GachaViewManager.Instance.Quit();
            }
        }
    }
}
