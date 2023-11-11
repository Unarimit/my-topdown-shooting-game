﻿using Assets.Scripts.BulletLogic;
using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.Entities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.CombatLogic
{

    public class CombatContextManager : MonoBehaviour
    {
        /// <summary>
        /// singleton
        /// </summary>
        public static CombatContextManager Instance;
        // ******************* inspector *************
        /// <summary>
        /// 己方干员列表，0号为玩家位
        /// </summary>
        public List<Transform> PlayerTeamTrans;

        public List<Transform> EnemyTeamTrans;

        public Transform Enviorment;

        /// <summary>
        /// agent的父trans，方便debug
        /// </summary>
        public Transform AgentsSpawnTrans;

        // ******************* end inspector *************

        /// <summary>
        /// 所有干员列表，在start初始化
        /// </summary>
        public Dictionary<Transform, CombatOperator> Operators { get; private set; }

        public Transform PlayerTrans => PlayerTeamTrans[0];

        private SkillManager _skillContext;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
            TeammateStatu = TeammateStatus.Follow;

            Time.timeScale = 1;

        }

        private void Start()
        {
            _skillContext = SkillManager.Instance;

            // 队员跟随状态显示
            TeammateText.text = TeammateStatu.ToString();

            AgentsSpawnTrans = Enviorment.Find("Agents");
            // 所有干员放入Operator列表
            // TODO: 暂时使用在sence中放置的初始化方式
            Operators = new Dictionary<Transform, CombatOperator>();
            TestData.AddTestData(Operators, PlayerTeamTrans, EnemyTeamTrans);

        }
        private void FixedUpdate()
        {
            ForOperatorsLogic();
            UpdatePerSecond();
        }

        public float updateInterval = 1f; // 每秒更新的间隔
        private float timer = 0f;
        private void UpdatePerSecond()
        {
            timer += Time.deltaTime;

            if (timer >= updateInterval)
            {
                // do logic
                RecoverShield();

                timer -= updateInterval; // 重置计时器
            }
        }


        // *********** NPC logic ************
        public enum TeammateStatus
        {
            Follow,
            Forward,
            StandBy
        }
        public TextMeshProUGUI TeammateText;
        [HideInInspector]
        public TeammateStatus TeammateStatu;
        public void ChangeTeammateStatu()
        {
            TeammateStatu = (TeammateStatus)(((int)TeammateStatu + 1) % 3);
            TeammateText.text = TeammateStatu.ToString();
        }


        // ************ Game logic ******************
        public bool IsPlayer(Transform transform)
        {
            return transform == PlayerTeamTrans[0];
        }

        public void DellDamage(Transform from, Transform to, int val)
        {
            if (!Operators.ContainsKey(to))
            {
                Debug.Log("aim a unexist target!");
                return;
            }

            // Process DMG
            Operators[to].TakeDamage(val);
            AnimeHelper.Instance.DamageTextEffect(val, to);
            if (Operators[to].CurrentHP <= 0) OperatorDied(to);
            else OperatorGotDMG(to);

        }

        

        private void OperatorDied(Transform aim)
        {
            Operators[aim].DoDied();
            if (Operators[aim].Team == 1) StorageManager.Instance.AddObject("win");
            if (Operators[aim].Team == 0) StorageManager.Instance.AddObject("loss");
            aim.GetComponent<DestructiblePersonController>().DoDied();
            if (aim == PlayerTrans)
            {
                PlayerDiedEvent.Invoke(transform, true);
                UIManager.Instance.ShowReviveCountdown();
            }

            aim.gameObject.SetActive(false);
            AnimeHelper.Instance.ApplyRagdoll(aim);
            //Operators.Remove(aim);
        }
        private void OperatorGotDMG(Transform aim)
        {
            aim.GetComponent<DestructiblePersonController>().GotDMG();
        }
        public int GetOperatorMaxHP(Transform aim)
        {
            return Operators[aim].MaxHP;
        }
        public int GetOperatorCurrentHP(Transform aim)
        {
            return Operators[aim].CurrentHP;
        }

        /// <summary>
        /// 如果技能正在冷却中返回false；否则进入cd，并返回true
        /// </summary>
        public bool UseSkill(Transform op, int index, Vector3 aim, float time)
        {
            if (Operators[op].CombatSkillList[index].IsCoolDowning(time)) return false;
            Operators[op].ActAttack();
            Operators[op].CombatSkillList[index].CoolDownEndTime = time + Operators[op].CombatSkillList[index].SkillInfo.CoolDown;
            _skillContext.CastSkill(op, Operators[op].CombatSkillList[index].SkillInfo, aim);
            return true;
        }

        public void RecoverShield()
        {
            foreach(var p in Operators.Values)
            {
                p.TryRecover();
            }
        }
        public void ForOperatorsLogic()
        {
            foreach (var pair in Operators)
            {
                if (pair.Value.TryRevive())
                {
                    Respawn(pair.Key);
                }
            }
        }
        public void Respawn(Transform trans)
        {
            Operators[trans].Respawn();
            if(trans == PlayerTrans) PlayerDiedEvent.Invoke(transform, false);

            trans.gameObject.SetActive(true);
            trans.position = Operators[trans].SpawnBase.position;
        }

        // ********************* Level logic *********************

        public Transform GenerateAgent(GameObject agnetPrefab, Vector3 pos, Vector3 angle, int Team, Operator OpInfo, Transform spawnBase)
        {
            if(Team == 1)
            {
                var go = Instantiate(agnetPrefab, AgentsSpawnTrans);
                go.transform.position = pos;
                go.transform.eulerAngles = angle;
                EnemyTeamTrans.Add(go.transform);
                Operators.Add(go.transform, new CombatOperator(OpInfo, Team, spawnBase));
                return go.transform;
            }
            else if(Team == 0)
            {
                var go = Instantiate(agnetPrefab, AgentsSpawnTrans);
                go.transform.position = pos;
                go.transform.eulerAngles = angle;
                PlayerTeamTrans.Add(go.transform);
                Operators.Add(go.transform, new CombatOperator(OpInfo, Team, spawnBase));
                return go.transform;
            }
            else
            {
                Debug.LogWarning("can not match this team");
                return null;
            }
        }

        // ********************* UI logic *********************

        /// <summary>
        /// 获得UI显示用的冷却比值
        /// </summary>
        public float GetCoolDownRatio(int index, float time)
        {
            if (Operators[PlayerTrans].CombatSkillList[index].IsCoolDowning(time))
            {
                return (Operators[PlayerTrans].CombatSkillList[index].CoolDownEndTime - time) / Operators[PlayerTrans].CombatSkillList[index].SkillInfo.CoolDown;
            }
            else
            {
                return 0f;
            }
        }

        public bool IsPlayerNoShield()
        {
            return (float)Operators[PlayerTrans].CurrentHP / Operators[PlayerTrans].MaxHP < 0.5;
        }
        public void GameFinish()
        {
            Time.timeScale = 0;
        }
        
        public void RestartScene()
        {
            SceneManager.LoadScene("Playground", LoadSceneMode.Single);
        }

        // *********** Player Logic *****************
        public delegate void PlayerDiedEventHandler(object sender, bool isDied);
        public event PlayerDiedEventHandler PlayerDiedEvent;
    }
}
