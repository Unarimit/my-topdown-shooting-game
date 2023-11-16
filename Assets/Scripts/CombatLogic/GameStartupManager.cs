using Assets.Scripts.CombatLogic.EnviormentLogic;
using Assets.Scripts.CombatLogic.LevelLogic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    internal class GameStartupManager : MonoBehaviour
    {
        CombatContextManager _context => CombatContextManager.Instance;
        public void Start()
        {
            // 地形生成
            _context.GenerateTerrain(TerrainGenerator.RandomTerrain());
            //TODO: bake navmap

            // 我方生成（单位，生成规则）
            var ops = TestData.GetTeamUnit();
            Vector3 init = new Vector3(Random.Range(5, 25), 0, Random.Range(5, 25));
            _context.GeneratePlayer(ops[0], init, Vector3.zero, transform);

            for(int i = 1; i < ops.Count; i++)
            {
                _context.GenerateAgent(ops[i], init, Vector3.zero, 0, transform);
            }


            // 敌方生成（单位，生成规则）
            Vector3 einit = new Vector3(Random.Range(37, 57), 0, Random.Range(37, 57));


            // 组件注册
            transform.AddComponent<StorageManager>();
            transform.AddComponent<GameLevelManager>();
            transform.AddComponent<AnimeHelper>();

            UIManager.Instance.Init();
        }

    }
}
