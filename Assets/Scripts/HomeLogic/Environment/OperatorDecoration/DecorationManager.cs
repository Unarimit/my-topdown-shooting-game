using Assets.Scripts.Common.Test;
using Assets.Scripts.Entities;
using Cinemachine;
using System.Collections.Generic;
using Unity.VisualScripting;
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
                    place(typec, typec.transform.GetChild(i), ops[Random.Range(0, ops.Count)]);
                }
            }
        }
        public void EnableRandomViewCamera()
        {
            var aimCam = cameras[Random.Range(0, cameras.Length)];
            aimCam.enabled = true;

            var ops = HomeContextManager.Instance.GetDecorationOperator();
            var typec = aimCam.GetComponentInParent<DecorationTypeController>();
            place(typec, aimCam.transform.parent, ops[Random.Range(0, ops.Count)]);

            foreach(var x in cameras)
            {
                if (x != aimCam) x.enabled = false;
            }
        }

        readonly HashSet<Transform> placed = new();
        private void place(DecorationTypeController typec, Transform child, Operator op)
        {
            if (placed.Contains(child)) return;

            var trans = HomeContextManager.Instance.GenerateDisplay(op, child, typec.m_WithGun);
            trans.GetComponent<Animator>().runtimeAnimatorController = typec.m_AnimeController;
            if (typec.m_DecorationControllerType == DecorationControllerType.Walk)
            {
                var comp = trans.AddComponent<WalkDecorationOperator>();
                comp.Inject(typec.m_Param);
            }else if(typec.m_DecorationControllerType == DecorationControllerType.Talk)
            {
                var comp = trans.AddComponent<TalkDecorationOperator>();
                comp.Inject(typec.m_Param);
            }

            placed.Add(child);

            if (typec.m_IsGroup)
            {
                for (int i = 0; i < typec.transform.childCount; i++)
                {
                    var ops = HomeContextManager.Instance.GetDecorationOperator();
                    place(typec, typec.transform.GetChild(i), ops[Random.Range(0, ops.Count)]);
                }
            }
        }


    }
}
