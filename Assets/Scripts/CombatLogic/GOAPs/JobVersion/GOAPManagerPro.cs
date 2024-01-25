using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.CombatLogic.GOAPs.Builders;
using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.CombatLogic.GOAPs.JobVersion
{
    /// <summary>
    /// 使用job system重写的GOAPManager，一定程度遵循了DOTS
    /// </summary>
    internal class GOAPManagerPro : MonoBehaviour
    {
        public struct CombatCharacterColdData
        {
            /// <summary> 角色id </summary>
            public int CId;
            /// <summary> 角色出生点 </summary>
            public Vector2 SpawnPosition;
            /// <summary> 所属队伍 </summary>
            public int Team;
            /// <summary> 角色类型 </summary>
            public OperatorType Type;
            /// <summary> 角色特性 </summary>
            public OperatorTrait Trait;
            /// <summary> 是否使用武器 </summary>
            public bool IsUseGun;
            /// <summary> 最大子弹数 </summary>
            public int MaxAmmo;
            /// <summary> 视野范围 </summary>
            public float SeeRange;
            /// <summary> 攻击范围 </summary>
            public float AttackRange;
            /// <summary> 最大生命 </summary>
            public int MaxHp;
        }
        public struct CombatCharacterHotData
        {
            /// <summary> 角色位置 </summary>
            public Vector2 Position;
            /// <summary> 角色朝向 </summary>
            public Vector2 Forward;
            /// <summary> 是否死亡 </summary>
            public bool IsDead;
            /// <summary> 现在生命 </summary>
            public int Hp;
            /// <summary> 现在子弹数 </summary>
            public int Ammo;
            /// <summary> 是否由玩家控制 </summary>
            public bool IsPlayer;
            /// <summary>  </summary>
            public bool HaveNoCdAttackSkill;
        }
        public struct PlanResultData
        {
            public GOAPPlan Plan;
            public int TargetCid;
            public Vector2 TargetPos;
            public PlanResultData(GOAPPlan plan)
            {
                Plan = plan;
                TargetCid = -1;
                TargetPos = new Vector2();
            }
        }
        public struct GOAPJob : IJob
        {
            [ReadOnly]
            public UnsafeHashMap<int, UnsafeList<int>> Map;
            [ReadOnly]
            public NativeHashMap<int, CombatCharacterColdData> CharactorColdDatas;
            [ReadOnly]
            public UnsafeHashMap<int, GOAPGraphPro> CharactorGraphs;

            public NativeHashMap<int, CombatCharacterHotData> CharactorHotDatas;
            public NativeHashMap<int, PlanResultData> CharactorPlans;
            public void Execute()
            {
                foreach (var x in CharactorHotDatas)
                {
                    if(CharactorPlans.ContainsKey(x.Key) is false)
                    {
                        CharactorPlans.Add(x.Key, new PlanResultData(GOAPPlan.Null));
                    }
                }

                foreach (var x in CharactorHotDatas)
                {
                    int id = x.Key;
                    // 视野范围的敌人
                    var list = findFieldOfViewEnemy(id);
                    // 计算初始状态
                    float attackRange = CharactorColdDatas[id].AttackRange;
                    var initState = CalcInitState(id,
                        false,
                        list.Count != 0,
                        list.Where(x => x.distance < attackRange + 2).Count() != 0);


                    var res = CharactorGraphs[id].DoPlan(initState);
                    PlanResultData resultData = new PlanResultData(res[0].GOAPPlan);
                    switch (res[0].GOAPPlan)
                    {
                        case GOAPPlan.Null:
                            throw new Exception($"can not do null plan in unkown action");
                        case GOAPPlan.MoveForward:
                            resultData.TargetPos = calPatrolPos(id);
                            break;
                        case GOAPPlan.GoAndAttack:
                            resultData.TargetCid = getMostValuableTarget(list);
                            break;
                        case GOAPPlan.SurroundAndAttack:
                            resultData.TargetCid = getMostValuableTarget(list);
                            break;
                        case GOAPPlan.RetreatAndReload:
                            resultData.TargetCid = findNearlyEnemy(id);
                            break;
                        case GOAPPlan.FollowAndHeal:
                            resultData.TargetCid = findNearlyTeammate(id);
                            break;
                    }
                    CharactorPlans[x.Key]= resultData;
                }
            }
            /// <summary>
            /// 计算初始状态
            /// </summary>
            private uint CalcInitState(int cid, bool inAttack, bool canSeeEnemy, bool isEnemyInRange)
            {
                var coldData = new NativeReference<CombatCharacterColdData>(CharactorColdDatas[cid], Allocator.Temp);
                var hotData = new NativeReference<CombatCharacterHotData>(CharactorHotDatas[cid], Allocator.Temp);
                uint res = 0;
                /*****************  简单状态  *****************/
                // 角色状态
                // -- HP
                if (hotData.Value.Hp < coldData.Value.MaxHp / 3) res |= (uint)1 << (int)GOAPStatus.LowHP;
                // -- 武器
                if (coldData.Value.IsUseGun is true)
                {
                    res |= (uint)1 << (int)GOAPStatus.UseGun;
                    if (hotData.Value.Ammo == 0) res |= (uint)1 << (int)GOAPStatus.NoAmmo;
                    else if (coldData.Value.MaxAmmo > hotData.Value.Ammo * 3)
                    {
                        res |= (uint)1 << (int)GOAPStatus.LowAmmo;
                    }
                }

                // -- 技能
                if (hotData.Value.HaveNoCdAttackSkill) res |= (uint)1 << (int)GOAPStatus.HaveNoCdAttackSkill;

                // op性格
                if (coldData.Value.Trait == Entities.OperatorTrait.Offensive) res |= (uint)1 << (int)GOAPStatus.Offensive;
                else if (coldData.Value.Trait == Entities.OperatorTrait.Timid) res |= (uint)1 << (int)GOAPStatus.Timid;
                else if (coldData.Value.Trait == Entities.OperatorTrait.Tactical) res |= (uint)1 << (int)GOAPStatus.Tactical;


                // 角色观察状态比较难算
                if (inAttack) res |= (uint)1 << (int)GOAPStatus.InAttack;
                if (canSeeEnemy) res |= (uint)1 << (int)GOAPStatus.SawEnemy;
                if (isEnemyInRange) res |= (uint)1 << (int)GOAPStatus.EnemyInRange;

                return res;
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
                float ligalDistance = CharactorColdDatas[cid].SeeRange;

                // 获取敌人列表
                List<int> enemyIdList;
                if (CharactorColdDatas[cid].Team == 0)
                    enemyIdList = getTargetTeamCharacterList(1, true, null);
                else if (CharactorColdDatas[cid].Team == 1)
                    enemyIdList = getTargetTeamCharacterList(0, true, null);
                else
                    throw new Exception($"error Team{CharactorColdDatas[cid].Team}");

                // 遍历查找满足条件的
                var res = new List<FieldOfViewItem>();
                foreach (var enemyId in enemyIdList)
                {
                    // 距离
                    var selfPos = CharactorHotDatas[cid].Position;
                    var enemyPos = CharactorHotDatas[enemyId].Position;
                    var d = Vector2.Distance(selfPos, enemyPos);
                    if (d > ligalDistance) continue;

                    // 角度
                    var selfForword = CharactorHotDatas[cid].Forward;
                    float angleToEnemy = Vector2.Angle(selfForword, enemyPos - selfPos);
                    if (angleToEnemy > ligalAngle / 2f) continue;

                    // 障碍物
                    if (isLineHaveObstacle(selfPos, enemyPos) is true) continue;

                    res.Add(new FieldOfViewItem { id = enemyId, distance = d });
                }
                return res;
            }

            /// <summary>
            /// 寻找附近的队友，找不到返回-1
            /// </summary>
            private int findNearlyTeammate(int cid)
            {
                // 获取队友列表
                List<int> teamIdList;
                teamIdList = getTargetTeamCharacterList(CharactorColdDatas[cid].Team, true, cid);
                if (teamIdList.Count == 0) return -1;
                else if (teamIdList.Count == 1 && teamIdList[0] == cid) return -1;

                // 遍历查找满足条件的
                int res = -1;
                float res_distance = 0;
                foreach (var teamId in teamIdList)
                {

                    var selfPos = CharactorHotDatas[cid].Position;
                    var teamPos = CharactorHotDatas[teamId].Position;
                    var d = Vector2.Distance(teamPos, selfPos);

                    if (CharactorPlans[teamId].Plan == GOAPPlan.FollowAndHeal) continue; // 防止出现互相跟随的情况

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
                if (res == -1) return -1;
                return res;
            }

            /// <summary>
            /// 获得最有价值的目标（通常是敌方）
            /// </summary>
            private int getMostValuableTarget(List<FieldOfViewItem> data)
            {
                int res = 0;
                for (int i = 0; i < data.Count; i++)
                {
                    if (data[i].distance < data[res].distance) res = i;
                }
                return data[res].id; // 暂时以距离最近的
            }

            private int findNearlyEnemy(int cid)
            {
                // 获取敌人列表
                List<int> enemyIdList;
                if (CharactorColdDatas[cid].Team == 0)
                    enemyIdList = getTargetTeamCharacterList(1, true, null);
                else if (CharactorColdDatas[cid].Team == 1)
                    enemyIdList = getTargetTeamCharacterList(0, true, null);
                else
                    throw new Exception($"error Team{CharactorColdDatas[cid].Team}");

                if (enemyIdList.Count == 0) return -1;
                else if (enemyIdList.Count == 1 && enemyIdList[0] == cid) return -1;

                int res = -1;
                float res_distance = 0;
                foreach (var teamId in enemyIdList)
                {

                    var selfPos = CharactorHotDatas[cid].Position;
                    var teamPos = CharactorHotDatas[teamId].Position;
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
                if (res == -1) return -1;
                return res;
            }

            private Vector2 calPatrolPos(int cid)
            {
                // 获取敌人列表
                List<int> enemyIdList;
                if (CharactorColdDatas[cid].Team == 0)
                    enemyIdList = getTargetTeamCharacterList(1, true, null);
                else if (CharactorColdDatas[cid].Team == 1)
                    enemyIdList = getTargetTeamCharacterList(0, true, null);
                else
                    throw new Exception($"error Team{CharactorColdDatas[cid].Team}");

                // 没有敌人活着时
                if (enemyIdList.Count == 0) return CharactorColdDatas[cid].SpawnPosition;
                Random rand = new Random();
                return CharactorHotDatas[enemyIdList[rand.Next(0, enemyIdList.Count)]].Position;
            }

            /// <summary>
            /// 线在地图上是否有障碍物？
            /// </summary>
            private bool isLineHaveObstacle(Vector2 start, Vector2 end)
            {
                // TODO： finish it
                return false;
            }
            private List<int> getTargetTeamCharacterList(int team, bool alive, int? excludeCid)
            {
                var res = new List<int>();
                foreach(var x in CharactorHotDatas)
                {
                    if (CharactorColdDatas[x.Key].Team != team) continue;
                    if (alive && x.Value.IsDead is true) continue;
                    if (excludeCid != null && x.Key == excludeCid) continue;
                    res.Add(x.Key);
                }
                return res;
            }
        }
        GOAPJob reusedJob;
        bool initSuccess = false;
        Dictionary<int, Transform> m_OpTransDic = new Dictionary<int, Transform>();
        Dictionary<int, CombatOperator> m_OperatorDic = new Dictionary<int, CombatOperator>();
        Dictionary<int, GOAPPlan> m_PlanDic = new Dictionary<int, GOAPPlan>();
        public void Inject(CombatContextManager context)
        {
            // 引用信息
            foreach(var x in context.Operators)
            {
                m_OperatorDic.Add(x.Value.Id, x.Value);
                m_OpTransDic.Add(x.Value.Id, x.Key);
                m_PlanDic.Add(x.Value.Id, GOAPPlan.Null);
            }

            // 地图信息
            reusedJob.Map = new UnsafeHashMap<int, UnsafeList<int>>(context.CombatVM.Level.Map.Length, Allocator.Persistent);
            for(int i = 0; i < context.CombatVM.Level.Map.Length; i++)
            {
                var list = new UnsafeList<int>(context.CombatVM.Level.Map[i].Length, Allocator.Persistent);
                for (int j = 0; j < context.CombatVM.Level.Map[0].Length; j++) list.Add(context.CombatVM.Level.Map[i][j]);
                reusedJob.Map[i] = list;
            }

            // 人物信息
            reusedJob.CharactorColdDatas = new NativeHashMap<int, CombatCharacterColdData>(context.Operators.Count, Allocator.Persistent);
            reusedJob.CharactorGraphs = new UnsafeHashMap<int, GOAPGraphPro>(context.Operators.Count, Allocator.Persistent);
            reusedJob.CharactorPlans = new NativeHashMap<int, PlanResultData>(context.Operators.Count, Allocator.Persistent);
            foreach (var x in context.Operators.Values)
            {
                reusedJob.CharactorColdDatas.Add(x.Id, new CombatCharacterColdData
                {
                    CId = x.Id,
                    SpawnPosition = new Vector2(x.SpawnBase.position.x, x.SpawnBase.position.z),
                    Team = x.Team,
                    Type = x.OpInfo.Type,
                    Trait = x.OpInfo.Trait,
                    IsUseGun = x.WeaponSkill.SkillInfo.IsGun,
                    MaxAmmo = x.WeaponSkill.SkillInfo.Ammo,
                    SeeRange = x.SeeRange,
                    AttackRange = x.AttackRange,
                    MaxHp = x.MaxHP,
                });
                reusedJob.CharactorGraphs.Add(x.Id, BuildHelper.BuildActionGraphForAgent(x).ToPro());
            }
            initSuccess = true;
        }
        
        JobHandle? handle;
        private void Update()
        {
            //if (handle != null && handle.Value.IsCompleted) handle = null;
            //if (handle != null) return;

            prepareHotDataForJob();
            reusedJob.Run();
            processPlanForJob();
            

            //handle = reusedJob.Schedule();

        }
        private void prepareHotDataForJob()
        {
            reusedJob.CharactorHotDatas = new NativeHashMap<int, CombatCharacterHotData>(reusedJob.CharactorColdDatas.Count, Allocator.TempJob);
            foreach (var x in m_OperatorDic.Values)
            {
                reusedJob.CharactorHotDatas.Add(x.Id, new CombatCharacterHotData
                {
                    Position = new Vector2(m_OpTransDic[x.Id].position.x, m_OpTransDic[x.Id].position.z),
                    Forward = new Vector2(m_OpTransDic[x.Id].forward.x, m_OpTransDic[x.Id].forward.z),
                    IsDead = x.IsDead,
                    IsPlayer = x.IsPlayer,
                    Hp = x.CurrentHP,
                    Ammo = m_OpTransDic[x.Id].GetComponent<GunController>().gunProperty.CurrentAmmo,
                    HaveNoCdAttackSkill = GOAPStatusHelper.HaveNoCdAttackSkill(x),
                });
            }
        }
        private void processPlanForJob()
        {
            foreach(var res in reusedJob.CharactorPlans)
            {
                int id = res.Key;

                if (m_OperatorDic[id].IsPlayer is true || // 忽略玩家
                    m_OperatorDic[id].IsDead is true ||  // 忽略死亡agent
                    m_OpTransDic[id].GetComponent<AgentController>().enabled is false) // 忽略暂停行为
                    continue;

                switch (res.Value.Plan)
                {
                    case GOAPPlan.Null:
                        throw new Exception($"return a null plan in job result");
                    case GOAPPlan.MoveForward:
                        if (m_PlanDic[id] == res.Value.Plan &&
                            m_OpTransDic[id].GetComponent<AgentController>().IsBehaviorFinish() is false)
                            break; // 不重复执行该计划

                        m_OpTransDic[id].GetComponent<AgentController>().DoMove(res.Value.TargetPos);
                        break;
                    case GOAPPlan.GoAndAttack:
                        // 可能会出现目标不同的情况，交给AgentController处理
                        if (res.Value.TargetCid == -1) break;
                        m_OpTransDic[id].GetComponent<AgentController>().DoGoAndAttack(m_OpTransDic[res.Value.TargetCid].gameObject);
                        break;
                    case GOAPPlan.SurroundAndAttack:
                        // 可能会出现目标不同的情况，交给AgentController处理
                        m_OpTransDic[id].GetComponent<AgentController>().DoSurroundAndAttack(m_OpTransDic[res.Value.TargetCid].gameObject);
                        break;
                    case GOAPPlan.RetreatAndReload:
                        // 可能会出现目标不同的情况，交给AgentController处理
                        m_OpTransDic[id].GetComponent<AgentController>().DoRetreatAndReload(m_OpTransDic[res.Value.TargetCid].gameObject);
                        break;
                    case GOAPPlan.FollowAndHeal:
                        // 可能会出现目标不同的情况，交给AgentController处理
                        m_OpTransDic[id].GetComponent<AgentController>().DoFollowAndHeal(m_OpTransDic[res.Value.TargetCid].gameObject);
                        break;
                }
                m_PlanDic[id] = res.Value.Plan;
            }
        }

    }
}
