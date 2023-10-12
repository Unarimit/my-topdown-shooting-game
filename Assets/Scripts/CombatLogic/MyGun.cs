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

        /// <summary>
        /// 是否是玩家的武器
        /// </summary>
        public bool IsPlayer { get; set; } = false;

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



        private DateTime lastShootTime = DateTime.Now;

        /// <summary>
        /// 判断是否可以射击
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="push"></param>
        /// <returns></returns>
        public bool ShootOrWait(GameObject bullet, Vector3 push)
        {
            if((DateTime.Now - lastShootTime).TotalMilliseconds < 1.0 / RateOfFile * 60 * 1000 || CurrentAmmo <= 0)
            {   
                return false;
            }

            bullet.SetActive(true);
            
            lastShootTime = DateTime.Now;
            return true;
        }

        /// <summary>
        /// 异步推出子弹，为了给特效加载时间
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="push"></param>
        /// <returns></returns>
        public IEnumerator DelayForce(GameObject bullet, Vector3 push)
        {
            
            yield return new WaitForSeconds(0.05f);

            // TODO：临时代码
            CurrentAmmo -= 1;
            if (IsPlayer) GunStatuUIManager.Instance.UpdateCurrentAmmo(CurrentAmmo);

            bullet.GetComponent<Rigidbody>().AddForce(MuzzleVelocity * push / 10);
            yield return null;
                
        }

        /// <summary>
        /// 异步更换子弹
        /// </summary>
        /// <returns></returns>
        public IEnumerator Reloading()
        {
            if (LastReloading == 0) yield break; // 不需重复换弹

            while(LastReloading != 0)
            {
                LastReloading -= Time.deltaTime;
                if (LastReloading < 0)
                {
                    LastReloading = 0;
                }
                yield return null;
            }
            // 更新子弹
            CurrentAmmo = MaxAmmo;
        }
    }
}
