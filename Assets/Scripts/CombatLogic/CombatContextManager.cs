using Assets.Scripts.BulletLogic;
using Assets.Scripts.CombatLogic;
using Assets.Scripts.CombatLogic.CombatEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.ComputerControllers
{

    public class CombatContextManager : MonoBehaviour
    {
        /// <summary>
        /// singleton
        /// </summary>
        public static CombatContextManager Instance;

        /// <summary>
        /// 己方干员列表，0号为玩家位
        /// </summary>
        public List<Transform> PlayerTeamTrans;

        public List<Transform> EnemyTeamTrans;


        /// <summary>
        /// 所有干员列表，在start初始化
        /// </summary>
        private Dictionary<Transform, CombatOperator> Operators;

        public Transform PlayerTrans => PlayerTeamTrans[0];


        private Dictionary<IDelayWeaponController, DelayWeapon> CombatDelayWeapons;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
            TeammateStatu = TeammateStatus.Follow;
        }

        private void Start()
        {
            // 队员跟随状态显示
            TeammateText.text = TeammateStatu.ToString();

            // 所有干员放入Operator列表
            // TODO: 暂时使用在sence中放置的初始化方式
            Operators = new Dictionary<Transform, CombatOperator>();
            CombatDelayWeapons = new Dictionary<IDelayWeaponController, DelayWeapon>();
            TestData.AddTestData(Operators, PlayerTeamTrans, EnemyTeamTrans);

        }
        private void FixedUpdate()
        {
            TriggerDelayWeapon();
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
            Operators[to].CurrentHP -= val;
            if (Operators[to].CurrentHP <= 0) OperatorDied(to);
            else OperatorGotDMG(to);

        }

        public void AddDelayWeapon(IDelayWeaponController controller, DelayWeapon data)
        {
            CombatDelayWeapons.Add(controller, data);
            controller.SetDalayWeaponEntity(data);
        }

        private void TriggerDelayWeapon()
        {
            var keysToRemove = new List<IDelayWeaponController>();
            foreach (var pair in CombatDelayWeapons)
            {
                if(pair.Value.DelayEndTime < Time.time)
                {
                    keysToRemove.Add(pair.Key);
                    pair.Key.DoDelayAction();
                    // TODO: Dell Damage
                }
            }
            foreach(var x in keysToRemove)
            {
                CombatDelayWeapons.Remove(x);
            }

        }

        private void OperatorDied(Transform aim)
        {
            aim.GetComponent<DestructiblePersonController>().DoDied();
        }
        private void OperatorGotDMG(Transform aim)
        {
            aim.GetComponent<DestructiblePersonController>().GotDMG();
        }
        public int GetOperatorMaxHP(Transform aim)
        {
            return Operators[aim].HP;
        }
        public int GetOperatorCurrentHP(Transform aim)
        {
            return Operators[aim].CurrentHP;
        }

        /// <summary>
        /// 如果技能正在冷却中返回false；否则进入cd，并返回true
        /// </summary>
        public bool UseSkill(Transform op ,int index, float time)
        {
            if (Operators[op].CombatSkillList[index].IsCoolDowning(time)) return false;
            Operators[op].CombatSkillList[index].CoolDownEndTime = time + Operators[op].CombatSkillList[index].CoolDown;

            return true;
        }




        // ********************* UI logic *********************

        /// <summary>
        /// 获得UI显示用的冷却比值
        /// </summary>
        public float GetCoolDownRatio(int index, float time)
        {
            if (Operators[PlayerTrans].CombatSkillList[index].IsCoolDowning(time))
            {
                return (Operators[PlayerTrans].CombatSkillList[index].CoolDownEndTime - time) / Operators[PlayerTrans].CombatSkillList[index].CoolDown;
            }
            else
            {
                return 0f;
            }
        }
        
    }
}
