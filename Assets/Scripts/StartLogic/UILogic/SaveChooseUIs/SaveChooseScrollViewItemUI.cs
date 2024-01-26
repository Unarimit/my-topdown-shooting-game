using Assets.Scripts.Entities.Save;
using Assets.Scripts.HomeLogic.UILogic.OverlayBuildingUIs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.StartLogic.UILogic.SaveChooseUIs
{
    internal class SaveChooseScrollViewItemUI : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_saveNameTMP;
        [SerializeField]
        TextMeshProUGUI m_saveTimeTMP;
        [SerializeField]
        TextMeshProUGUI m_saveDescTMP;
        [SerializeField]
        RawImage m_SaveSnapshotImage;
        [SerializeField]
        Button m_controlButton;
        [SerializeField]
        ObProduceScrollViewUI m_obProduceScrollViewUI; // 图省事，复用一个子UI

        internal void Inject(SaveAbstract save, SaveChoosePanelUI saveChoosePanelUI)
        {
            m_saveNameTMP.text = save.SaveName;
            m_saveTimeTMP.text = save.SaveTime.ToString();
            m_saveDescTMP.text = save.SaveDesc;

            // TODO: 改成保存时截图存储
            if (save.SaveDay % 2 == 0) m_SaveSnapshotImage.texture = ResourceManager.Load<Texture>("Textures/save-night");
            else m_SaveSnapshotImage.texture = ResourceManager.Load<Texture>("Textures/save-day");

            m_obProduceScrollViewUI.Inject(save.Resource);
            m_controlButton.onClick.AddListener(() => saveChoosePanelUI.OnSaveSelect(save.SaveId));
        }
        private void OnDestroy()
        {
            m_controlButton.onClick.RemoveAllListeners();
        }
    }
}
