using Assets.Scripts.BulletLogic;
using Assets.Scripts.CombatLogic;
using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Assets.Scripts.ComputerControllers
{

    public class CombatContextManager : MonoBehaviour
    {
        // ******************* inspector *************
        /// <summary>
        /// 己方干员列表，0号为玩家位
        /// </summary>
        public List<Transform> PlayerTeamTrans;

        public List<Transform> EnemyTeamTrans;

        public Transform Enviorment;

        // ******************* end inspector *************

        /// <summary>
        /// 所有干员列表，在start初始化
        /// </summary>
        public Dictionary<Transform, CombatOperator> Operators { get; private set; }

        public Transform PlayerTrans => PlayerTeamTrans[0];

        /// <summary>
        /// singleton
        /// </summary>
        public static CombatContextManager Instance;
        private SkillManager _skillContext;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
            TeammateStatu = TeammateStatus.Follow;
        }

        private void Start()
        {
            _skillContext = SkillManager.Instance;

            // 队员跟随状态显示
            TeammateText.text = TeammateStatu.ToString();

            // 所有干员放入Operator列表
            // TODO: 暂时使用在sence中放置的初始化方式
            Operators = new Dictionary<Transform, CombatOperator>();
            TestData.AddTestData(Operators, PlayerTeamTrans, EnemyTeamTrans);

        }
        private void FixedUpdate()
        {
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

        // Engage Channel
        public delegate void OnEnemyEngageHandler(Transform sender, Vector3 poi);
        public event OnEnemyEngageHandler OnEnemyEngageEvent;
        public void EnemyFindCounter(Transform sender, Vector3 poi)
        {
            OnEnemyEngageEvent.Invoke(sender, poi);
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
            DamageTextEffect(val, to);
            if (Operators[to].CurrentHP <= 0) OperatorDied(to);
            else OperatorGotDMG(to);

        }

        

        private void OperatorDied(Transform aim)
        {
            if (Operators[aim].Team == 1) StorageManager.Instance.KillOne();
            aim.GetComponent<DestructiblePersonController>().DoDied();
            Operators.Remove(aim);
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

        // ********************* Level logic *********************

        public Transform GenerateAgent(GameObject agnetPrefab, Vector3 pos, Vector3 angle, int Team, Operator BaseInfo)
        {
            if(Team == 1)
            {
                var go = Instantiate(agnetPrefab, Enviorment);
                go.transform.position = pos;
                go.transform.eulerAngles = angle;
                EnemyTeamTrans.Add(go.transform);
                Operators.Add(go.transform, new CombatOperator(BaseInfo, Team));
                return go.transform;
            }
            else
            {
                Debug.Log("No Team 0 Match");
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
        
        private void DamageTextEffect(int dmg, Transform hitted)
        {
            var prefab = Resources.Load<GameObject>("Effects/DamageTextEffect");
            var go = Instantiate(prefab, Enviorment);
            go.transform.position = hitted.position + new Vector3(0, 1.5f, 0);
            go.GetComponent<DamageNumEffectController>().DamageNum = dmg;
        }
    }
}
