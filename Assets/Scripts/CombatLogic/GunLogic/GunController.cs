using Assets.Scripts.CombatLogic.UILogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class GunController : MonoBehaviour
    {

        [Header("Shot")]
        [Tooltip("Bullet prefab")]
        public GameObject Bullet;
        [Tooltip("GunFire GameObject")]
        public GameObject GunFire;
        [Tooltip("Bullet start trans")]
        public Transform BulletStartTrans;
        public Transform Enviorment;

        public GunProperty gunProperty = new GunProperty();

        /// <summary>
        /// 是否是玩家的武器
        /// </summary>
        public bool IsPlayer { get; set; } = false;

        public class GunProperty
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


            /// <summary>
            /// 子弹上限
            /// </summary>
            public int MaxAmmo { get; set; } = 30;

            /// <summary>
            /// 当前子弹数量
            /// </summary>
            public int CurrentAmmo { get; set; } = 30;

            /// <summary>
            /// 换弹时间
            /// </summary>
            public double ReloadTime { get; set; } = 2;

            /// <summary>
            /// 剩余换弹时间
            /// </summary>
            public double LastReloading { get; set; } = 2;



            public DateTime lastShootTime = DateTime.Now;
        }
        

        public bool Shoot(Vector3 push)
        {
            if (!ShootOrWait()) return false;
            else
            {
                // 发射逻辑
                var bullet = Instantiate(Bullet, Enviorment);
                bullet.transform.position = BulletStartTrans.position;
                bullet.SetActive(true);
                StartCoroutine(DelayForce(bullet, push));
                gunProperty.CurrentAmmo -= 1;

                // UI更新
                if (IsPlayer) GunStatuUIManager.Instance.UpdateCurrentAmmo(gunProperty.CurrentAmmo);

                return true;
            }
        }



        /// <summary>
        /// 判断是否可以射击
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="push"></param>
        /// <returns></returns>
        public bool ShootOrWait()
        {
            if((DateTime.Now - gunProperty.lastShootTime).TotalMilliseconds < 1.0 / gunProperty.RateOfFile * 60 * 1000 || gunProperty.CurrentAmmo <= 0)
            {   
                return false;
            }


            gunProperty.lastShootTime = DateTime.Now;
            return true;
        }

        /// <summary>
        /// 异步推出子弹，枪焰动画，给拖尾特效加载时间
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="push"></param>
        /// <returns></returns>
        public IEnumerator DelayForce(GameObject bullet, Vector3 push)
        {

            GunFire.SetActive(true);
            yield return new WaitForSeconds(0.05f);


            bullet.GetComponent<Rigidbody>().AddForce(gunProperty.MuzzleVelocity * push / 10);
            GunFire.SetActive(false);
            yield return null;
                
        }

        /// <summary>
        /// 异步更换子弹
        /// </summary>
        /// <returns></returns>
        public IEnumerator Reloading()
        {
            if (gunProperty.LastReloading == 0) yield break; // 不需重复换弹

            while(gunProperty.LastReloading != 0)
            {
                gunProperty.LastReloading -= Time.deltaTime;
                if (gunProperty.LastReloading < 0)
                {
                    gunProperty.LastReloading = 0;
                }
                yield return null;
            }
            // 更新子弹
            gunProperty.CurrentAmmo = gunProperty.MaxAmmo;
        }
    }
}
