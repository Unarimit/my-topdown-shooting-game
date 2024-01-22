using Assets.Scripts.CombatLogic.GOAPs;
using Assets.Scripts.CombatLogic.UILogic.MiniMap;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tactical.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
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
        public NavMeshAgent NavMeshAgent { get; private set; }
        #endregion

        public int Team => _controller.Model.Team;
        private GameObject mapMarkUI;

        private Dictionary<GOAPPlan, BehaviorTree> m_BTreeDic;
        private BehaviorTree activeBTree;
        private void Awake()
        {
            _controller = GetComponent<OperatorController>();
            _context = CombatContextManager.Instance;

            // AI
            m_BTreeDic = getBehaviorTreeDic();
            activeBTree = m_BTreeDic[GOAPPlan.Null];

            // 注册其他组件
            NavMeshAgent = GetComponent<NavMeshAgent>();
            NavMeshAgent.enabled = true;
            mapMarkUI = initMiniMapMark();
        }
        private void Start()
        {
            activeBTree.enabled = true;
        }
        private void OnEnable()
        {
            activeBTree.enabled = true; // 它里面会自己判断，重复调用没有问题
            activeBTree.EnableBehavior();
        }
        private void OnDisable()
        {
            if (_context.Operators[transform].IsDead is true)
            {
                activeBTree.enabled = false;
                activeBTree = m_BTreeDic[GOAPPlan.Null];
            }
            else
            {
                activeBTree.enabled = false;
            }
            
        }
        private void OnDestroy()
        {
            // 注销组件
            activeBTree.enabled = false;
            NavMeshAgent.enabled = false;
            if(mapMarkUI != null) Destroy(mapMarkUI);
        }
        internal void DoMove(Vector2 patrolPos)
        {
            if (canBreakBehavior() is false) return;
            activeBTree.enabled = false;
            activeBTree = m_BTreeDic[GOAPPlan.MoveForward];
            activeBTree.SetVariable("target", (SharedVector3)new Vector3(patrolPos.x, 0, patrolPos.y));
            startBehavior();
        }
        private GameObject _last_aim_gaa;
        internal void DoGoAndAttack(GameObject aim)
        {
            if (canBreakBehavior() is false) return;
            if (aim == null) return;
            if (activeBTree == m_BTreeDic[GOAPPlan.GoAndAttack] && _last_aim_gaa == aim) return;

            activeBTree.enabled = false;
            activeBTree = m_BTreeDic[GOAPPlan.GoAndAttack];
            activeBTree.SetVariable("target", (SharedGameObjectList)new List<GameObject>() { aim });
            startBehavior();
            _last_aim_gaa = aim;
        }
        private GameObject _last_aim_saa;
        internal void DoSurroundAndAttack(GameObject aim)
        {
            if (canBreakBehavior() is false) return;
            if (aim == null) return;
            if (activeBTree == m_BTreeDic[GOAPPlan.GoAndAttack] && _last_aim_saa == aim) return;
            activeBTree.enabled = false;
            activeBTree = m_BTreeDic[GOAPPlan.SurroundAndAttack];
            activeBTree.SetVariable("target", (SharedGameObjectList)new List<GameObject>() { aim });
            startBehavior();
            _last_aim_saa = aim;
        }
        private GameObject _last_aim_rar;
        internal void DoRetreatAndReload(GameObject nealy_enemy)
        {
            if (canBreakBehavior() is false) return;
            if (nealy_enemy == null) return;
            if (activeBTree == m_BTreeDic[GOAPPlan.GoAndAttack] && _last_aim_rar == nealy_enemy) return;
            activeBTree.enabled = false;
            activeBTree = m_BTreeDic[GOAPPlan.RetreatAndReload];
            if(nealy_enemy != null) activeBTree.SetVariable("target", (SharedGameObjectList)new List<GameObject>() { nealy_enemy });
            startBehavior();
            _last_aim_rar = nealy_enemy;
        }
        private GameObject _last_aim_fah;
        internal void DoFollowAndHeal(GameObject aim)
        {
            if (canBreakBehavior() is false) return;
            if (aim == null) return;
            if (activeBTree == m_BTreeDic[GOAPPlan.GoAndAttack] && _last_aim_fah == aim) return;
            activeBTree.enabled = false;
            activeBTree = m_BTreeDic[GOAPPlan.FollowAndHeal];
            activeBTree.SetVariable("teammate", (SharedGameObjectList)new List<GameObject>() { aim });
            startBehavior();
            _last_aim_fah = aim;
        }

        private bool canBreakBehavior()
        {
            var canbreak = activeBTree.GetVariable("canBreak");
            if (canbreak == null) return true;
            else return ((SharedBool)canbreak).Value;
        }
        private void startBehavior()
        {
            activeBTree.enabled = true;
            activeBTree.EnableBehavior();
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
        private GameObject initMiniMapMark()
        {
            var go = Instantiate(ResourceManager.Load<GameObject>("Characters/MiniMapMark"), transform);
            var mapmark = go.transform.GetComponent<MiniMapMarkUI>();

            mapmark.Inject(_controller.Model.Team, _controller.Model.OpInfo.Type);
            return go;
        }
        private Dictionary<GOAPPlan, BehaviorTree> getBehaviorTreeDic()
        {
            var res = new Dictionary<GOAPPlan, BehaviorTree>();
            var agentTrees = transform.GetComponents<BehaviorTree>();
            foreach(var x in agentTrees)
            {
                res.Add(Enum.Parse<GOAPPlan>(x.BehaviorName), x);
            }
            return res;
        }
    }
}
