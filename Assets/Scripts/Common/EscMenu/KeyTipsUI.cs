using Assets.Scripts.CombatLogic.UILogic;
using System.Text;
using TMPro;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Common.EscMenu
{
    public class KeyTipsUI : SubUIBase
    {

        public TextMeshProUGUI keyPromptText;
        public InputActionAsset inputActionAsset;

        private void Start()
        {
            GenerateKeyPrompts();
        }

        private void GenerateKeyPrompts()
        {
            var inputActions = inputActionAsset.actionMaps;
            var sb = new StringBuilder();
            foreach (var actionMap in inputActions)
            {
                foreach (var action in actionMap.actions)
                {
                    // 忽略不是按键类型的操作
                    if (!action.bindings[0].isPartOfComposite)
                    {
                        if (action.bindings[0].effectivePath.Contains("/")) sb.Append($"{action.name}: " +
                            $"{action.bindings[0].effectivePath.Split("/")[1]} \n");
                        else sb.Append($"{action.name}: {action.bindings[0].effectivePath} \n");

                    }
                }
            }
            keyPromptText.text = sb.ToString();
        }
    }
}
