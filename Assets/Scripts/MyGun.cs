using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class MyGun
    {
        public string Name { get; set; } = "AK47";

        /// <summary>
        /// 射速 rounds/min
        /// </summary>
        public int RateOfFile { get; set; } = 600;

        /// <summary>
        /// 初速度 m/s
        /// </summary>
        public int MuzzleVelocity { get; set; } = 715;


        private DateTime lastShootTime = DateTime.Now;

        public bool ShootOrWait(GameObject bullet, Vector3 push)
        {
            if((DateTime.Now - lastShootTime).TotalMilliseconds < 1.0 / RateOfFile * 60 * 1000)
            {
                return false;
            }

            bullet.SetActive(true);
            
            lastShootTime = DateTime.Now;
            return true;
        }

        public IEnumerator DelayForce(GameObject bullet, Vector3 push)
        {
            for(int i = 0; i < 2; i++)
            {
                if(i == 1) yield return new WaitForSeconds(0.05f);
                if (i == 1)
                {
                    bullet.GetComponent<Rigidbody>().AddForce(MuzzleVelocity * push / 10);
                    yield return null;
                }
            }
        }
    }
}
