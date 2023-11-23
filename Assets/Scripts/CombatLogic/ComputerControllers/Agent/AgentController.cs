using Assets.Scripts.CombatLogic.ComputerControllers.States.Agent;
using Assets.Scripts.CombatLogic.MyCharacterControllers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.CombatLogic.ComputerControllers
{
    public class AgentController : MonoBehaviour
    {
        private Dictionary<StateType, IAgentState> states = new Dictionary<StateType, IAgentState>();
        private IAgentState currentState;

        [Tooltip("for debug use")]
        public StateType type;

        [HideInInspector]
        public Vector3 aimPos;
        [HideInInspector]
        public Transform aimTran;
        public bool isStopped => _navMeshAgent.velocity == new Vector3();
        private Vector3 _instantiatePosition;
        private NavMeshAgent _navMeshAgent;
        private CombatContextManager _context;
        private OperatorController _controller;
        private new void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        protected void Start()
        {
            _instantiatePosition = transform.position;
            _context = CombatContextManager.Instance;
            _controller = GetComponent<OperatorController>();
            states.Add(StateType.Idle, new IdleState(this));
            states.Add(StateType.React, new ReactState(this, _context));
            states.Add(StateType.Attack, new AttackState(this));

            TranslateState(StateType.Idle);
        }

        public void TranslateState(StateType state)
        {
            if (currentState != null)
                currentState.OnExit();
            currentState = states[state];
            currentState.OnEnter();
            type = state;
        }
        private void Update()
        {
            currentState.OnUpdate();
            _controller.AnimatorMove(new Vector2(_navMeshAgent.velocity.x, _navMeshAgent.velocity.z), _navMeshAgent.velocity.magnitude);
        }

        // ********************** Agent Behavior ********************
        private float MoveRadius = 2.0f;
        public void RandomMove()
        {
            Vector3 moveVec = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
            moveVec = moveVec.normalized * MoveRadius;
            MoveTo(new Vector3(_instantiatePosition.x, transform.position.y, _instantiatePosition.z) + moveVec, 2.0f);
        }

        public Vector3 TryFindAim()
        {
            if (_context.Operators[transform].Team == 1) return _context.PlayerTrans.position;
            else return _context.EnemyTeamTrans.FirstOrDefault() == null ? new Vector3() : _context.EnemyTeamTrans[0].position;
        }

        public void MoveTo(Vector3 location, float MaxSpeed)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = MaxSpeed;
            _navMeshAgent.SetDestination(location);
        }
        public void StopMoving()
        {
            _navMeshAgent.isStopped = true;
        }

        // ************************** normal detect **************************

        public struct SeeMsg
        {
            public bool Found;
            public bool FromSelf;
            public Vector3 FoundPos;
            public Transform FoundTrans;
        }

        private float FindDistance = 10f;
        protected float FindAngle = 60f;

        /// <summary>
        /// 尝试发现敌人
        /// </summary>
        /// <returns></returns>
        public SeeMsg TrySeeCounters(List<Transform> CounterGroup)
        {
            var forward = transform.forward;
            foreach (var x in CounterGroup)
            {
                if (x == null || _context.Operators[x].IsDead) continue;
                var vec = x.position - transform.position;
                if (Vector3.Angle(forward, vec) < FindAngle && vec.magnitude < FindDistance) // in my eyes
                {
                    // it is in my eyes
                    Ray ray = new Ray(transform.position, vec);
                    var hits = Physics.RaycastAll(ray, vec.magnitude, LayerMask.GetMask(new string[] { "Obstacle" }));
                    if (hits.Length == 0)
                    {
                        return new SeeMsg { Found = true, FoundPos = x.position, FoundTrans = x, FromSelf = true };
                    }
                }
            }
            return new SeeMsg { Found = false };
        }

        public bool TrySeeAim(Transform trans)
        {
            if (trans == null || _context.Operators[trans].IsDead) return false;
            var vec = trans.position - transform.position;
            if (Vector3.Angle(transform.forward, vec) < FindAngle && vec.magnitude < FindDistance) // in my eyes
            {
                // it is in my eyes
                Ray ray = new Ray(transform.position, vec);
                var hits = Physics.RaycastAll(ray, vec.magnitude, LayerMask.GetMask(new string[] { "Obstacle" }));
                if (hits.Length == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void Aim(Vector3 aim)
        {
            _controller.Aim(true, aim);
        }
        public void Shoot(Vector3 aim, float diff_factor)
        {
            
            if(_controller.HasAmmon()) _controller.Shoot(aim, diff_factor);
            else _controller.Reload();
        }

    }
}
