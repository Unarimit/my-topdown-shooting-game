using Assets.Scripts.CombatLogic.UILogic.MiniMap;
using Assets.Scripts.Entities;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tactical.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Random = UnityEngine.Random;

namespace Assets.Scripts.CombatLogic.Characters.Computer.Agent
{
    public class AgentController : MonoBehaviour
    {
        [HideInInspector]
        public Vector3 aimPos;
        [HideInInspector]
        public Transform aimTran;

        #region component
        private CombatContextManager _context;
        private OperatorController _controller;
        public NavMeshAgent NavMeshAgent { get; private set; }
        #endregion

        public int Team => _controller.Model.Team;
        private GameObject mapMarkUI;

        // 单一行为树
        private BehaviorTree _behaviorTree;

        // 行为树变量
        private SharedGameObjectList _targetVariable;
        private SharedGameObjectList _teammateVariable;
        private SharedVector3 _moveTargetVariable;

        private void Awake()
        {
            _controller = GetComponent<OperatorController>();
            _context = CombatContextManager.Instance;

            // 获取单一行为树
            _behaviorTree = GetComponent<BehaviorTree>();
            if (_behaviorTree == null)
            {
                Debug.LogError("BehaviorTree component not found on Agent!");
                return;
            }

            // 注册其他组件
            NavMeshAgent = GetComponent<NavMeshAgent>();
            NavMeshAgent.enabled = true;
            mapMarkUI = initMiniMapMark();
        }

        private void Start()
        {
            // 在Start中初始化行为树变量（确保行为树已经加载）
            InitializeBehaviorTreeVariables();
            
            if (_behaviorTree != null)
            {
                _behaviorTree.enabled = true;
                _behaviorTree.EnableBehavior();
            }
        }

        private void InitializeBehaviorTreeVariables()
        {
            if (_behaviorTree == null) return;

            // 确保行为树变量存在
            _targetVariable = _behaviorTree.GetVariable("target") as SharedGameObjectList;
            if (_targetVariable == null)
            {
                _targetVariable = new SharedGameObjectList();
                _targetVariable.Value = new List<GameObject>();
                _behaviorTree.SetVariable("target", _targetVariable);
            }
            else if (_targetVariable.Value == null)
            {
                _targetVariable.Value = new List<GameObject>();
            }

            _teammateVariable = _behaviorTree.GetVariable("teammate") as SharedGameObjectList;
            if (_teammateVariable == null)
            {
                _teammateVariable = new SharedGameObjectList();
                _teammateVariable.Value = new List<GameObject>();
                _behaviorTree.SetVariable("teammate", _teammateVariable);
            }
            else if (_teammateVariable.Value == null)
            {
                _teammateVariable.Value = new List<GameObject>();
            }

            _moveTargetVariable = _behaviorTree.GetVariable("moveTarget") as SharedVector3;
            if (_moveTargetVariable == null)
            {
                _moveTargetVariable = new SharedVector3();
                _moveTargetVariable.Value = Vector3.zero;
                _behaviorTree.SetVariable("moveTarget", _moveTargetVariable);
            }

            // 初始化canBreak变量
            var canBreakVar = _behaviorTree.GetVariable("canBreak") as SharedBool;
            if (canBreakVar == null)
            {
                canBreakVar = new SharedBool();
                canBreakVar.Value = true;
                _behaviorTree.SetVariable("canBreak", canBreakVar);
            }
        }

        private void OnEnable()
        {
            if (_behaviorTree != null)
            {
                _behaviorTree.enabled = true;
                _behaviorTree.EnableBehavior();
            }
        }

        private void OnDisable()
        {
            if (_behaviorTree == null) return;

            if (_context.Operators[transform].IsDead is true) // 死亡导致的OnDisable
            {
                _behaviorTree.enabled = false;
                _controller.ClearAnimate();
            }
            else // 暂定行为导致的OnDisable
            {
                _behaviorTree.enabled = false;
            }
        }

        private void OnDestroy()
        {
            if (_behaviorTree != null)
            {
                _behaviorTree.enabled = false;
            }
            if (NavMeshAgent != null)
            {
                NavMeshAgent.enabled = false;
            }
            if (mapMarkUI != null) Destroy(mapMarkUI);
        }

        // 设置目标敌人
        public void SetTarget(GameObject target)
        {
            if (_targetVariable == null || target == null) return;
            _targetVariable.Value = new List<GameObject> { target };
        }

