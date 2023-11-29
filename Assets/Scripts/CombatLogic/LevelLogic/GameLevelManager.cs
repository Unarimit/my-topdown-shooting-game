using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.LevelLogic
{
    public class GameLevelManager : MonoBehaviour
    {
        public static GameLevelManager Instance;
        /// <summary>
        /// 敌方索敌系数
        /// </summary>
        public float EnemyAttackFactor { get; private set; }
        private LevelRule _rule;
        /// <summary>
        /// 掉落
        /// </summary>
        private Dictionary<string, int> Dropouts = new Dictionary<string, int>();
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }
        public void Init(LevelRule rule)
        {
            _rule = rule;
        }
        private void Start()
        {
            CheckAimAndAction();
        }

        /// <summary>
        /// 超时逻辑
        /// </summary>
        private float _time;
        private void Update()
        {
            _time += Time.deltaTime;
            if(_time > 1)
            {
                _time -= 1;
                addDropout(TestDB.DropoutTable.Time.ToString(), 1);
            }
        }

        public bool IsEnemyCanReact()
        {
            return EnemyAttackFactor >= _rule.EnemyAttackThreshold;
        }



        #region 掉落和检测相关
        public void FinishInteract(InteractablePrefab interactable)
        {
            foreach(var pair in interactable.Dropouts)
            {
                addDropout(pair.Key, pair.Value);
            }
        }
        public void CalculateDropout(CombatOperator cOperator)
        {
            if (cOperator.Team == 0) addDropout(TestDB.DropoutTable.KillTeam.ToString(), 1);
            else if (cOperator.Team == 1)
            {
                addDropout(TestDB.DropoutTable.KillEnemy.ToString(), 1);
                EnemyAttackFactor += 0.2f;
            }
        }
        private void addDropout(string key, int value)
        {
            if (!Dropouts.ContainsKey(key)) Dropouts.Add(key, 0);
            Dropouts[key] += value;
            CheckAimAndAction();
        }
        private int getDropout(string key)
        {
            if (Dropouts.ContainsKey(key)) return Dropouts[key];
            else return 0;
        }
        public delegate void AimChangeEventHandler(string text);
        public event AimChangeEventHandler AimChangeEvent;
        public void CheckAimAndAction()
        {

            if (isMatchWinCondition())
            {
                UIManager.Instance.ShowFinish(true);
                CombatContextManager.Instance.GameFinish();
            }
            if (isMatchLossCondition())  
            {
                UIManager.Instance.ShowFinish(false);
                CombatContextManager.Instance.GameFinish();
            }
            
            AimChangeEvent.Invoke(generateText());

        }

        private bool isMatchWinCondition()
        {
            foreach (var x in _rule.WinCondition)
            {
                if(x.Amount > getDropout(x.ItemName)) return false;
            }
            return true;
        }
        public bool isMatchLossCondition()
        {
            foreach (var x in _rule.LossCondition)
            {
                if (x.Amount > getDropout(x.ItemName)) return false;
            }
            return true;
        }
        private string generateText()
        {
            var sb = new StringBuilder();
            sb.Append("胜利条件\n");
            foreach (var x in _rule.WinCondition)
            {
                sb.Append(string.Format(x.Description, $"{getDropout(x.ItemName)}/{x.Amount}"));
                sb.Append("\n");
            }
            sb.Append("失败条件\n");
            foreach (var x in _rule.LossCondition)
            {
                sb.Append(string.Format(x.Description, $"{getDropout(x.ItemName)}/{x.Amount}"));
                sb.Append("\n");
            }
            return sb.ToString();
        }
        #endregion
    }
}
