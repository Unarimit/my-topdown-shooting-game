using Assets.Scripts.Common;
using Assets.Scripts.Common.EscMenu;
using Assets.Scripts.Entities;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.HomeLogic
{
    internal class HomeContextManager : MonoBehaviour
    {
        public static HomeContextManager Instance;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
            Time.timeScale = 1;
        }

        public List<LevelRule> GetLevelRules()
        {
            return TestDB.LevelRules;
        }

        public void GoToLevel(LevelRule rule)
        {
            var level = new LevelInfo();
            level.LevelRule = rule;
            level.EnemySpawn = new RectInt(25, 25, 5, 5); // TODO：完善出生点指示
            level.TeamSpawn = new RectInt(5, 5, 5, 5);
            level.Map = MapGenerator.RandomMap(rule.MapSize);
            level.EnemyOperators = new List<Operator>();
            foreach(var ops in rule.OperatorPrefabs)
            {
                for(int i = 0; i < Random.Range(ops.MinAmount, ops.MaxAmount+1); i++)
                {
                    level.EnemyOperators.Add((Operator)ops.OpInfo.Clone());
                    if (ops.UseRandomCModel)
                    {
                        level.EnemyOperators[^1].ModelResourceUrl = TestDB.GetRandomModelUrl();
                    }
                }
            }
            TestDB.Level = level;
            SceneManager.LoadScene("Prepare");
        }

        private bool isEscMenu = false;
        public void OnEscMenu(InputValue value)
        {
            if (isEscMenu) return;
            isEscMenu = true;
            var ui = EscMenuUI.OpenEscMenuUI();
            ui.ReturnBtn.onClick.AddListener(() =>
            {
                isEscMenu = false;
            });
        }
    }
}
