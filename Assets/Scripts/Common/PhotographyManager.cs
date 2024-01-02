using Assets.Scripts.CombatLogic;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Buildings;
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

        internal static Texture GetBuildingIcon(Building building)
        {
            certainPhotographyManager();
            if (Instance.BuildingDic.ContainsKey(building.ModelUrl) is not true)
            {
                Instance.BuildingDic.Add(building.ModelUrl, Instance.GetBuildingIcon(building.ModelUrl));
            }
            return Instance.BuildingDic[building.ModelUrl];
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
        public Camera m_BuildingCamera;
        private Transform _fullBodyTrans;
        private Transform _headOnlyTrans;
        private Transform _buildingTrans;
        private Dictionary<string, Texture> PortraitDic = new Dictionary<string, Texture>();
        private Dictionary<string, Texture> HeadIconDic = new Dictionary<string, Texture>();
        private Dictionary<string, Texture> BuildingDic = new Dictionary<string, Texture>();

        private void Awake()
        {

            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");

            m_FullBodyCamera.gameObject.SetActive(false);
            m_HeadOnlyCamera.gameObject.SetActive(false);
            _fullBodyTrans = transform.Find("FullBodyTrans");
            _headOnlyTrans = transform.Find("HeadOnlyTrans");
            _buildingTrans = transform.Find("BuildingTrans");
        }

        public Texture GetCharacterPortrait(string modelUrl)
        {
            var prefab = ResourceManager.Load<GameObject>("Characters/Displayer");
            var go = Instantiate(prefab, _fullBodyTrans);
            GetComponent<FbxLoadManager>().LoadModel(modelUrl, go.transform, false);
            changeLayer(go);

            // render
            var res = render(m_FullBodyCamera);

            // destroy model
            go.SetActive(false);
            Destroy(go);

            return res;
        }

        public Texture GetCharacterHeadIcon(string modelUrl)
        {
            // 自定义头像
            // TODO:应该添加flag判断，而不是判空
            Texture res = ResourceManager.TryLoad<Texture2D>("Fbx/HeadIcon/" + modelUrl);
            if (res != null) return res;

            var prefab = ResourceManager.Load<GameObject>("Characters/Displayer");
            var go = Instantiate(prefab, _headOnlyTrans);
            GetComponent<FbxLoadManager>().LoadModel(modelUrl, go.transform, false);
            changeLayer(go);

            // render
            res = render(m_HeadOnlyCamera);

            // destroy model
            go.SetActive(false);
            Destroy(go);

            return res;
        }

        public Texture GetBuildingIcon(string modelUrl)
        {
            var prefab = ResourceManager.Load<GameObject>("Buildings/" + modelUrl);
            var go = Instantiate(prefab, _buildingTrans);
            changeLayer(go);
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            // render
            var res = render(m_BuildingCamera);

            // destroy model
            go.SetActive(false);
            //Destroy(go);

            return res;
        }

        private Texture render(Camera camera)
        {
            // open camera
            camera.gameObject.SetActive(true);

            // render
            camera.Render();
            RenderTexture.active = camera.targetTexture;
            var res = new Texture2D(camera.targetTexture.width, camera.targetTexture.height, TextureFormat.ARGB32, false);
            res.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
            res.Apply();

            // close camera
            camera.gameObject.SetActive(false);
            return res;
        }

        private void changeLayer(GameObject go)
        {
            var queue = new Queue<GameObject>();
            var layer = LayerMask.NameToLayer("OperatorIconShading");
            queue.Enqueue(go);
            int deep = 0;
            while (queue.Count != 0)
            {
                int len = queue.Count;
                for(int _ = 0 ; _ < len; _++)
                {
                    var t = queue.Dequeue();
                    t.layer = layer;
                    for (int i = 0; i < t.transform.childCount; i++)
                    {
                        queue.Enqueue(t.transform.GetChild(i).gameObject);
                    }
                }
                
                deep += 1;
                if (deep == 4) break;
            }
        }
    }

}
