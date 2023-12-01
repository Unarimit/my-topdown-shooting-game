using Assets.Scripts.CombatLogic.Characters.Computer.Fighter;
using Assets.Scripts.CombatLogic.CombatEntities;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.CombatLogic.Characters
{
    internal class OperatorController : MonoBehaviour
    {
        #region settings
        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;
        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;
        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;
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
        private int _animIDReloading;
        private Animator _animator;
        private bool _isSprint;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _terminalVelocity = 53.0f;
        /// <summary>
        /// 用于复原射击动画
        /// </summary>
        private float _shootTime = -1f;
        /// <summary>
        /// 角色处于不可控制状态
        /// </summary>
        private float _freezeTime = 0;
        #endregion



        #region component
        private GameObject _mainCamera;
        private CharacterController _controller;
        private NavMeshAgent _navMeshAgent;
        private CapsuleCollider _collider;
        private GunController _gunController;
        private CombatContextManager _context => CombatContextManager.Instance;
        #endregion

        public CombatOperator Model { get; private set; } 
        public float Speed => _speed;
        
        /// <summary>
        /// 外部作用力
        /// </summary>
        public Vector3 OutForce { get; set; }

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _collider = GetComponent<CapsuleCollider>();
            _gunController = GetComponent<GunController>();

            // AssignAnimationIDs
            
        }
        public void Inject(CombatOperator model)
        {
            Model = model;
            initGun();
            initHeadMark();
            initAnimeId();

        }


        private void Start()
        {
            if (_controller != null)
            {
                _controller.detectCollisions = false;
                _controller.enabled = true; // 默认false，防止初始化时记录错误的位置
            }
        }
        private void Update()
        {
            jumpAndGravity();
            groundedCheck();
            closeUnActiveAnimation();
            prepareFighter();

            if(_freezeTime > 0)
            {
                _freezeTime -= Time.deltaTime;
                _controller.Move(OutForce * Time.deltaTime); // TODO：CC的Move方法会互相覆盖，如果想更泛用一些，使用分量替换
            } 
        }
        /// <summary>
        /// 输入移动偏移量wasd，进行移动
        /// </summary>
        /// <param name="vec"></param>
        public void Move(Vector2 vec)
        {
            if (_freezeTime > 0) return;
            if (_controller == null)
            {
                Debug.LogWarning("do not have CharacterController component");
                return;
            }
            if (_animator.GetBool(_animIDAim))
            {
                aimMove(vec);
            }
            else
            {
                normalMove(vec);
            }
            AnimatorMove(vec, _speed);
        }
        /// <summary>
        /// 当使用agent代理移动操作时，可以使用Animator展示移动动画
        /// </summary>
        public void AnimatorMove(Vector2 vec, float speed)
        {
            _speed = speed;
            if (_animator.GetBool(_animIDAim)) aimAnimatorMove(vec);
            else normalAnimatorMove(speed);
        }
        /// <summary>
        /// 输入瞄准目标和位置
        /// </summary>
        /// <param name="isAim"></param>
        /// <param name="aim"></param>
        public bool Aim(bool isAim, Vector3 aim)
        {
            if (!tryBreakAction(ActionName.Aim)) return false;
            _animator.SetBool(_animIDAim, isAim);
            // 枪口朝向鼠标方向
            if (isAim) transform.LookAt(new Vector3(aim.x, transform.position.y, aim.z));
            return true;
        }

        public void Shoot(Vector3 aim)
        {
            if (!tryBreakAction(ActionName.Shoot)) return;
            _shootTime = Time.time;
            _gunController.ShootUseSkill(aim);
        }

        public bool HasAmmon()
        {
            return _gunController.gunProperty.CurrentAmmo != 0;
        }

        public void Shoot(Vector3 aim, float diffFactor)
        {
            if (!tryBreakAction(ActionName.Shoot)) return;
            _shootTime = Time.time;
            aim.x += Random.Range(0, diffFactor);
            aim.z += Random.Range(0, diffFactor);
            _animator.SetBool(_animIDShoot, _gunController.ShootUseSkill(aim));
        }
        public void Skill(int i, Vector3 aim)
        {
            if (tryBreakAction(ActionName.Skill))
            {
                _context.UseSkill(transform, Model.CombatSkillList[i], aim);
            }
        }
        public void Slide()
        {
            if (!tryBreakAction(ActionName.Slide)) return;
            if (!_context.UseSkill(transform, Model.SlideSkill, Vector3.zero)) return;
            _animator.SetBool(_animIDSlide, true);
            _freezeTime += Model.SlideSkill.SkillInfo.AfterCastTime;
            StartCoroutine(delayCloseSlide());
        }
        public void Jump()
        {
            if (!tryBreakAction(ActionName.Jump)) return;
            // Jump
            if (Grounded && _animator.GetBool(_animIDAim) != true && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                _animator.SetBool(_animIDJump, true);
            }
        }
        public void SetSprint(bool isSprint)
        {
            _isSprint = isSprint;
        }
        public void Reload()
        {
            if (!tryBreakAction(ActionName.Jump)) return;
            _animator.SetBool(_animIDAim, false);
            StartCoroutine(coroReloading(_gunController.Reloading()));
        }
        public void ClearAnimate()
        {
            // 清空动画
            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDAim, false);
            _animator.SetBool(_animIDShoot, false);
            _animator.SetBool(_animIDSlide, false);
            _animator.SetFloat(_animIDSpeed, 0);
        }


        protected virtual void OnFootstep(AnimationEvent animationEvent)
        {
        }

        private void aimMove(Vector2 input)
        {
            var vec = input.normalized;
            // 移动
            var moveVec = new Vector3(vec.x * Time.deltaTime, _verticalVelocity * Time.deltaTime, vec.y * Time.deltaTime);

            _controller.Move(moveVec);
            _speed = _controller.velocity.magnitude;
        }
        private void normalMove(Vector2 input)
        {
            float targetSpeed = _isSprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (input == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed;
            if (_controller != null) currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            else currentHorizontalSpeed = _navMeshAgent.velocity.magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(input.x, 0.0f, input.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (input != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }
        private void aimAnimatorMove(Vector3 input)
        {
            // 动画
            var aniVec = new Vector3(input.x, 0, input.y);
            var proj1 = Vector3.Project(aniVec, transform.forward);
            var proj2 = Vector3.Project(aniVec, transform.right);
            _animator.SetFloat(_animIDWSpeed, proj1.magnitude); // 这里没有区分正负
            _animator.SetFloat(_animIDASpeed, proj2.magnitude);
        }
        private void normalAnimatorMove(float speed)
        {

            _animator.SetFloat(_animIDSpeed, speed);
            _animator.SetFloat(_animIDMotionSpeed, 1f);
        }
        private void jumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                //_animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;
                _animator.SetBool(_animIDJump, false);
                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }

            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }
        private void groundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            _animator.SetBool(_animIDGrounded, Grounded);
        }
        private IEnumerator delayCloseSlide()
        {
            yield return new WaitForSeconds(Model.SlideSkill.SkillInfo.AfterCastTime);
            _animator.SetBool(_animIDSlide, false);
        }
        
        private enum ActionName
        {
            Slide, Jump, Aim, Shoot, Skill, Reload
        }
        private bool tryBreakAction(ActionName action)
        {
            if (_freezeTime > 0) return false;
            else if (_animator.GetBool(_animIDJump)) return false;
            else if (_animator.GetBool(_animIDReloading)) return false;
            else if (_animator.GetBool(_animIDSlide)) return false;
            return true;
        }
        private IEnumerator coroReloading(IEnumerator wait)
        {
            _animator.SetBool(_animIDReloading, true);
            yield return wait;
            _animator.SetBool(_animIDReloading, false);
            yield break;
        }
        private void initGun()
        {
            _gunController.InitGun(Model.WeaponSkill);
        }

        private void initHeadMark()
        {
            // team mark
            GameObject t_prefab = null;
            if (Model == _context.CombatVM.Player)
            {
                return;
            }
            else if (Model.Team == 0)
            {
                if (Model.OpInfo.Type == Entities.OperatorType.CA) t_prefab = ResourceManager.Load<GameObject>("Effects/TeamCAMark");
                if (Model.OpInfo.Type == Entities.OperatorType.CV) t_prefab = ResourceManager.Load<GameObject>("Effects/TeamCVMark");
            }
            else if (Model.Team == 1)
            {
                if (Model.OpInfo.Type == Entities.OperatorType.CA) t_prefab = ResourceManager.Load<GameObject>("Effects/EnemyCAMark");
                if (Model.OpInfo.Type == Entities.OperatorType.CV) t_prefab = ResourceManager.Load<GameObject>("Effects/EnemyCVMark");
            }
            Instantiate(t_prefab, transform);
        }

        private void initAnimeId()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDAim = Animator.StringToHash("Aim");
            _animIDShoot = Animator.StringToHash("Shoot");
            _animIDASpeed = Animator.StringToHash("ASpeed");
            _animIDWSpeed = Animator.StringToHash("WSpeed");
            _animIDSlide = Animator.StringToHash(Model.SlideSkill.SkillInfo.CharacterAnimeId);
            _animIDReloading = Animator.StringToHash("Reloading");
        }

        private List<FighterController> fighters = new List<FighterController>();
        private const float FIGHTER_INTERVAL = 5;
        private float curFighterInterval = FIGHTER_INTERVAL;
        private void prepareFighter()
        {
            if (Model.OpInfo.Fighters == null) return;
            if (fighters.Count < Model.OpInfo.Fighters.Count)
            {
                curFighterInterval -= Time.deltaTime;
                if (curFighterInterval < 0)
                {
                    var t = _context.GenerateFighter(Model.OpInfo.Fighters[fighters.Count].Operator,
                        transform.position, transform.eulerAngles, Model.Team, transform);
                    fighters.Add(t.GetComponent<FighterController>());
                    curFighterInterval = FIGHTER_INTERVAL;
                }
            }

        }

        private void closeUnActiveAnimation()
        {
            if (Time.time - _shootTime < 0.1f) _animator.SetBool(_animIDShoot, false);
        }
        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }
    }
}
