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
        private Dictionary<HomePage, CinemachineVirtualCamera[]> camerasDic = new Dictionary<HomePage, CinemachineVirtualCamera[]>();
        // 为了让相机能够按我规划的路径过渡，拙劣的模仿了关键帧方式


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
            camerasDic.Add(HomePage.MainView, new [] { transform.Find("MainViewVC").GetComponent<CinemachineVirtualCamera>() });
            camerasDic.Add(HomePage.TopView, new [] { transform.Find("TopViewVC").GetComponent<CinemachineVirtualCamera>() });
            camerasDic.Add(HomePage.CoreView, new [] { transform.Find("CoreViewVC").GetComponent<CinemachineVirtualCamera>() });
            camerasDic.Add(HomePage.BattleView, new [] { transform.Find("BattleViewVC").GetComponent<CinemachineVirtualCamera>() });
            camerasDic.Add(HomePage.FileRoomView, new[] { transform.Find("FileRoomViewVC").GetComponent<CinemachineVirtualCamera>() });
            camerasDic.Add(HomePage.ActionView, new[] { transform.Find("ActionViewVC").GetComponent<CinemachineVirtualCamera>() });
            camerasDic.Add(HomePage.CoreCharacterView, new [] { transform.Find("CoreCharacterViewVC").GetComponent<CinemachineVirtualCamera>() });
            camerasDic.Add(HomePage.CoreMechaView, new [] { 
                transform.Find("CoreMechaPhase1ViewVC").GetComponent<CinemachineVirtualCamera>(),
                transform.Find("CoreMechaPhase2ViewVC").GetComponent<CinemachineVirtualCamera>(),
            });
            camerasDic.Add(HomePage.GachaCharacterView, new [] {
                transform.Find("GachaingView").Find("GachaingViewPhaseVC").GetComponent<CinemachineVirtualCamera>(),
            });

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
            var last = camerasDic[CurCameraPos][^1];

            // 过渡退出
            if(CurCameraPos != HomePage.GachaCharacterView)
            {
                for (int i = camerasDic[CurCameraPos].Length - 2; i >= 0; i--)
                {
                    last.gameObject.SetActive(false);
                    camerasDic[CurCameraPos][i].gameObject.SetActive(true);
                    last = camerasDic[CurCameraPos][i];

                    yield return null;
                    while (brain.IsBlending is true)
                    {
                        yield return null;
                    }
                }
            }

            CurCameraPos = pos;

            // 过渡进入
            for (int i = 0; i < camerasDic[pos].Length; i++)
            {
                last.gameObject.SetActive(false);
                camerasDic[pos][i].gameObject.SetActive(true);
                last = camerasDic[pos][i];

                yield return null;
                while (brain.IsBlending is true)
                {
                    yield return null;
                }
            }
            IsFinishTween = true;
        }

    }
}
