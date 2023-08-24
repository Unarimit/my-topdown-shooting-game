using Assets.Scripts.BulletLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

namespace Assets.Scripts.ComputerControllers
{
    /// <summary>
    /// 基本的人物控制器，可以开枪、和行走
    /// </summary>
    public class PersonController : MonoBehaviour
    {
        [Header("Shot")]
        [Tooltip("Bullet prefab")]
        public GameObject Bullet;
        [Tooltip("GunFire GameObject")]
        public GameObject GunFire;
        [Tooltip("Bullet start trans")]
        public Transform BulletStartTrans;
        public Transform Enviorment;


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

        [HideInInspector]
        public GameInformationManager _gameInformationManager;
        private CharacterController _controller;
        private DestructiblePersonController _destructiblePersonController;




        private bool Moving = false;

        private void Awake()
        {
            bool temp = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _destructiblePersonController = GetComponent<DestructiblePersonController>();
            _destructiblePersonController.HittedEvent += DestructiblePersonController_HittedEvent;
            if (!temp) Debug.LogError(transform.ToString() + " have no animator");

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
            _animator.SetBool(_animIDGrounded, true);
            _animator.SetFloat(_animIDMotionSpeed, 1);
        }

        private void DestructiblePersonController_HittedEvent(object sender, Vector3 hitSourcePos)
        {
            transform.LookAt(hitSourcePos);
        }

        public virtual void Start()
        {
            _gameInformationManager = GameInformationManager.Instance;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"> shoud be (x, transform.y, z) </param>
        /// <param name="Speed"></param>
        public void MoveOnce(Vector3 location, float Speed)
        {
            transform.LookAt(location);
            if (Moving)
            {
                Debug.LogError(transform.ToString() + " is in Moving! can not trigger more Moving");
            }
            else
            {
                StartCoroutine(MoveOnceSub(location, Speed));
            }
            
        }
        IEnumerator MoveOnceSub(Vector3 location, float Speed)
        {
            Moving = true;
            _animator.SetFloat(_animIDSpeed, Speed);
            while (transform.position != location)
            {
                if (!Moving) break;
                var aim = location - transform.position;
                var moveVec = aim.normalized * Time.deltaTime * Speed;
                // 防止运动超出界限
                if(moveVec.magnitude >= aim.magnitude * Time.deltaTime * Speed)
                {
                    _controller.Move(aim * Time.deltaTime * Speed);
                    break;
                }
                else _controller.Move(moveVec);
                yield return null;
            }
            Moving = false;
            _animator.SetFloat(_animIDSpeed, 0);
            yield return null;
        }

        public void Aim(Vector3 location)
        {
            _animator.SetBool(_animIDAim, true);
            transform.LookAt(location);
        }

        System.Random random = new System.Random();
        /// <summary>
        /// if not aim, return false
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool Shoot(MyGun gun, Vector3 location)
        {
            float diff_factor = 0.03f;
            if (!_animator.GetBool(_animIDAim)) return false;

            var b = Instantiate(Bullet, Enviorment);
            b.transform.position = BulletStartTrans.position;
            var res = gun.ShootOrWait(b, transform.forward);
            if (!res)
            {
                Destroy(b);
                GunFire.SetActive(false);
                _animator.SetBool(_animIDShoot, false);
            }
            else
            {
                GunFire.SetActive(true);
                _animator.SetBool(_animIDShoot, true);
                var push = (location - BulletStartTrans.position).normalized;
                push.x += ((float)random.NextDouble() - 0.5f) * diff_factor;
                push.z += ((float)random.NextDouble() - 0.5f) * diff_factor;

                StartCoroutine(gun.DelayForce(b, push));
            }

            return true;
        }


        private void OnFootstep(AnimationEvent animationEvent)
        {
            
        }
    }
}
