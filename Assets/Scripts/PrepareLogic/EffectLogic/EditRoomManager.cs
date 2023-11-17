using Assets.Scripts.CombatLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic.EffectLogic
{
    public class EditRoomManager : MonoBehaviour
    {
        public Camera m_camera;

        public static EditRoomManager Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }
        private GameObject _lastGO;
        public void SetCharacterModel(string modelUrl)
        {
            if(_lastGO != null) Destroy(_lastGO);

            m_camera.gameObject.SetActive(true);
            var prefab = ResourceManager.Load<GameObject>("Characters/Displayer");
            var go = Instantiate(prefab, transform);
            PrepareContextManager.Instance.GetComponent<FbxLoadManager>().LoadModel(modelUrl, go.transform, false);

            go.transform.position = new Vector3(-1.4f, 0, 3.2f) + transform.position;
            go.transform.eulerAngles = new Vector3(0, 130, 0);
            _lastGO = go;
        }
    }
}
