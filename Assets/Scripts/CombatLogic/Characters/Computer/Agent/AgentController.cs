using Assets.Scripts.CombatLogic.GOAPs;
using Assets.Scripts.CombatLogic.UILogic.MiniMap;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tactical.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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
        private BehaviorTree _bTree;
        public NavMeshAgent NavMeshAgent { get; private set; }
        #endregion

        public int Team => _controller.Model.Team;
        private GameObject mapMarkUI;
        private void Awake()
        {
            _controller = GetComponent<OperatorController>();
            _context = CombatContextManager.Instance;
            _bTree = GetComponent<BehaviorTree>();
            // 注册其他组件
            NavMeshAgent = GetComponent<NavMeshAgent>();
            NavMeshAgent.enabled = true;
            mapMarkUI = initMiniMapMark();
        }
        private void Start()
        {
            if(Team == 0) _bTree.SetVariable("test", (SharedGameObjectList)_context.EnemyTeamTrans.Select(x => x.gameObject).ToList());
            else if(Team == 1) _bTree.SetVariable("test", (SharedGameObjectList)_context.PlayerTeamTrans.Select(x => x.gameObject).ToList());
            _bTree.enabled = true;
            //_bTree.EnableBehavior();
        }
        private void OnEnable()
        {
            if(_bTree.enabled is true) _bTree.EnableBehavior(); // 它里面会自己判断，重复调用没有问题
        }
        private void OnDisable()
        {
            _bTree.DisableBehavior();
        }
        private void OnDestroy()
        {
            // 注销组件
            _bTree.enabled = false;
            NavMeshAgent.enabled = false;
            if(mapMarkUI != null) Destroy(mapMarkUI);
        }
        internal void DoPatrol(Vector2 patrolPos)
        {

        }
        internal void DoGoAndAttack(GameObject aim) 
        { 

        }
        internal void DoSurroundAndAttack(GameObject aim)
        {

        }
        internal void DoRetreatAndReload(Vector2 retreatPos)
        {

        }
        internal void DoFollowAndHeal(GameObject aim)
        {

        }

        // ************************** normal detect **************************

        protected float FindAngle = 60f;
        public void Aim(bool isAim, Vector3 aim)
        {
            _controller.Aim(isAim, aim);
        }
        public void Shoot(Vector3 aim)
        {

            if (_controller.HasAmmon()) _controller.Shoot(aim);
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
