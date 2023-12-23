using Assets.Scripts.Common;
using Assets.Scripts.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.ActionUIs
{
    internal class ActionScrollViewItemUI : MonoBehaviour
    {
        public void Inject(LevelRule levelRule, ActionUI actionUI)
        {
            transform.Find("LevelNameTMP").GetComponent<TextMeshProUGUI>().text = levelRule.LevelName;
            GetComponent<Button>().onClick.AddListener(() =>
            {
                actionUI.OnActionSelect(levelRule);
            });
        }
    }
}
