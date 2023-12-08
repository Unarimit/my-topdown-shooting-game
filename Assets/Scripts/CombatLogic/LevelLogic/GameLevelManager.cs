using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.CombatLogic.ContextExtends;
using Assets.Scripts.CombatLogic.UILogic.CombatSummaryUIs;
using Assets.Scripts.Entities;
using Assets.Scripts.Services;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.LevelLogic
{
    public class GameLevelManager : MonoBehaviour
    {
        public static GameLevelManager Instance;
        private CombatContextManager _context => CombatContextManager.Instance;
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
                addDropout(MyConfig.DropoutTable.Time.ToString(), 1);
            }
        }

        public bool IsEnemyCanReact()
        {
            return EnemyAttackFactor >= _rule.EnemyAttackThreshold;
        }

        public void TestFunction()
        {
            levelAccomplish(true);
        }


        private bool isAccomplish = false;
        /// <summary>
        /// 关卡目标达成
        /// </summary>
        private void levelAccomplish(bool isWin)
        {
            if (isAccomplish is true) return;
            else isAccomplish = true;

            // 0. stop game
            _context.ActiveAllCharacter(false);

            // 1. get player operators and sort
            var cops = _context.FindCombatOperators(x => x.Team == 0);
            cops.Sort((x, y) => { return (y.StatCauseDamage * 2 + y.StatReceiveDamage) - (x.StatCauseDamage * 2 + x.StatReceiveDamage); });

            // 2. transform camera to mvp or svp
            //// tip: 角色死亡不重要，反正都是原地替换
            var trans = _context.GetTransformByCop(cops[0]);
            trans.gameObject.SetActive(false);
            _context.GenerateMvpDisplayer(cops[0].OpInfo, trans.position, trans.eulerAngles);

            // 3. call ui
            UIManager.Instance.TweenQuit();
            CombatSummaryCanvasUI.CreateAndShowCombatSummaryCanvasUI(cops, isWin, Dropouts);
        }
        #region 掉落和检测相关
        public void FinishInteract(InteractablePrefab interactable)
        {
            foreach(var x in interactable.Dropouts)
            {
                addDropout(x.DropItem.ItemId, x.GetDropoutAmount());
            }
        }
        public void CalculateDropout(CombatOperator cOperator)
        {
            if (cOperator.Team == 0) addDropout(MyConfig.DropoutTable.KillTeam.ToString(), 1);
            else if (cOperator.Team == 1)
            {
                // 自带掉落
                foreach(var ePrab in _rule.OperatorPrefabs)
                {
                    if(ePrab.OpInfo.Id == cOperator.OpInfo.Id)
                    {
                        if(ePrab.Dropouts != null)
                        {
                            foreach (var dp in ePrab.Dropouts)
                            {
                                addDropout(dp.DropItem.ItemId, dp.GetDropoutAmount());
                            }
                        }
                        break;
                    }
                }
                // 敌人指示物
                addDropout(MyConfig.DropoutTable.KillEnemy.ToString(), 1);
                // AI相关
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
            if (isAccomplish is true) return;

            if(AimChangeEvent != null) AimChangeEvent.Invoke(generateText());

            // 最后执行物体控制指令（
            if (isMatchWinCondition())
            {
                levelAccomplish(true);
            }
            if (isMatchLossCondition())
            {
                levelAccomplish(false);
            }

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
