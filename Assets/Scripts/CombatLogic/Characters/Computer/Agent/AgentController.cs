using Assets.Scripts.CombatLogic.Characters.Computer.Agent.States;
using Assets.Scripts.CombatLogic.UILogic.MiniMap;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.CombatLogic.Characters.Computer.Agent
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
        public bool isStopped => NavMeshAgent.velocity == new Vector3();
        private Vector3 _instantiatePosition;

        #region component
        private CombatContextManager _context;
        private OperatorController _controller;
        public NavMeshAgent NavMeshAgent { get; private set; }
        #endregion

        public int Team => _controller.Model.Team;
        private GameObject mapMarkUI;
        private void Awake()
        {
            _controller = GetComponent<OperatorController>();
            _context = CombatContextManager.Instance;
            // 注册其他组件
            NavMeshAgent = GetComponent<NavMeshAgent>();
            NavMeshAgent.enabled = true;
            mapMarkUI = initMiniMapMark();
        }
        private void OnDestroy()
        {
            // 注销组件
            NavMeshAgent.enabled = false;
            if(mapMarkUI != null) Destroy(mapMarkUI);
        }

        protected void Start()
        {
            _instantiatePosition = transform.position;
            if(_controller.Model.OpInfo.Type == Entities.OperatorType.CA)
            {
                states.Add(StateType.CaIdle, new CaIdle(this));
                states.Add(StateType.CaReact, new CaReact(this, _context));
                states.Add(StateType.CaAttack, new CaAttack(this));
                TranslateState(StateType.CaIdle);
            }
            else
            {
                states.Add(StateType.CvIdle, new CvIdle(this));
                states.Add(StateType.CvFollow, new CvFollow(this));
                states.Add(StateType.CvHelp, new CvHelp(this));
                TranslateState(StateType.CvIdle);
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
            currentState.OnUpdate();
            _controller.AnimatorMove(new Vector2(NavMeshAgent.velocity.x, NavMeshAgent.velocity.z), NavMeshAgent.velocity.magnitude);
        }

        // ********************** Agent Behavior ********************
        private float MoveRadius = 2.0f;
        public void RandomMove()
        {
            Vector3 moveVec = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
            moveVec = moveVec.normalized * MoveRadius;
            MoveTo(new Vector3(_instantiatePosition.x, transform.position.y, _instantiatePosition.z) + moveVec, 0.5f);
        }


        public void MoveTo(Vector3 location, float factor)
        {
            NavMeshAgent.isStopped = false;
            NavMeshAgent.speed = _controller.Model.OpInfo.MaxSpeed * factor;
            NavMeshAgent.SetDestination(location);
        }
        public void StopMoving()
        {
            NavMeshAgent.isStopped = true;
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

        /// <summary>
        /// 类视锥体检测是否能发现目标
        /// </summary>
        /// <param name="trans">值为null,或对应的角色死亡时返回null</param>
        /// <returns></returns>
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

        public void Aim(bool isAim, Vector3 aim)
        {
            _controller.Aim(isAim, aim);
        }
        public void Shoot(Vector3 aim)
        {

            if (_controller.HasAmmon()) _controller.Shoot(aim);
            else _controller.Reload();
        }
        public void Shoot(Vector3 aim, float diff_factor)
        {

            if (_controller.HasAmmon()) _controller.Shoot(aim, diff_factor);
            else _controller.Reload();
        }

        private GameObject initMiniMapMark()
        {
            var go = Instantiate(ResourceManager.Load<GameObject>("Characters/MiniMapMark"), transform);
            var mapmark = go.transform.GetComponent<MiniMapMarkUI>();

            mapmark.Inject(_controller.Model.Team, _controller.Model.OpInfo.Type);
            return go;
        }

    }
}
