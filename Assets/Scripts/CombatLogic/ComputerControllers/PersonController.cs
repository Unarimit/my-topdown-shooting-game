using Assets.Scripts.BulletLogic;
using Assets.Scripts.CombatLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

namespace Assets.Scripts.ComputerControllers
{
    /// <summary>
    /// 基本的人物控制器，可以开枪、和行走
    /// </summary>
    public class PersonController : MonoBehaviour
    {


        public GunController _gunController;

        private Animator _animator;
        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDAim;
        private int _animIDShoot;
        private int _animIDASpeed;
        private int _animIDWSpeed;
        private int _animIDSlide;
        private int _animIDDied;
        private int _animIDReloading;

        [HideInInspector]
        public CombatContextManager _context;
        private DestructiblePersonController _destructiblePersonController;


        protected bool Moving = false;

        protected void Awake()
        {
            bool temp = TryGetComponent(out _animator);

            // hitted or died
            _destructiblePersonController = GetComponent<DestructiblePersonController>();
            _destructiblePersonController.HP0Event += HP0Event;

            if (!temp) Debug.LogError(transform.ToString() + " have no animator");

            // anime
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDAim = Animator.StringToHash("Aim");
            _animIDShoot = Animator.StringToHash("Shoot");
            _animIDASpeed = Animator.StringToHash("ASpeed");
            _animIDWSpeed = Animator.StringToHash("WSpeed");
            _animIDSlide = Animator.StringToHash("Slide");
            _animIDDied = Animator.StringToHash("Died");
            _animIDReloading = Animator.StringToHash("Reloading");
        }
        private void OnEnable()
        {

            _animator.SetBool(_animIDGrounded, true);
            _animator.SetFloat(_animIDMotionSpeed, 1);
        }

        private void HP0Event(object sender)
        {
            // 清空动画
            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDAim, false);
            _animator.SetBool(_animIDShoot, false);
            _animator.SetBool(_animIDSlide, false);

        }

        protected virtual void Start()
        {
            _context = CombatContextManager.Instance;
        }

        // ************************** move **************************

        
        public void setSpeed(float speed)
        {
            _animator.SetFloat(_animIDSpeed, speed);
        }


        // ************************** aim and shoot **************************

        public void Aim(Vector3 location)
        {
            if (_animator.GetBool(_animIDReloading)) return;
            _animator.SetBool(_animIDAim, true);
            transform.LookAt(location);
        }
        public void StopAimming()
        {
            _animator.SetBool(_animIDAim, false);
        }

        private IEnumerator CoroReloading()
        {
            _animator.SetBool(_animIDAim, false);
            _animator.SetBool(_animIDReloading, true);
            yield return _gunController.Reloading();
            _animator.SetBool(_animIDReloading, false);
            yield break;
        }

        /// <summary>
        /// if not aim, return false
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool Shoot(Vector3 location)
        {
            if (!_animator.GetBool(_animIDAim) || _animator.GetBool(_animIDReloading)) return false;

            if(_gunController.gunProperty.CurrentAmmo == 0)
            {
                StartCoroutine(CoroReloading());
                return false;
            }

            // 子弹偏移
            System.Random random = new System.Random();
            float diff_factor = 0.03f;

            var push = (location - _gunController.BulletStartTrans.position).normalized;
            push.x += ((float)random.NextDouble() - 0.5f) * diff_factor;
            push.z += ((float)random.NextDouble() - 0.5f) * diff_factor;
            var res = _gunController.Shoot(push);
            if (!res)
            {
                _animator.SetBool(_animIDShoot, false);
            }
            else
            {
                _animator.SetBool(_animIDShoot, true);
            }

            return true;
        }


    }
}
