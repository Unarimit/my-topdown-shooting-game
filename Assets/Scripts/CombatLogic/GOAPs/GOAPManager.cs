
using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.CombatLogic.GOAPs.Builders;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.GOAPs
{
    /// <summary>
    /// GOAP计划，对应一棵行为树
    /// </summary>
    internal enum GOAPPlan
    {
        Null,
        GoAndAttack,
        SurrondAndAttack,
        RetreatAndReload,
        FollowAndHeal,
        Patrol,

    }
    /// <summary>
    /// 管理GOAP过程
    /// </summary>
    internal class GOAPManager : MonoBehaviour
    {
        // 全局信息
        Dictionary<int, AgentController> m_AgentControllerDic = new Dictionary<int, AgentController>();
        Dictionary<int, CombatOperator> m_OperatorDic = new Dictionary<int, CombatOperator>();
        Dictionary<int, GOAPGraph> m_GraphDic = new Dictionary<int, GOAPGraph>();
        int[][] m_Map; // 应该映射为障碍物map

        bool initSuccess = false;
        public void Inject(CombatContextManager context)
        {
            m_Map = context.CombatVM.Level.Map;
            foreach(var x in context.Operators)
            {
                if (x.Value.IsPlayer is true) continue;

                m_AgentControllerDic.Add(x.Value.Id, x.Key.GetComponent<AgentController>());
                m_OperatorDic.Add(x.Value.Id, x.Value);
                m_GraphDic.Add(x.Value.Id, BuildHelper.BuildActionGraphForAgent(x.Value));
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
        /// 主要逻辑，使用GOAP为所有agent进行规划
        /// </summary>
        private void updatePlan()
        {
            foreach(var id in m_OperatorDic.Keys)
            {
                var list = findFieldOfViewEnemy(id);
                // 注意不需要搜索敌人的情况
                var state = GOAPStatusHelper.CalcState(m_OperatorDic[id], m_AgentControllerDic[id].GunProperty, false, list.Count != 0);
                m_GraphDic[id].DoPlan(state);

            }
        }



        float angle = 60f;
        float distance = 10f;
        /// <summary>
        /// 寻找agent视野内的敌方单位
        /// </summary>
        private List<int> findFieldOfViewEnemy(int cid)
        {
            var enemyIdList = new List<int>();
            if (m_OperatorDic[cid].Team == 0) enemyIdList = m_AgentControllerDic.Where(x => m_OperatorDic[x.Key].Team == 1).Select(x => x.Key).ToList();
            else if (m_OperatorDic[cid].Team == 1) enemyIdList = m_AgentControllerDic.Where(x => m_OperatorDic[x.Key].Team == 0).Select(x => x.Key).ToList();
            else throw new System.Exception($"error Team{m_OperatorDic[cid].Team}");

            var res = new List<int>();
            foreach(var enemyId in enemyIdList)
            {
                // 距离
                var selfPos = new Vector2(m_AgentControllerDic[cid].transform.position.x, m_AgentControllerDic[cid].transform.position.z);
                var enemyPos = new Vector2(m_AgentControllerDic[enemyId].transform.position.x, m_AgentControllerDic[enemyId].transform.position.z);
                if (Vector2.Distance(selfPos, enemyPos) > distance) continue;

                // 角度
                var selfForword = new Vector2(m_AgentControllerDic[cid].transform.forward.x, m_AgentControllerDic[cid].transform.forward.z);
                float angleToEnemy = Vector2.Angle(selfForword, enemyPos);
                if (angleToEnemy > angle / 2f) continue;

                // 障碍物
                if (isLineHaveObstacle(selfPos, enemyPos) is true) continue;

                res.Add(enemyId);
            }
            return res;
        }
        /// <summary>
        /// 线在地图上是否有障碍物？
        /// </summary>
        private bool isLineHaveObstacle(Vector2 start, Vector2 end)
        {
            // TODO： finish it
            return false;
        }
    }
}
