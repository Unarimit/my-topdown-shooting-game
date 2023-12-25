using Assets.Scripts.Common.Test;
using Cinemachine;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.Environment.OperatorDecoration
{
    internal class DecorationManager : MonoBehaviour
    {
        public static DecorationManager Instance;
        CinemachineVirtualCamera[] cameras;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
            cameras = GetComponentsInChildren<CinemachineVirtualCamera>();
            foreach(var cam in cameras) cam.enabled = false;
        }
        public void PlaceOperatorDecoration()
        {
            var typeControllers = GetComponentsInChildren<DecorationTypeController>();
            var ops = HomeContextManager.Instance.GetDecorationOperator();
            foreach(var typec in typeControllers)
            {
                for(int i = 0; i < typec.transform.childCount; i++)
                {
                    var trans = HomeContextManager.Instance.GenerateDisplay(ops[Random.Range(0, ops.Count)], typec.transform.GetChild(i), typec.m_WithGun);
                    trans.GetComponent<Animator>().runtimeAnimatorController = typec.m_AnimeController;
                }
            }
        }

        public void EnableRandomViewCamera()
        {
            var aimCam = cameras[Random.Range(0, cameras.Length)];
            aimCam.enabled = true;

            var ops = HomeContextManager.Instance.GetDecorationOperator();
            var typec = aimCam.GetComponentInParent<DecorationTypeController>();
            var trans = HomeContextManager.Instance.GenerateDisplay(ops[Random.Range(0, ops.Count)], aimCam.transform.parent, typec.m_WithGun);
            trans.GetComponent<Animator>().runtimeAnimatorController = typec.m_AnimeController;

            foreach(var x in cameras)
            {
                if (x != aimCam) x.enabled = false;
            }
        }

    }
}
