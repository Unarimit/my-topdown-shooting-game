
using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using Assets.Scripts.CombatLogic.CombatEntities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.GOAPs
{
    internal enum GOAPPlans
    {
        GoAndShoot,
        SurrondAndShoot,
        RetreatAndReload,
        Patrol,

    }

    internal class GOAPManager : MonoBehaviour
    {
        bool initSuccess = false;
        Dictionary<int, AgentController> m_AgentControllerDic;
        Dictionary<int, CombatOperator> m_OperatorDic;
        int[][] m_Map;

        public void Inject(CombatContextManager context)
        {
            m_Map = context.CombatVM.Level.Map;
            m_AgentControllerDic = new Dictionary<int, AgentController>();
            m_OperatorDic = new Dictionary<int, CombatOperator>();
            foreach(var x in context.Operators)
            {
                m_AgentControllerDic.Add(x.Value.Id, x.Key.GetComponent<AgentController>());
                m_OperatorDic.Add(x.Value.Id, x.Value);
            }
            initSuccess = true;
        }


        float time = 0;
        private void Update()
        {
            time += Time.deltaTime;
            if(time >= 0.1f && initSuccess is true)
            {
                time = 0;
                updatePlan();
            }
        }
        /// <summary>
        /// 主要逻辑，使用GOAP为所有agent计划
        /// </summary>
        private void updatePlan()
        {

        }
    }
}
