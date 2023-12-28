using Assets.Scripts.Entities.Level;
using Michsky.UI.Shift;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.ActionUIs
{
    internal class ActionScrollViewItemUI : MonoBehaviour
    {
        MainPanelButton button;
        public void Inject(LevelRule levelRule, ActionUI actionUI, ActionScrollViewUI container)
        {
            button = transform.GetComponent<MainPanelButton>();
            button.buttonText = levelRule.LevelName;
            gameObject.SetActive(true);
            GetComponent<Button>().onClick.AddListener(() =>
            {
                container.DeSelectAll();
                button.SetKeyPress(true);
                actionUI.OnActionSelect(levelRule);
            });
        }

        public void SetSelect(bool isSelect)
        {
            button.SetKeyPress(isSelect);
        }
    }
}
