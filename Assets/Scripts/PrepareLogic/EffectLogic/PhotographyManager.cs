using Assets.Scripts.CombatLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PrepareLogic.EffectLogic
{
    public class PhotographyManager : MonoBehaviour
    {
        public Camera m_FullBodyCamera;
        public Camera m_HeadOnlyCamera;

        public static PhotographyManager Instance;
        private Transform _fullBodyTrans;
        private Transform _headOnlyTrans;
        private void Awake()
        {

            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");

            m_FullBodyCamera.gameObject.SetActive(false);
            m_HeadOnlyCamera.gameObject.SetActive(false);
            _fullBodyTrans = transform.Find("FullBodyTrans");
            _headOnlyTrans = transform.Find("HeadOnlyTrans");
        }

        public Texture GetCharacterPortrait(string modelUrl)
        {
            m_FullBodyCamera.gameObject.SetActive(true);
            var prefab = ResourceManager.Load<GameObject>("Characters/Displayer");
            var go = Instantiate(prefab, _fullBodyTrans);
            PrepareContextManager.Instance.GetComponent<FbxLoadManager>().LoadModel(modelUrl, go.transform, false);

            // render
            m_FullBodyCamera.Render();
            RenderTexture.active = m_FullBodyCamera.targetTexture;
            var res = new Texture2D(m_FullBodyCamera.targetTexture.width, m_FullBodyCamera.targetTexture.height, TextureFormat.ARGB32, false);
            res.ReadPixels(new Rect(0, 0, m_FullBodyCamera.targetTexture.width, m_FullBodyCamera.targetTexture.height), 0, 0);
            res.Apply();

            // destroy model
            go.SetActive(false);
            Destroy(go);
            m_FullBodyCamera.gameObject.SetActive(false);

            return res;
        }

        public Texture GetCharacterHeadIcon(string modelUrl)
        {
            m_HeadOnlyCamera.gameObject.SetActive(true);
            var prefab = ResourceManager.Load<GameObject>("Characters/Displayer");
            var go = Instantiate(prefab, _headOnlyTrans);
            PrepareContextManager.Instance.GetComponent<FbxLoadManager>().LoadModel(modelUrl, go.transform, false);

            // render
            m_HeadOnlyCamera.Render();
            RenderTexture.active = m_HeadOnlyCamera.targetTexture;
            var res = new Texture2D(m_HeadOnlyCamera.targetTexture.width, m_HeadOnlyCamera.targetTexture.height, TextureFormat.ARGB32, false);
            res.ReadPixels(new Rect(0, 0, m_HeadOnlyCamera.targetTexture.width, m_HeadOnlyCamera.targetTexture.height), 0, 0);
            res.Apply();

            // destroy model
            go.SetActive(false);
            Destroy(go);
            m_HeadOnlyCamera.gameObject.SetActive(false);

            return res;
        }
    }

}
