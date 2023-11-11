using Assets.Scripts;
using Assets.Scripts.BulletLogic;
using Assets.Scripts.CombatLogic;
using Assets.Scripts.CombatLogic.EnviormentLogic;
using System.Collections;
using Unity.Burst.Intrinsics;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using static Assets.Scripts.GunController;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]

    public class ThirdPersonController : MonoBehaviour
    {
        public GunController GunController;


        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;


        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;


        private OperatorAnimatorBaseController _animatorController;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private CombatContextManager _context;




        private DestructiblePersonController _destructiblePersonController;
        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _controller.detectCollisions = false;
            _animatorController = GetComponent<OperatorAnimatorBaseController>();
            _input = StarterAssetsInputs.Instance;
            _context = CombatContextManager.Instance;

            _destructiblePersonController = GetComponent<DestructiblePersonController>();
            _destructiblePersonController.HP0Event += HP0Event;


        }

        private void Update()
        {
            Move();
            Skill();
            DoTriggerAction();
        }

        private void FixedUpdate()
        {
            AimAndShoot();
        }

        private void DoTriggerAction()
        {
            _animatorController.SetSprint(_input.sprint);
            if (_input.jump && _animatorController.TryBreakAction(OperatorAnimatorBaseController.ActionName.Jump))
            {
                _animatorController.DoJump();
                _input.jump = false;
            }
            if(_input.slide && _animatorController.TryBreakAction(OperatorAnimatorBaseController.ActionName.Slide))
            {
                _animatorController.DoSlide();
            }
            if(_input.reloading && _animatorController.TryBreakAction(OperatorAnimatorBaseController.ActionName.Reload))
                _animatorController.DoReload(GunController.Reloading());
            
        }
        
        private void Move()
        {
            var moveVec = _animatorController.GetMoveVec(_input.move);
            _controller.Move(moveVec);
            _context.Operators[transform].Speed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        }
        
        

        private float GunLine = 0.95f; // 枪线，应该在准星上
        private void AimAndShoot()
        {
            // 瞄准和射击的动作控制
            if (_input.aim && _animatorController.TryBreakAction(OperatorAnimatorBaseController.ActionName.Aim))
            {
                _animatorController.SetAim(true);
                var aim = getMouseAiming();
                // 枪口朝向鼠标方向
                transform.LookAt(new Vector3(aim.x, transform.position.y, aim.z));

                // 开始射击
                if (_input.shoot)
                {
                    if(GunController.Shoot((aim - GunController.BulletStartTrans.position).normalized))
                    {
                        _animatorController.SetShoot(true);
                        _context.Operators[transform].ActAttack();
                    }
                    else
                    {
                        _animatorController.SetShoot(false);
                    }
                }

                //transform.Rotate(new Vector3(0, 45, 0));
            }
            else
            {
                _animatorController.SetAim(false);
                _animatorController.SetShoot(false);
            }
        }


        //TODO：分离技能逻辑
        private void Skill()
        {
            if((_input.skill1 || _input.skill2) && _animatorController.TryBreakAction(OperatorAnimatorBaseController.ActionName.Skill))
            {
                if (_input.skill1) _context.UseSkill(transform, 0, getMouseAiming(), Time.time);
                if (_input.skill2) _context.UseSkill(transform, 1, getMouseAiming(), Time.time);
            }
        }



        private Vector3 getMouseAiming()
        {
            var mouse = Mouse.current.position.ReadValue();
            var ray = Camera.main.ScreenPointToRay(mouse);
            var hits = Physics.RaycastAll(ray, 100f, LayerMask.GetMask(new string[] { "Ground" }));
            var aim = new Vector3(); // 射击目标
            if (hits.Length == 1)
            {
                var temp = hits[0].point;
                aim = new Vector3(
                    temp.x + GunLine / ray.direction.y * ray.direction.x,
                    GunLine,
                    temp.z + GunLine / ray.direction.y * ray.direction.z
                );
            }
            else
            {
                Debug.Log("can not hit ground " + hits.Length);
            }
            return aim;
        }


        private void HP0Event(object sender)
        {
            // 清空动画
            _animatorController.StopAll();

        }



        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

    }
}