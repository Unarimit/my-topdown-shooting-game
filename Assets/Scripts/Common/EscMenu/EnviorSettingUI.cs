
using Michsky.UI.Shift;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common.EscMenu
{
    internal class EnviorSettingUI : MonoBehaviour
    {
        Button quitBtn;
        MainPanelManager mpm;
        public void Open()
        {
            if(mpm == null) mpm = GetComponent<MainPanelManager>();
            mpm.OpenPanel("GamePlay");
            gameObject.SetActive(true);
            GetComponent<Animator>().Play("Panel In");
        }

        private void quit()
        {
            gameObject.SetActive(false);
        }
    }
}
