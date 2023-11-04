using Assets.Scripts.CombatLogic.ComputerControllers.States;
using Assets.Scripts.ComputerControllers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.CombatLogic.ComputerControllers
{
    public class AgentController : PersonController
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
        private new void Awake()
        {
            base.Awake();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        protected override void Start()
        {

            base.Start();
            _instantiatePosition = transform.position;

            states.Add(StateType.Idle, new IdleState(this));
            states.Add(StateType.React, new ReactState(this));
            states.Add(StateType.Attack, new AttackState(this));

            TranslateState(StateType.Idle);

            if (_gunController != null)
            {
                _gunController.gunProperty.MuzzleVelocity = 1200;
                _gunController.gunProperty.RateOfFile = 300;
                _gunController.gunProperty.CurrentAmmo = 10;
                _gunController.gunProperty.MaxAmmo = 10;
            }
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
            base.setSpeed(_navMeshAgent.velocity.sqrMagnitude);
            currentState.OnUpdate();
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
            _navMeshAgent.speed = MaxSpeed / 2;
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
        protected float FindAngle = 30f;
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

        private void OnFootstep(AnimationEvent animationEvent)
        {

        }
    }
}
