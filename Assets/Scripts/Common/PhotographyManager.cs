using Assets.Scripts.CombatLogic;
using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    /// <summary>
    /// 角色Portrait、HeadIcon等帮助类
    /// </summary>
    public class PhotographyManager : MonoBehaviour
    {

        public static PhotographyManager Instance;
        public static Texture GetOperatorPortrait(Operator op)
        {
            certainPhotographyManager();
            if (Instance.PortraitDic.ContainsKey(op.ModelResourceUrl) is not true)
            {
                Instance.PortraitDic.Add(op.ModelResourceUrl, Instance.GetCharacterPortrait(op.ModelResourceUrl));
            }
            return Instance.PortraitDic[op.ModelResourceUrl];
        }

        public static Texture GetOperatorHeadIcon(Operator op)
        {
            certainPhotographyManager();
            if (Instance.HeadIconDic.ContainsKey(op.ModelResourceUrl) is not true)
            {
                Instance.HeadIconDic.Add(op.ModelResourceUrl, Instance.GetCharacterHeadIcon(op.ModelResourceUrl));
            }
            return Instance.HeadIconDic[op.ModelResourceUrl];
        }

        private static void certainPhotographyManager()
        {
            if (Instance == null)
            {
                var go = Instantiate(ResourceManager.Load<GameObject>("UIs/PhotographyRoom"));
                go.transform.position = new Vector3(500, 500, 500);
                DontDestroyOnLoad(go);
            }
        }

        public Camera m_FullBodyCamera;
        public Camera m_HeadOnlyCamera;
        private Transform _fullBodyTrans;
        private Transform _headOnlyTrans;
        private Dictionary<string, Texture> PortraitDic = new Dictionary<string, Texture>();
        private Dictionary<string, Texture> HeadIconDic = new Dictionary<string, Texture>();
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
            GetComponent<FbxLoadManager>().LoadModel(modelUrl, go.transform, false);

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
            GetComponent<FbxLoadManager>().LoadModel(modelUrl, go.transform, false);

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
