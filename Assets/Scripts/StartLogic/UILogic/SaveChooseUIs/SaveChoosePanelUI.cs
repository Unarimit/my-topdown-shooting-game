using Assets.Scripts.Entities.Save;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.StartLogic.UILogic.SaveChooseUIs
{
    internal class SaveChoosePanelUI : MonoBehaviour
    {
        private void OnEnable()
        {
            transform.Find("ScrollView").GetComponent<SaveChooseScrollViewUI>().Inject(MyServices.Database.SaveAbstracts, this);
        }

        public void OnSaveSelect(string saveId)
        {
            StartContextManager.Instance.LoadGame(saveId);
        }
    }
}
