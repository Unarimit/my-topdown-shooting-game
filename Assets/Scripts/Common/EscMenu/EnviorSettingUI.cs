
using Michsky.UI.Shift;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common.EscMenu
{
    internal class EnviorSettingUI : MonoBehaviour
    {

        public static EnviorSettingUI OpenEnviorSettingUI()
        {
            var prefab = ResourceManager.Load<GameObject>("UIs/EnviorSettingCanvas");
            var go = Instantiate(prefab);
            go.GetComponent<EnviorSettingUI>().Display();
            return go.GetComponent<EnviorSettingUI>();
        }

        Button quitBtn;
        MainPanelManager mpm;

        public void Display()
        {
            if(mpm == null) mpm = transform.Find("EnviorSettingPanel").GetComponent<MainPanelManager>();
            mpm.OpenPanel("GamePlay");
            gameObject.SetActive(true);
            transform.Find("EnviorSettingPanel").GetComponent<Animator>().Play("Panel In");
        }

        public void Quit()
        {
            Destroy(gameObject);
        }
    }
}