        // 设置目标队友
        public void SetTeammate(GameObject teammate)
        {
            if (_teammateVariable == null || teammate == null) return;
            _teammateVariable.Value = new List<GameObject> { teammate };
        }

        // 设置移动目标
        public void SetMoveTarget(Vector3 position)
        {
            if (_moveTargetVariable == null) return;
            _moveTargetVariable.Value = position;
        }

        // 清除目标
        public void ClearTargets()
        {
            if (_targetVariable != null) _targetVariable.Value = new List<GameObject>();
            if (_teammateVariable != null) _teammateVariable.Value = new List<GameObject>();
        }

        public bool IsBehaviorFinish()
        {
            if (_behaviorTree == null) return true;
            return _behaviorTree.ExecutionStatus != BehaviorDesigner.Runtime.Tasks.TaskStatus.Running;
        }

        public void Aim(bool isAim, Vector3 aim)
        {
            _controller.Aim(isAim, aim);
        }

        float scatter = 1f;
        SkillTargetTip? weaponTargetTip = null;
        public void Shoot(Vector3 aim)
        {
            if (weaponTargetTip == null) weaponTargetTip = _controller.Model.WeaponSkill.SkillInfo.TargetTip;
            if (_controller.HasAmmon())
            {
                if (weaponTargetTip == SkillTargetTip.TeammateSingle) _controller.Shoot(aim);
                if (weaponTargetTip == SkillTargetTip.EnemySingle) _controller.Shoot(aim + new Vector3(Random.Range(0, scatter), Random.Range(0, scatter), Random.Range(0, scatter)));
            }
            else _controller.Reload();
        }

        private GameObject initMiniMapMark()
        {
            var go = Instantiate(ResourceManager.Load<GameObject>("Characters/MiniMapMark"), transform);
            var mapmark = go.transform.GetComponent<MiniMapMarkUI>();

            mapmark.Inject(_controller.Model.Team, _controller.Model.OpInfo.Type);
            return go;
        }

        // 供行为树条件任务调用的方法
        public bool HasAmmo()
        {
            return _controller.HasAmmon();
        }

        public bool IsLowHealth()
        {
            return _context.Operators[transform].CurrentHP < _context.Operators[transform].MaxHP / 3;
        }

        public bool IsHurt()
        {
            return _context.Operators[transform].CurrentHP < _context.Operators[transform].MaxHP;
        }

        public bool HasEnemyInRange()
        {
            // 视野范围内查找敌人
            float seeRange = _controller.Model.SeeRange;
            float attackRange = _controller.Model.AttackRange;
            int myTeam = _controller.Model.Team;

            foreach (var op in _context.Operators)
            {
                if (op.Value.Team == myTeam) continue;
                if (op.Value.IsDead) continue;

                float distance = Vector3.Distance(transform.position, op.Key.position);
                if (distance <= seeRange)
                {
                    // 简单视野检测（角度）
                    Vector3 directionToEnemy = (op.Key.position - transform.position).normalized;
                    float angle = Vector3.Angle(transform.forward, directionToEnemy);
                    if (angle <= 60f) // 120度视野范围
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public GameObject GetNearestEnemy()
        {
            float seeRange = _controller.Model.SeeRange;
            int myTeam = _controller.Model.Team;
            GameObject nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (var op in _context.Operators)
            {
                if (op.Value.Team == myTeam) continue;
                if (op.Value.IsDead) continue;

                float distance = Vector3.Distance(transform.position, op.Key.position);
                if (distance <= seeRange && distance < nearestDistance)
                {
                    nearest = op.Key.gameObject;
                    nearestDistance = distance;
                }
            }
            return nearest;
        }

        public GameObject GetNearestTeammate()
        {
            int myTeam = _controller.Model.Team;
            GameObject nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (var op in _context.Operators)
            {
                if (op.Value.Team != myTeam) continue;
                if (op.Value.IsDead) continue;
                if (op.Key == transform) continue;

                float distance = Vector3.Distance(transform.position, op.Key.position);
                if (distance < nearestDistance)
                {
                    nearest = op.Key.gameObject;
                    nearestDistance = distance;
                }
            }
            return nearest;
        }

        public Vector3 GetPatrolPosition()
        {
            // 向随机方向移动
            return transform.position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
        }

        public OperatorTrait GetTrait()
        {
            return _controller.Model.OpInfo.Trait;
        }
    }
}