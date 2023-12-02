
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common.EscMenu
{
    internal class EnviorSettingUI : MonoBehaviour
    {
        Button quitBtn;

        private void Awake()
        {
            quitBtn = transform.Find("QuitBtn").GetComponent<Button>();
            quitBtn.onClick.AddListener(quit);
        }

        private void quit()
        {
            gameObject.SetActive(false);
        }
    }
}
