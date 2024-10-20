using UnityEngine;

namespace Assets.Scripts.HomeLogic.Environment
{
    internal class LightManager : MonoBehaviour
    {
        public void Day()
        {
            RenderSettings.ambientLight = new Color32(111, 111, 111, 0);
            transform.Find("Day").gameObject.SetActive(true);
            transform.Find("Night").gameObject.SetActive(false);
        }

        public void Night()
        {
            RenderSettings.ambientLight = new Color32(15, 26, 72, 0);
            transform.Find("Night").gameObject.SetActive(true);
            transform.Find("Day").gameObject.SetActive(false);
        }

        public void InSpace()
        {
            RenderSettings.ambientLight = new Color32(111, 111, 111, 0);
            transform.Find("Night").gameObject.SetActive(true);
            transform.Find("Day").gameObject.SetActive(false);
        }
    }
}
