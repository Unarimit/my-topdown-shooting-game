using Assets.Scripts.BulletLogic;
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
        public List<Transform> PlayerTeamTrans;

        public List<Transform> EnemyTeamTrans;

        public static CombatContextManager Instance;

        /// <summary>
        /// 所有干员列表，在start初始化
        /// </summary>
        private Dictionary<Transform, CombatOperator> Operators;

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
            foreach(var x in PlayerTeamTrans)
            {
                Operators.Add(x, new CombatOperator { HP = 10, Team = 0 });
                Operators[x].CurrentHP = Operators[x].HP;
            }
            foreach(var x in EnemyTeamTrans)
            {
                Operators.Add(x, new CombatOperator { HP = 5, Team = 1 });
                Operators[x].CurrentHP = Operators[x].HP;
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
            Operators[to].CurrentHP -= val;
            if (Operators[to].CurrentHP <= 0) OperatorDied(to);
            else OperatorGotDMG(to);

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
    }
}
