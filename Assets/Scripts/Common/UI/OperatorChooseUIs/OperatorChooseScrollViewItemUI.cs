using Assets.Scripts.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common.UI.OperatorChooseUIs
{
    internal class OperatorChooseScrollViewItemUI : MonoBehaviour
    {
        [SerializeField]
        RawImage m_headIconImage;
        [SerializeField]
        TextMeshProUGUI m_operatorName;
        [SerializeField]
        TextMeshProUGUI m_operatorType;
        [SerializeField]
        TextMeshProUGUI m_operatorJob;

        [SerializeField]
        Button m_chooseButton;

        public void Inject(Operator op, OperatorChooseUI ui)
        {
            gameObject.SetActive(true);
            m_headIconImage.texture = PhotographyManager.GetOperatorHeadIcon(op);
            m_operatorName.text = op.Name;
            m_operatorType.text = op.Type.ToString();
            if (op.Job.JobStatus == JobStatus.Nothing)
            {
                m_operatorJob.transform.parent.gameObject.SetActive(false);
            }
            else if (op.Job.JobStatus == JobStatus.HomeBuilding) m_operatorJob.text = "家园";
            else if (op.Job.JobStatus == JobStatus.Fighter) m_operatorJob.text = "舰载机";

            m_chooseButton.onClick.AddListener(() => ui.Choose(op));
        }

        private void OnDestroy()
        {
            m_chooseButton.onClick.RemoveAllListeners();
        }
    }
}
