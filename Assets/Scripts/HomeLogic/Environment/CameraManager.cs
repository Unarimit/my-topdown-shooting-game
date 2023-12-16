using Assets.Scripts.HomeLogic.UILogic;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.HomeLogic.Environment
{
    internal class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;

        // component
        private CinemachineBrain brain;

        // fields
        private Dictionary<HomePage, CinemachineVirtualCamera> camerasDic = new Dictionary<HomePage, CinemachineVirtualCamera>();

        /// <summary>
        /// 相机是否完成过渡
        /// </summary>
        public bool IsFinishTween { private set; get; } = true;
        public HomePage CurCameraPos { get; private set; }
        private void Awake()
        {
            // singleton
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");

            // find component
            camerasDic.Add(HomePage.MainView, transform.Find("MainViewVC").GetComponent<CinemachineVirtualCamera>());
            camerasDic.Add(HomePage.TopView, transform.Find("TopViewVC").GetComponent<CinemachineVirtualCamera>());
            camerasDic.Add(HomePage.CoreView, transform.Find("CoreViewVC").GetComponent<CinemachineVirtualCamera>());
            camerasDic.Add(HomePage.BattleView, transform.Find("BattleViewVC").GetComponent<CinemachineVirtualCamera>());

            brain = transform.Find("Main Camera").GetComponent<CinemachineBrain>();

        }
        private void Start()
        {
            CurCameraPos = HomePage.MainView;
        }

        public IEnumerator SwitchCamera(HomePage pos)
        {
            if (IsFinishTween is false)
            {
                Debug.LogError("SwitchCamera方法应该先判断IsFinishTween，避免出现不合理的逻辑");
                yield break;
            }
            if(pos == CurCameraPos) yield break;

            // 可能需要处理没有设置机位的HomePage

            IsFinishTween = false;
            camerasDic[CurCameraPos].gameObject.SetActive(false);
            camerasDic[pos].gameObject.SetActive(true);
            CurCameraPos = pos;

            yield return null;

            while (brain.IsBlending is true)
            {
                yield return null;
            }

            IsFinishTween = true;
        }

        
    }
}
