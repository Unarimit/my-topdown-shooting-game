﻿using System;
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

        /// <summary>
        /// 用于检测敌方来源
        /// </summary>
        public delegate void HittedEventHandler(object sender, Vector3 hitSourcePos);
        public event HittedEventHandler HittedEvent;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag == "Bullet")
            {
                HP -= 1;
                HittedEvent.Invoke(this, collision.transform.GetComponent<BulletController>().InitiatePos);
            }
            if (HP <= 0) Destroy(gameObject);
        }

    }
}
