using Assets.Scripts.BulletLogic;
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

        [HideInInspector]
        public CombatContextManager _gameInformationManager;
        private DestructiblePersonController _destructiblePersonController;
        private NavMeshAgent _navMeshAgent;


        protected bool Moving = false;

        private void Awake()
        {
            bool temp = TryGetComponent(out _animator);
            _destructiblePersonController = GetComponent<DestructiblePersonController>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _destructiblePersonController.HittedEvent += HittedEvent;
            _destructiblePersonController.HP0Event += HP0Event;
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
            _animIDDied = Animator.StringToHash("Died");
            _animator.SetBool(_animIDGrounded, true);
            _animator.SetFloat(_animIDMotionSpeed, 1);
        }

        private void HittedEvent(object sender, Vector3 hitSourcePos)
        {
            transform.LookAt(new Vector3(hitSourcePos.x, 0, hitSourcePos.z));
        }
        private void HP0Event(object sender)
        {
            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDAim, false);
            _animator.SetBool(_animIDShoot, false);
            _animator.SetBool(_animIDSlide, false);
            _animator.SetBool(_animIDDied, true);
            _gameInformationManager.EnemyTeamTrans.Remove(transform);
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
            this.enabled = false;
        }
        protected virtual void Start()
        {
            _gameInformationManager = CombatContextManager.Instance;
        }

        // ************************** move **************************

        /// <summary>
        /// 一次性move到某个地方
        /// </summary>
        /// <param name="location"> shoud be (x, transform.y, z) </param>
        /// <param name="Speed"></param>
        public void MoveOnce(Vector3 location, float MaxSpeed)
        {
            _navMeshAgent.speed = MaxSpeed/2;
            _navMeshAgent.SetDestination(location);
        }
        protected void StopMoving()
        {
            Moving = false;
            _navMeshAgent.isStopped = true;
        }
        protected void baseUpdate()
        {
            _animator.SetFloat(_animIDSpeed, _navMeshAgent.velocity.sqrMagnitude);
        }


        // ************************** aim and shoot **************************

        public void Aim(Vector3 location)
        {
            _animator.SetBool(_animIDAim, true);
            transform.LookAt(location);
        }
        public void StopAimming()
        {
            _animator.SetBool(_animIDAim, false);
        }

        
        /// <summary>
        /// if not aim, return false
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool Shoot(Vector3 location)
        {
            if (!_animator.GetBool(_animIDAim)) return false;

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

        // ************************** normal detect **************************

        protected struct FoundMsg
        {
            public bool Found;
            public bool FromSelf;
            public Vector3 FoundPos;
        }

        private float FindDistance = 10f;
        protected float FindAngle = 30f;
        /// <summary>
        /// 尝试发现敌人
        /// </summary>
        /// <returns></returns>
        protected FoundMsg TryFindCounters(List<Transform> CounterGroup)
        {
            var forward = transform.forward;
            foreach (var x in CounterGroup)
            {
                if (x == null) continue;
                var vec = x.position - transform.position;
                if (Vector3.Angle(forward, vec) < FindAngle && vec.magnitude < FindDistance) // in my eyes
                {
                    // it is in my eyes
                    Ray ray = new Ray(transform.position, vec);
                    var hits = Physics.RaycastAll(ray, vec.magnitude, LayerMask.GetMask(new string[] { "Obstacle" }));
                    if (hits.Length == 0)
                    {
                        return new FoundMsg { Found = true, FoundPos = x.position, FromSelf = true };
                    }
                }
            }
            return new FoundMsg { Found = false };
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            
        }
    }
}
