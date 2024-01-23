
using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.CombatLogic.GOAPs.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.CombatLogic.GOAPs
{
    /// <summary>
    /// GOAP计划，对应一棵行为树
    /// </summary>
    internal enum GOAPPlan
    {
        // ver1
        Null,
        GoAndAttack,
        SurroundAndAttack,
        RetreatAndReload,
        FollowAndHeal,
        MoveForward,

        //ver2
        RetreatAndAttack,
    }
    /// <summary>
    /// 管理GOAP过程
    /// </summary>
    internal class GOAPManager : MonoBehaviour
    {
        // 全局信息
        Dictionary<int, Transform> m_OpTransDic = new Dictionary<int, Transform>();
        Dictionary<int, CombatOperator> m_OperatorDic = new Dictionary<int, CombatOperator>();
        Dictionary<int, GOAPGraph> m_GraphDic = new Dictionary<int, GOAPGraph>();
        Dictionary<int, GOAPPlan> m_PlanDic = new Dictionary<int, GOAPPlan>();
        int[][] m_Map; // 应该映射为障碍物map

        bool initSuccess = false;
        public void Inject(CombatContextManager context)
        {
            m_Map = context.CombatVM.Level.Map;
            foreach(var x in context.Operators)
            {
                m_OperatorDic.Add(x.Value.Id, x.Value);
                m_GraphDic.Add(x.Value.Id, BuildHelper.BuildActionGraphForAgent(x.Value));
                m_PlanDic.Add(x.Value.Id, GOAPPlan.Null);
                m_OpTransDic.Add(x.Value.Id, x.Key);
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
                if (m_OperatorDic[id].IsPlayer is true || // 忽略玩家
                    m_OperatorDic[id].IsDead is true ||  // 忽略死亡agent
                    m_OpTransDic[id].GetComponent<AgentController>().enabled is false) // 忽略暂停行为
                    continue; 

                var list = findFieldOfViewEnemy(id);
                // 注意不需要搜索敌人的情况
                var state = GOAPStatusHelper.CalcState(m_OperatorDic[id], 
                    m_OpTransDic[id].GetComponent<GunController>().gunProperty, 
                    false, 
                    list.Count != 0, 
                    list.Where(x => x.distance < m_OperatorDic[id].AttackRange + 2).Count() != 0); // 触发距离应长于实际攻击距离

                var res = m_GraphDic[id].DoPlan(state);

                if(GOAPDebugger.Instance != null) GOAPDebugger.Instance.PrintActions(m_GraphDic[id].Name, res);

                // 执行plan
                if (res.Count == 0) throw new Exception($"{m_GraphDic[id].Name} plan result cnt is 0");
                switch (res[0].GOAPPlan) {
                    case GOAPPlan.Null:
                        throw new Exception($"can not do null plan in {res[0].ActionName}");
                    case GOAPPlan.MoveForward:
                        if (m_PlanDic[id] == res[0].GOAPPlan) break; // 不重复执行该计划
                        m_OpTransDic[id].GetComponent<AgentController>().DoMove(calPatrolPos(id));
                        break;
                    case GOAPPlan.GoAndAttack:
                        // 可能会出现目标不同的情况，交给AgentController处理
                        m_OpTransDic[id].GetComponent<AgentController>().DoGoAndAttack(getMostValuableTarget(list));
                        break;
                    case GOAPPlan.SurroundAndAttack:
                        // 可能会出现目标不同的情况，交给AgentController处理
                        m_OpTransDic[id].GetComponent<AgentController>().DoSurroundAndAttack(getMostValuableTarget(list));
                        break;
                    case GOAPPlan.RetreatAndReload:
                        // 可能会出现目标不同的情况，交给AgentController处理
                        m_OpTransDic[id].GetComponent<AgentController>().DoRetreatAndReload(findNearlyEnemy(id));
                        break;
                    case GOAPPlan.FollowAndHeal:
                        // 可能会出现目标不同的情况，交给AgentController处理
                        m_OpTransDic[id].GetComponent<AgentController>().DoFollowAndHeal(findNearlyTeammate(id));
                        break;
                }
                m_PlanDic[id] = res[0].GOAPPlan;
            }
        }


        struct FieldOfViewItem
        {
            public int id;
            public float distance;
        }
        /// <summary>
        /// 寻找agent视野内的敌方单位
        /// </summary>
        private List<FieldOfViewItem> findFieldOfViewEnemy(int cid)
        {
            float ligalAngle = 120f;
            float ligalDistance = m_OperatorDic[cid].SeeRange;

            // 获取敌人列表
            var enemyIdList = new List<int>();
            if (m_OperatorDic[cid].Team == 0) 
                enemyIdList = m_OperatorDic.Where(x => x.Value.Team == 1 && x.Value.IsDead is false).Select(x => x.Key).ToList();
            else if (m_OperatorDic[cid].Team == 1) 
                enemyIdList = m_OperatorDic.Where(x => x.Value.Team == 0 && x.Value.IsDead is false).Select(x => x.Key).ToList();
            else 
                throw new Exception($"error Team{m_OperatorDic[cid].Team}");

            // 遍历查找满足条件的
            var res = new List<FieldOfViewItem>();
            foreach(var enemyId in enemyIdList)
            {
                // 距离
                var selfPos = new Vector2(m_OpTransDic[cid].position.x, m_OpTransDic[cid].position.z);
                var enemyPos = new Vector2(m_OpTransDic[enemyId].position.x, m_OpTransDic[enemyId].position.z);
                var d = Vector2.Distance(selfPos, enemyPos);
                if (d > ligalDistance) continue;

                // 角度
                var selfForword = new Vector2(m_OpTransDic[cid].forward.x, m_OpTransDic[cid].forward.z);
                float angleToEnemy = Vector2.Angle(selfForword, enemyPos - selfPos);
                if (angleToEnemy > ligalAngle / 2f) continue;

                // 障碍物
                if (isLineHaveObstacle(selfPos, enemyPos) is true) continue;

                res.Add(new FieldOfViewItem { id = enemyId, distance = d });
            }
            return res;
        }

        /// <summary>
        /// 寻找附近的队友
        /// </summary>
        private GameObject findNearlyTeammate(int cid)
        {
            // 获取队友列表
            var teamIdList = new List<int>();
            teamIdList = m_OperatorDic.Where(x => m_OperatorDic[x.Key].Team == m_OperatorDic[cid].Team && x.Value.IsDead is false && x.Value.Id != cid)
                .Select(x => x.Key)
                .ToList();
            if(teamIdList.Count == 0) return null;
            else if(teamIdList.Count == 1 && teamIdList[0] == cid) return null;

            // 遍历查找满足条件的
            int res = -1;
            float res_distance = 0;
            foreach (var teamId in teamIdList)
            {

                var selfPos = new Vector2(m_OpTransDic[cid].position.x, m_OpTransDic[cid].position.z);
                var teamPos = new Vector2(m_OpTransDic[teamId].position.x, m_OpTransDic[teamId].position.z);
                var d = Vector2.Distance(teamPos, selfPos);
                if(res == -1)
                {
                    res = teamId;
                    res_distance = d;
                }
                else if(d < res_distance)
                {
                    res_distance = d;
                    res = teamId;
                }
            }
            if (res == -1) return null;
            return m_OpTransDic[res].gameObject;
        }

        /// <summary>
        /// 获得最有价值的目标（通常是敌方）
        /// </summary>
        private GameObject getMostValuableTarget(List<FieldOfViewItem> data)
        {
            int res = 0;
            for(int i = 0; i < data.Count; i++)
            {
                if (data[i].distance < data[res].distance) res = i;
            }
            return m_OpTransDic[data[res].id].gameObject; // 暂时以距离最近的
        }

        private GameObject findNearlyEnemy(int cid)
        {
            var enemyIdList = m_OperatorDic.Where(x => m_OperatorDic[x.Key].Team != m_OperatorDic[cid].Team && x.Value.IsDead is false)
                .Select(x => x.Key)
                .ToList();
            if (enemyIdList.Count == 0) return null;
            else if (enemyIdList.Count == 1 && enemyIdList[0] == cid) return null;

            int res = -1;
            float res_distance = 0;
            foreach (var teamId in enemyIdList)
            {

                var selfPos = new Vector2(m_OpTransDic[cid].position.x, m_OpTransDic[cid].position.z);
                var teamPos = new Vector2(m_OpTransDic[teamId].position.x, m_OpTransDic[teamId].position.z);
                var d = Vector2.Distance(teamPos, selfPos);
                if (res == -1)
                {
                    res = teamId;
                    res_distance = d;
                }
                else if (d < res_distance)
                {
                    res_distance = d;
                    res = teamId;
                }
            }
            if (res == -1) return null;
            return m_OpTransDic[res].gameObject;
        }

        private Vector2 calPatrolPos(int cid)
        {
            var enemyIdList = m_OperatorDic.Where(x => m_OperatorDic[x.Key].Team != m_OperatorDic[cid].Team && x.Value.IsDead is false)
                .Select(x => x.Key)
                .ToList();
            // 没有敌人活着时
            if (enemyIdList.Count == 0) return new Vector2(m_OperatorDic[cid].SpawnBase.position.x, m_OperatorDic[cid].SpawnBase.position.z);

            var randomEnemy = m_OpTransDic[enemyIdList[Random.Range(0, enemyIdList.Count)]];
            return new Vector2(randomEnemy.position.x + Random.Range(3f, 5f), randomEnemy.position.z + Random.Range(3f, 5f));
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
