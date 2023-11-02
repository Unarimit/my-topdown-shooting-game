using Assets.Scripts.ComputerControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BulletLogic
{
    public class DestructiblePersonController : MonoBehaviour
    {
        public ParticleSystem Shield;
        public GameObject EorTMark;

        /// <summary>
        /// 用于检测敌方来源
        /// </summary>
        public delegate void HittedEventHandler(object sender, Vector3 hitSourcePos);
        public event HittedEventHandler HittedEvent;


        /// <summary>
        /// 用于检测敌方来源
        /// </summary>
        public delegate void HP0EventHandler(object sender);
        public event HP0EventHandler HP0Event;
        private CombatContextManager _contxt;
        private void Start()
        {
            Shield.Stop();
            _contxt = CombatContextManager.Instance;
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag == "Bullet")
            {
                HittedEvent.Invoke(transform, collision.transform.GetComponent<BulletController>().InitiatePos);
                Shield.Simulate(1.0f);
                Shield.Play();
                Shield.startColor = new Color(1, 1, 1) * 
                    (float)_contxt.GetOperatorCurrentHP(transform) / _contxt.GetOperatorMaxHP(transform);

                CombatContextManager.Instance.DellDamage(null, transform, 1);
            }
            
        }
        public void DoDied()
        {
            EorTMark.SetActive(false);
            GetComponent<Collider>().enabled = false;
            HP0Event.Invoke(transform);
        }
        public void GotDMG()
        {
            if (_contxt.GetOperatorCurrentHP(transform) == 2) Shield.Stop();
        }

    }
}
