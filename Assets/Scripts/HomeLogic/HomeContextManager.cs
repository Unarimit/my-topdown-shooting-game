using Assets.Scripts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
        }

        public void GoToLevel()
        {
            // TODO: this just for test
            TestDB.Level = new Entities.LevelInfo
            {
                Map = MapGenerator.RandomMiddleMap(),
                EnemyOperators = TestDB.GetRandomOperator(6),
                EnemySpawn = new RectInt(25, 25, 5, 5),
                TeamSpawn = new RectInt(5, 5, 5, 5),
                LevelName = "演习作战",
                WinDesc = "击杀敌方10次",
                LossDesc = "死亡6次"
            };

            SceneManager.LoadScene("Prepare");
        }
    }
}
