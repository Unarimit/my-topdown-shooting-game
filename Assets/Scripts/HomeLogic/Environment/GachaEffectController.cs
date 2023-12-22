using Assets.Scripts.Common.Test;
using Assets.Scripts.Entities;
using UnityEngine;
using UnityEngine.Playables;

namespace Assets.Scripts.HomeLogic.Environment
{
    internal enum GachaEffectStatu
    {
        Playing,
        Boom,
        Finish
    }
    internal class GachaEffectController : MonoBehaviour
    {
        public GameObject m_Boom;
        public GameObject m_Tail;

        public float m_ForwordSpeed = 10f;
        public Vector3 m_AngleSpeed = Vector3.zero;
        /// <summary>
        /// 基本由timeline控制
        /// </summary>
        public GachaEffectStatu Statu = GachaEffectStatu.Finish;

        Vector3 initPos;
        Vector3 initAngle;
        private void Awake()
        {
            initPos = transform.position;
            initAngle = transform.eulerAngles;
        }
        public void MyReset()
        {
            transform.position = initPos;
            transform.eulerAngles = initAngle;
        }

        public void Play()
        {
            transform.position = initPos;
            transform.eulerAngles = initAngle;
            GetComponent<PlayableDirector>().Play();
            m_Tail.SetActive(true);
            this.enabled = true;
        }
        private void OnEnable()
        {
            Statu = GachaEffectStatu.Playing;
        }

        private void Update()
        {
            if(Statu == GachaEffectStatu.Finish)
            {
                this.enabled = false;
                return;
            }
            if (Statu == GachaEffectStatu.Boom)
            {
                transform.eulerAngles = Vector3.zero;
                m_Tail.SetActive(false);
                m_Boom.GetComponent<ParticleSystem>().Play();
                this.enabled = false;
                return;
            }
            transform.position += transform.forward * m_ForwordSpeed * Time.deltaTime;
            transform.eulerAngles += m_AngleSpeed * Time.deltaTime;
        }

    }
}
