using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

namespace Assets.Scripts.CombatLogic.EnviormentLogic
{
    public class OperatorAnimatorBaseController : MonoBehaviour
    {
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

        private GameObject _mainCamera;
        private CharacterController _controller;
        private NavMeshAgent _navMeshAgent;
        private CapsuleCollider _collider;
        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _collider = GetComponent<CapsuleCollider>();

            // AssignAnimationIDs
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
            _animIDReloading = Animator.StringToHash("Reloading");
        }

        private void Update()
        {
            JumpAndGravity();
            GroundedCheck();
        }

        public void ChangeMoveSpeed(float speed)
        {

            _animator.SetFloat(_animIDSpeed, speed);
            _animator.SetFloat(_animIDMotionSpeed, 1f);
        }
        public Vector3 GetMoveVec(Vector2 input)
        {
            if (_animator.GetBool(_animIDAim)) return AimMove(input);
            else return NormalMove(input);
        }
        public void DoJump()
        {
            // Jump
            if (Grounded && _animator.GetBool(_animIDAim) != true && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                _animator.SetBool(_animIDJump, true);
            }
        }
        public void SetAim(bool isAim)
        {
            _animator.SetBool(_animIDAim, isAim);
        }
        public void SetSprint(bool isSprint)
        {
            _isSprint = isSprint;
        }
        public void SetShoot(bool isShoot)
        {
            _animator.SetBool(_animIDShoot, isShoot);
        }
        public void DoReload(IEnumerator reloadLogic)
        {

            StartCoroutine(CoroReloading(reloadLogic));
        }
        public void DoSlide()
        {
            _animator.SetBool(_animIDSlide, true);
            ChangeCollider(true);
            StartCoroutine(DelayCloseSlide());
        }
        IEnumerator DelayCloseSlide()
        {
            yield return new WaitForEndOfFrame();
            _animator.SetBool(_animIDSlide, false);
            yield return new WaitForSeconds(2f);
            ChangeCollider(false);
        }
        public void StopAll()
        {
            // 清空动画
            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDAim, false);
            _animator.SetBool(_animIDShoot, false);
            _animator.SetBool(_animIDSlide, false);
        }

        public enum ActionName{
            Slide, Jump, Aim, Shoot, Skill, Reload
        }
        /// <summary>
        /// 是否可以打断当前动作，做新动作
        /// </summary>
        /// <returns></returns>
        public bool TryBreakAction(ActionName actionName)
        {
            if (_animator.GetBool(_animIDJump)) return false;
            else if (_animator.GetBool(_animIDReloading)) return false;
            else if(_animator.GetBool(_animIDSlide)) return false;
            return true;
        }

        private IEnumerator CoroReloading(IEnumerator wait)
        {
            _animator.SetBool(_animIDReloading, true);
            yield return wait;
            _animator.SetBool(_animIDReloading, false);
            yield break;
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

             _animator.SetBool(_animIDGrounded, Grounded);
        }
        private Vector3 NormalMove(Vector2 input)
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _isSprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (input == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed;
            if (_controller != null) currentHorizontalSpeed =  new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
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

            ChangeMoveSpeed(_animationBlend);

            // move the player
            return targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;

        }
        private Vector3 AimMove(Vector3 input)
        {
            var vec = input.normalized;
            // 移动
            var moveVec = new Vector3(vec.x * Time.deltaTime, _verticalVelocity * Time.deltaTime, vec.y * Time.deltaTime);

            // 动画
            var aniVec = new Vector3(vec.x, 0, vec.y);
            var proj1 = Vector3.Project(aniVec, transform.forward);
            var proj2 = Vector3.Project(aniVec, transform.right);
            _animator.SetFloat(_animIDWSpeed, proj1.magnitude); // 这里没有区分正负
            _animator.SetFloat(_animIDASpeed, proj2.magnitude);

            return moveVec;
        }
        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _terminalVelocity = 53.0f;
        private void JumpAndGravity()
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

        /// <summary>
        /// 用于下蹲时切换Collider的范围
        /// </summary>
        private void ChangeCollider(bool isSquat)
        {
            if (isSquat)
            {
                _collider.center = new Vector3(0, 0.475f, 0);
                _collider.height = 0.75f;
            }
            else
            {
                _collider.center = new Vector3(0, 0.8f, 0);
                _collider.height = 1.4f;
            }

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
        protected virtual void OnFootstep(AnimationEvent animationEvent)
        {
        }
    }
}
