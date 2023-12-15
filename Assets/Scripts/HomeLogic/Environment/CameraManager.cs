using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.Environment
{
    enum CameraPos
    {
        TopView,
        CoreView,
        BattleView,
        MainView
    }
    internal class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;

        // component
        private CinemachineBrain brain;

        // fields
        private Dictionary<CameraPos, CinemachineVirtualCamera> camerasDic = new Dictionary<CameraPos, CinemachineVirtualCamera>();

        /// <summary>
        /// 相机是否完成过渡
        /// </summary>
        public bool IsFinishTween { private set; get; } = true;
        public CameraPos CurCameraPos { get; private set; }
        private void Awake()
        {
            // singleton
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");

            // find component
            camerasDic.Add(CameraPos.MainView, transform.Find("MainViewVC").GetComponent<CinemachineVirtualCamera>());
            camerasDic.Add(CameraPos.TopView, transform.Find("TopViewVC").GetComponent<CinemachineVirtualCamera>());
            camerasDic.Add(CameraPos.CoreView, transform.Find("CoreViewVC").GetComponent<CinemachineVirtualCamera>());
            camerasDic.Add(CameraPos.BattleView, transform.Find("BattleViewVC").GetComponent<CinemachineVirtualCamera>());

            brain = transform.Find("Main Camera").GetComponent<CinemachineBrain>();

        }
        private void Start()
        {
            CurCameraPos = CameraPos.MainView;
        }

        public IEnumerator SwitchCamera(CameraPos pos)
        {
            if (IsFinishTween is false)
            {
                Debug.LogError("SwitchCamera方法应该先判断IsFinishTween，避免出现不合理的逻辑");
                yield break;
            }
            if(pos == CurCameraPos) yield break;

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
