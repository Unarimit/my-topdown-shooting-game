using Assets.Scripts.Common.Test;
using Assets.Scripts.Entities;
using Assets.Scripts.HomeLogic.ContextExtend;
using System;
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

        public float m_ForwordSpeed = 10f;
        public Vector3 m_AngleSpeed = Vector3.zero;
        /// <summary>
        /// 基本由timeline控制
        /// </summary>
        public GachaEffectStatu Statu = GachaEffectStatu.Finish;

        public GameObject m_BlueTail;
        public GameObject m_PurpleTail;
        public GameObject m_OrangeTail;

        private GameObject currentTail;

        Vector3 initPos;
        Vector3 initAngle;
        private void Awake()
        {
            initPos = transform.position;
            initAngle = transform.eulerAngles;
            m_BlueTail.SetActive(false);
            m_PurpleTail.SetActive(false);
            m_OrangeTail.SetActive(false);
        }
        public void MyReset()
        {
            transform.position = initPos;
            transform.eulerAngles = initAngle;
        }

        public void Play(GachaRarity gachaRarity)
        {
            if(gachaRarity == GachaRarity.Low) currentTail = m_BlueTail;
            else if (gachaRarity == GachaRarity.Middle) currentTail = m_PurpleTail;
            else if (gachaRarity == GachaRarity.High) currentTail = m_OrangeTail;

            transform.position = initPos;
            transform.eulerAngles = initAngle;
            GetComponent<PlayableDirector>().Play();
            currentTail.SetActive(true);
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
                currentTail.SetActive(false);
                m_Boom.GetComponent<ParticleSystem>().Play();
                this.enabled = false;
                return;
            }
            transform.position += transform.forward * m_ForwordSpeed * Time.deltaTime;
            transform.eulerAngles += m_AngleSpeed * Time.deltaTime;
        }

    }
}
