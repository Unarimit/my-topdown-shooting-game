﻿using Assets.Scripts.BulletLogic;
using Assets.Scripts.CombatLogic.UILogic.MiniMap;
using Assets.Scripts.Services;
using StarterAssets;
using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif
using Random = UnityEngine.Random;


namespace Assets.Scripts.CombatLogic.Characters.Player
{
    /// <summary>
    /// 玩家控制类
    /// </summary>
    [RequireComponent(typeof(OperatorController))]
    public class PlayerController : MonoBehaviour
    {

        public delegate void InteractEvent(Transform trans);
        public event InteractEvent InteractEventHandler;

        #region component
        private OperatorController _controller;
        private StarterAssetsInputs _input;
        private CombatContextManager _context => CombatContextManager.Instance;
        private DestructiblePersonController _destructiblePersonController;
        #endregion

        #region 伴随生命周期注册和销毁的组件
        private GameObject cameraFlowing;
        private GameObject mapMarkUI;
        private void Awake()
        {
            _controller = GetComponent<OperatorController>();
            // 注册其他组件
            // 相机追踪
            cameraFlowing = Instantiate(ResourceManager.Load<GameObject>("Characters/CameraFlowing"), transform);
            _context.m_Camera.Follow = cameraFlowing.transform;
            _context.CombatVM.PlayerTrans = transform;
            // 小地图
            mapMarkUI = initMiniMapMark();
            // 玩家标识
            transform.tag = MyConfig.PLAYER_TAG;
        }
        private void OnDestroy()
        {
            // 玩家标识
            transform.tag = MyConfig.UNTAGED_TAG;
            // 注销组件
            if (cameraFlowing != null) Destroy(cameraFlowing);
            if (mapMarkUI != null) Destroy(mapMarkUI);
        }
        #endregion
        private void Start()
        {
            _input = StarterAssetsInputs.Instance;
            _destructiblePersonController = GetComponent<DestructiblePersonController>();

        }
        private GameObject initMiniMapMark()
        {
            var go = Instantiate(ResourceManager.Load<GameObject>("Characters/MiniMapMark"), transform);
            var mapmark = go.transform.GetComponent<MiniMapMarkUI>();

            mapmark.Inject(_controller.Model.Team, _controller.Model.OpInfo.Type, true);
            return go;
        }

        private void Update()
        {
            Move();
            Skill();
            if (_controller.Model.WeaponSkill.SkillInfo.IsGun) AimAndShoot();
            else Attack();
            DoTriggerAction();
        }
        private void DoTriggerAction()
        {
            _controller.SetSprint(_input.sprint);
            if (_input.jump)
            {
                _controller.Jump();
                _input.jump = false;
            }
            else if (_input.slide) _controller.Slide();
            else if (_input.reloading) _controller.Reload();
            else if (_input.interact)
            {
                if(InteractEventHandler != null) InteractEventHandler.Invoke(transform);
            }
        }
        #region 按键逻辑
        private void Move()
        {
            _controller.Move(_input.move);
        }

        private float GunLine = 0.95f; // 枪线，应该在准星上
        private void Attack()
        {
            if (_input.shoot)
            {
                _controller.MeleeAttack(getMouseAiming());
            }
        }
        private void AimAndShoot()
        {
            // 瞄准和射击的动作控制
            if (_input.aim && _controller.Aim(true, getMouseAiming()))
            {
                _context.CombatVM.IsPlayerAimming = true;
                // 开始射击
                if (_input.shoot)
                {
                    _controller.Shoot(getMouseAiming());
                }
            }
            else
            {
                _context.CombatVM.IsPlayerAimming = false;
                _controller.Aim(false, Vector3.zero);
            }
        }
        private void Skill()
        {
            if (_input.skill1 || _input.skill2)
            {
                if (_input.skill1) _controller.Skill(0, getMouseAiming());
                if (_input.skill2) _controller.Skill(1, getMouseAiming());
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
        #endregion
    }
}