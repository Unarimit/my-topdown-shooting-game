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
        public int HP = 10;
        public ParticleSystem Shield;

        private int _fullHp;
        /// <summary>
        /// 用于检测敌方来源
        /// </summary>
        public delegate void HittedEventHandler(object sender, Vector3 hitSourcePos);
        public event HittedEventHandler HittedEvent;
        private void Start()
        {
            Shield.Stop();
            _fullHp = HP;
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag == "Bullet")
            {
                HP -= 1;
                HittedEvent.Invoke(this, collision.transform.GetComponent<BulletController>().InitiatePos);
                Shield.Simulate(1.0f);
                Shield.Play();
                Shield.startColor = new Color(1, 1, 1) * (float)HP / _fullHp;
            }
            if (HP == 2) Shield.Stop();
            if (HP <= 0) Destroy(gameObject);
        }

    }
}
