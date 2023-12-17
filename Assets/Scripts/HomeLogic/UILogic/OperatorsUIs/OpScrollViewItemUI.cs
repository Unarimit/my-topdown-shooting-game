using Assets.Scripts.Common;
using Assets.Scripts.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.OperatorsUIs
{
    /// <summary>
    /// 家园中的展示角色信息，主要展示体力状态
    /// </summary>
    internal class OpScrollViewItemUI : MonoBehaviour
    {
        [SerializeField]
        RawImage m_portraitImage;
        [SerializeField]
        TextMeshProUGUI m_opNameTMP;
        [SerializeField]
        TextMeshProUGUI m_opTypeTMP;
        [SerializeField]
        TextMeshProUGUI m_opPowerTMP;
        public void Inject(Operator op)
        {
            m_portraitImage.texture = PhotographyManager.GetOperatorPortrait(op);
            m_opNameTMP.text = op.Name;
            m_opTypeTMP.text = op.Type.ToString();
            m_opPowerTMP.text = generatePowerText(op);
        }

        private string generatePowerText(Operator op)
        {
            return $"体力：{op.Power} / {op.MaxPower}";
        }
    }
}
