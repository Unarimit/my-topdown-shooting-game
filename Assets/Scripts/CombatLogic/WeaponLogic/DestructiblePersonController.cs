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
        private CombatContextManager _context;

        // prefab in character
        private ParticleSystem Shield;
        private GameObject EorTMark;
        private void Start()
        {
            _context = CombatContextManager.Instance;

            // init prefab
            // shield
            var s_prefab = Resources.Load<GameObject>("Effects/Shield");
            Shield = Instantiate(s_prefab, transform).GetComponent<ParticleSystem>();
            Shield.Stop();

            // team mark
            if (_context.Operators[transform].Team == 0 && transform != _context.PlayerTrans)
            {
                var t_prefab = Resources.Load<GameObject>("Effects/TeammateMark");
                EorTMark = Instantiate(s_prefab, transform);
            }
            else if(_context.Operators[transform].Team == 1)
            {
                var t_prefab = Resources.Load<GameObject>("Effects/EnemyMark");
                EorTMark = Instantiate(s_prefab, transform);
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag == "Bullet")
            {
                //HittedEvent.Invoke(transform, collision.transform.GetComponent<BulletController>().InitiatePos);
                Shield.startColor = new Color(1, 1, 1) * 
                    (float)_context.GetOperatorCurrentHP(transform) / _context.GetOperatorMaxHP(transform);
                CombatContextManager.Instance.DellDamage(null, transform, 1);

                Shield.Simulate(1.0f);
                Shield.Play();
            }
            
        }
        public void DoDied()
        {
            GetComponent<Collider>().enabled = false;
            HP0Event.Invoke(transform);
            if(EorTMark != null) EorTMark.SetActive(false);
        }
        public void GotDMG()
        {
            if (_context.GetOperatorCurrentHP(transform) == 2) Shield.Stop();
        }

    }
}
