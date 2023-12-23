using Assets.Scripts.Common.Test;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.Environment
{
    internal class MidLightsManager : MonoBehaviour
    {
        public static MidLightsManager Instance { get; private set; }

        [SerializeField]
        Material m_redMaterial;
        [SerializeField]
        Material m_blueMaterial;


        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }

        private void Start()
        {
            if(HomeContextManager.Instance.HomeVM.IsInInvade is true)
            {
                inInvade();
            }
        }

        private void inInvade()
        {
            var m_MeshRenderers = GetComponentsInChildren<MeshRenderer>();
            for(int i = 0; i < m_MeshRenderers.Length; i++)
            {
                if (i % 2 == 1) continue;
                if (i % 4 == 0)
                {
                    m_MeshRenderers[i].sharedMaterial = m_redMaterial;
                }
                else
                {
                    m_MeshRenderers[i].sharedMaterial = m_blueMaterial;
                }
                var c = m_MeshRenderers[i].transform.AddComponent<LightFleshContoller>();
                c.Interval = 3000;
            }
        }
    }
}
