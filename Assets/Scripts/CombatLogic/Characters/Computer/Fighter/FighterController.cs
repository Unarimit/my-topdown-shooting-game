using Assets.Scripts.CombatLogic.Characters.Computer.Fighter.States;
using Assets.Scripts.CombatLogic.ComputerControllers.Fighter.States;
using Assets.Scripts.CombatLogic.ComputerControllers.States;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

namespace Assets.Scripts.CombatLogic.Characters.Computer.Fighter
{
    internal class FighterController : MonoBehaviour
    {
        // inspector
        public IFighterState.StateType m_StateType;

        // component
        internal Transform m_CharactorTrans;
        internal NavMeshAgent m_NavAgent;

        // 属性
        internal Transform CvBase { get; set; }
        internal int Team => _context.Operators[CvBase].Team;
        internal Transform Aim { get; set; }
        internal CombatCombatSkill _skill { get; private set; }


        private Dictionary<IFighterState.StateType, IFighterState> states = new Dictionary<IFighterState.StateType, IFighterState>();
        private IFighterState currentState;
        private CombatContextManager _context => CombatContextManager.Instance;

        // setting
        private const float CHARACTER_END_Y = 2; // 起飞动画参数
        private const float CHARACTER_END_SCALE = 0.7f; // 起飞动画参数

        private void Awake()
        {
            m_CharactorTrans = transform.Find("modelroot");
            m_NavAgent = transform.GetComponent<NavMeshAgent>();
            _skill = new CombatCombatSkill(SkillManager.Instance.skillConfig.CombatSkills[5]);

            states.Add(IFighterState.StateType.Idle, new IdleState(this));
            states.Add(IFighterState.StateType.Attack, new AttackState(this));
            states.Add(IFighterState.StateType.Return, new ReturnState(this));
            currentState = states[IFighterState.StateType.Idle];
        }
        private void Start()
        {
            // 起飞过渡动画
            m_CharactorTrans.DOLocalMoveY(CHARACTER_END_Y, 2);
            m_CharactorTrans.DOScale(CHARACTER_END_SCALE, 2);
        }
        private void Update()
        {
            m_CharactorTrans.position += flyVerticleSimulate();
            currentState.OnUpdate();
        }
        public void TranslateState(IFighterState.StateType state)
        {
            if (currentState != null)
                currentState.OnExit();
            currentState = states[state];
            currentState.OnEnter();
            m_StateType = state;
        }

        /// <summary>
        /// nav to aim
        /// </summary>
        public void SetDest(Vector3 vec)
        {
            m_NavAgent.SetDestination(vec);
        }

        /// <summary>
        /// 俯冲轰炸
        /// </summary>
        public void DiveBombing(Vector3 aim)
        {
            _context.UseSkill(m_CharactorTrans, _skill, aim);
        }


        /// <summary>
        /// 返回飞行时上下移动的增量
        /// </summary>
        private Vector3 flyVerticleSimulate()
        {
            return new Vector3(0, Mathf.Sin(Time.time * 2) * Time.deltaTime * 0.3f, 0);
        }
    }
}
