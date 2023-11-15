using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.PrepareLogic.UILogic.TeammateUIs
{
    /// <summary>
    /// 改变页面的UI，占据TeammateUI grid布局的第一个位置
    /// </summary>
    public class EditerSwitcherUI : MonoBehaviour, IPointerClickHandler
    {

        public TextMeshProUGUI m_MainText;
        int statu = 0;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if(statu == 0)
                {
                    UIManager.Instance.SwithPage();
                    m_MainText.text = "GO TO MAIN PAGE";
                    statu = 1;
                }
                else
                {
                    UIManager.Instance.SwithPage();
                    m_MainText.text = "GO TO EDITOR";
                    statu = 0;
                }
            }
        }
    }
}
