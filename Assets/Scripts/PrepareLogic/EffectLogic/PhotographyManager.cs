using Assets.Scripts.CombatLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PrepareLogic.EffectLogic
{
    public class PhotographyManager : MonoBehaviour
    {
        public Camera m_camera;

        public static PhotographyManager Instance;
        private void Awake()
        {

            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");

        }

        public Texture GetCharacterPortrait(string modelUrl)
        {
            m_camera.gameObject.SetActive(true);
            var prefab = ResourceManager.Load<GameObject>("Characters/Displayer");
            var go = Instantiate(prefab, transform);
            PrepareContextManager.Instance.GetComponent<FbxLoadManager>().LoadModel(modelUrl, go.transform, false);
            go.transform.position = new Vector3(0, 0, 1.2f);
            go.transform.eulerAngles = new Vector3(0, 200, 0);

            // render
            m_camera.Render();
            RenderTexture.active = m_camera.targetTexture;
            var res = new Texture2D(m_camera.targetTexture.width, m_camera.targetTexture.height, TextureFormat.ARGB32, false);
            res.ReadPixels(new Rect(0, 0, m_camera.targetTexture.width, m_camera.targetTexture.height), 0, 0);
            res.Apply();

            // destroy model
            go.SetActive(false);
            Destroy(go);
            m_camera.gameObject.SetActive(false);

            return res;
        }
    }

}
