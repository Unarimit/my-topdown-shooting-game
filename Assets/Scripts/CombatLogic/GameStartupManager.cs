using Assets.Scripts.CombatLogic.EnviormentLogic;
using Assets.Scripts.CombatLogic.LevelLogic;
using Assets.Scripts.Common;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    internal class GameStartupManager : MonoBehaviour
    {
        CombatContextManager _context => CombatContextManager.Instance;
        public void Start()
        {

            transform.GetComponent<SkillManager>().Init();

            PrepareGameScene();

            // 组件注册
            transform.AddComponent<StorageManager>();
            transform.AddComponent<GameLevelManager>();
            transform.AddComponent<AnimeHelper>();

            UIManager.Instance.Init();
        }

        /// <summary>
        /// 根据信息放置地形和人物
        /// </summary>
        private void PrepareGameScene()
        {

            if (TestDB.Level == null || TestDB.Level.TeamOperators == null || TestDB.Level.TeamOperators.Count == 0) // 调试生成
            {
                Debug.Log("DB has no level info, enter test mode");
                _context.GenerateTerrain(MapGenerator.RandomMap());
                // 我方生成
                var ops = TestDB.GetRandomOperator(5);
                ops[0].WeaponSkillId = 3;
                ops[0].McBody.HP = 100;
                ops[1] = TestDB.GetRandomCV();

                Vector3 init = new Vector3(Random.Range(5, 15), 0, Random.Range(5, 15));
                _context.GeneratePlayer(ops[0], init, Vector3.zero, transform);
                for (int i = 1; i < ops.Count; i++)
                {
                    _context.GenerateAgent(ops[i], init, Vector3.zero, 0, transform);
                }
                // 敌方生成
                Vector3 einit = new Vector3(Random.Range(37, 57), 0, Random.Range(37, 57));
            }
            else
            {
                // terrain
                _context.GenerateTerrain(TestDB.Level.Map);

                // team
                {
                    var ops = TestDB.Level.TeamOperators;
                    var spawn = TestDB.Level.TeamSpawn;

                    var spawnTrans = new GameObject().transform;
                    spawnTrans.parent = transform;
                    spawnTrans.gameObject.name = "TeamSpawnGO";

                    spawnTrans.position = new Vector3(spawn.x + Random.Range(0, spawn.width), 0, spawn.y + Random.Range(0, spawn.height));

                    for (int i = 0; i < ops.Count; i++)
                    {
                        var v3 = new Vector3(spawn.x + Random.Range(0, spawn.width), 0, spawn.y + Random.Range(0, spawn.height));
                        if (i == 0) _context.GeneratePlayer(ops[i], v3, Vector3.zero, spawnTrans);
                        else _context.GenerateAgent(ops[i], v3, Vector3.zero, 0, spawnTrans);
                    }
                }
                // enemy
                {
                    var ops = TestDB.Level.EnemyOperators;
                    var spawn = TestDB.Level.EnemySpawn;

                    var spawnTrans = new GameObject().transform;
                    spawnTrans.parent = transform;
                    spawnTrans.gameObject.name = "EnemySpawnGO";

                    spawnTrans.position = new Vector3(spawn.x + Random.Range(0, spawn.width), 0, spawn.y + Random.Range(0, spawn.height));

                    for (int i = 0; i < ops.Count; i++)
                    {
                        var v3 = new Vector3(spawn.x + Random.Range(0, spawn.width), 0, spawn.y + Random.Range(0, spawn.height));
                        _context.GenerateAgent(ops[i], v3, Vector3.zero, 1, spawnTrans);
                    }
                }

            }
            //TODO: bake navmap
            var nm = transform.Find("NavMesh Surface").GetComponent<NavMeshSurface>();
            nm.UpdateNavMesh(nm.navMeshData);
        }
    }
}
