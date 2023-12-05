using Assets.Scripts.CombatLogic;
using Assets.Scripts.CombatLogic.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GunController : MonoBehaviour
    {

        [Header("Shot")]
        [Tooltip("GunFire GameObject")]
        public GameObject GunFire;
        [Tooltip("Bullet start trans")]
        public Transform BulletStartTrans;

        public GunProperty gunProperty = new GunProperty();
        /// <summary>
        /// 是否是玩家的武器
        /// </summary>
        public bool IsPlayer { get; set; } = false;


        private List<AudioClip> gunshotAudioAudioClips;
        [Range(0, 1)] public float GunshotAudioVolume = 0.5f;
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

        private float LastFireTime = -1; // 最后一次开火时间
        private CombatContextManager _context => CombatContextManager.Instance;
        private void Start()
        {
            gunshotAudioAudioClips = AnimeHelper.Instance.GetGunshot();
        }

        // 判断枪口火焰
        private void Update()
        {
            GunFire.SetActive(Time.time - LastFireTime < 0.1);
        }

        public CombatCombatSkill Skill { get; private set; }
        public void InitGun(CombatCombatSkill cskill)
        {
            if (GetComponent<OperatorController>().Model.IsPlayer)
            {
                IsPlayer = true;
                _context.CombatVM.PlayerGun = this;
            }
            gunProperty.CurrentAmmo = cskill.SkillInfo.Ammo;
            gunProperty.MaxAmmo = cskill.SkillInfo.Ammo;
            Skill = cskill;
        }
        public bool ShootUseSkill(Vector3 aim)
        {
            if (gunProperty.CurrentAmmo == 0) return false; // 有子弹
            if (_context.UseSkill(transform, Skill, aim, BulletStartTrans.position, BulletStartTrans.eulerAngles)) // 满足冷却要求
            {
                gunProperty.CurrentAmmo -= 1;
                LastFireTime = Time.time;
                if (IsPlayer)
                {
                    AudioSource.PlayClipAtPoint(gunshotAudioAudioClips[gunProperty.CurrentAmmo % 15], Camera.main.transform.position + Camera.main.transform.forward * 2, GunshotAudioVolume);
                    CurrentAmmoChangeEvent.Invoke(); // UI更新
                }
                else
                {
                    AudioSource.PlayClipAtPoint(gunshotAudioAudioClips[gunProperty.CurrentAmmo % 15], transform.position, GunshotAudioVolume);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 异步更换子弹
        /// </summary>
        /// <returns></returns>
        public IEnumerator Reloading()
        {
            if (gunProperty.LastReloading == 0) yield break; // 不需重复换弹

            while (gunProperty.LastReloading != 0)
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
            gunProperty.LastReloading = gunProperty.ReloadTime;
            if (IsPlayer) CurrentAmmoChangeEvent.Invoke();
        }
        public bool IsReloading()
        {
            return gunProperty.LastReloading != gunProperty.ReloadTime;
        }

        public delegate void GunCurrentAmmoChange();
        public event GunCurrentAmmoChange CurrentAmmoChangeEvent;

    }
}
