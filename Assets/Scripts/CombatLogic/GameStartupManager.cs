using Assets.Scripts.CombatLogic.EnviormentLogic;
using Assets.Scripts.CombatLogic.GOAPs;
using Assets.Scripts.CombatLogic.LevelLogic;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Test;
using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.Entities.Level;
using Assets.Scripts.HomeLogic.Environment;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.CombatLogic
{
    internal class GameStartupManager : MonoBehaviour
    {
        [SerializeField]
        LightManager lightManager;
        CombatContextManager _context => CombatContextManager.Instance;

        public void Start()
        {

            transform.GetComponent<SkillManager>().Init();

            CombatLevelInfo level;
            if (MyServices.Database.CurCombatLevelInfo == null 
                || MyServices.Database.CurCombatLevelInfo.TeamOperators == null 
                || MyServices.Database.CurCombatLevelInfo.TeamOperators.Count == 0)
            {   // 调试生成，第二个判断是因为可能进入了prepare页面，但没有选择角色，保存了不完整的生成信息
                Debug.Log("DB has no level info, enter test mode");
                level = CombatLevelGenerator.GeneratorLevelInfo((CombatLevelRule)MyServices.Database.LevelRules[0]);
                level.TeamOperators = MyServices.Database.Operators.Take(5).ToList();
            }
            else
            {
                level = MyServices.Database.CurCombatLevelInfo;
            }
            _context.CombatVM.Level = level;
            _context.CombatVM.LevelResult = new CombatLevelResult { CombatStatu = CombatStatu.Ing, 
                LevelRule = level.LevelRule, 
                JoinOperator = level.TeamOperators };

            //level.TeamOperators[0].McBody.HP = 100;

            prepareLevel(level);

            // AI
            GetComponent<GOAPManager>().Inject(_context);

            // 组件注册
            transform.AddComponent<AnimeHelper>();
            transform.AddComponent<GameLevelManager>().Init(level.LevelRule);

            UIManager.Instance.Init();

            if(Random.Range(0, 2) == 0) lightManager.Day();
            else lightManager.Night();

        }
        [MyTest]
        public void TestInvasion()
        {
            MyServices.Database.CurCombatLevelInfo = CombatLevelGenerator.GeneratorLevelInfo(MyServices.Database.GetInvasionLevel());
            MyServices.Database.CurCombatLevelInfo.TeamOperators = MyServices.Database.Operators.Take(5).ToList();
            SceneLoadHelper.MyLoadSceneAsync("Playground");
        }

        /// <summary>
        /// 根据关卡信息放置地形和人物
        /// </summary>
        private void prepareLevel(CombatLevelInfo level)
        {
            // terrain
            _context.GenerateTerrain(level.Map);

            // team
            {
                var ops = level.TeamOperators;
                var spawn = level.LevelRule.TeamSpawn;

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
                var ops = level.EnemyOperators;
                var spawn = level.LevelRule.EnemySpawn;

                var spawnTrans = new GameObject().transform;
                spawnTrans.parent = transform;
                spawnTrans.gameObject.name = "EnemySpawnGO";

                spawnTrans.position = new Vector3(spawn.x + Random.Range(0, spawn.width), 0, spawn.y + Random.Range(0, spawn.height));

                for (int i = 0; i < ops.Count; i++) // 位置
                {
                    _context.GenerateAgent(ops[i], GetPosByInitMethod(level, level.EnemyOperatorsBy[i].InitPosition), Vector3.zero, 1, spawnTrans);
                }
            }

            // interactable object
            if(level.LevelRule.InteractablePrefabs != null)
            {
                foreach (var inter in level.LevelRule.InteractablePrefabs)
                {
                    int len = Random.Range(inter.MinAmount, inter.MaxAmount);
                    for (int _ = 0; _ < len; _++)
                    {
                        var trans = InteractableObjectController.CreateInteractableObject(inter);
                        var p = ResourceManager.Load<GameObject>(inter.ModelUrl);
                        Instantiate(p, trans);
                        trans.position = GetPosByInitMethod(level, inter.InitPosition);
                    }
                }
            }

            // place building
            if(level.LevelRule.AllowHomeBuilding is true)
            {
                var bDic = MyServices.Database.Buildings;
                foreach (var place in MyServices.Database.BuildingArea.PlaceInfos)
                {
                    if(place.AreaIndex == PlaceInfo.BattleIndex)
                    {
                        //if (bDic[place.BuildingId])
                        _context.GenerateBuilding((CombatBuilding)bDic[place.BuildingId], place.PlacePosition, 0);
                    }
                }
            }

            

            //TODO: bake navmap
            var nm = transform.Find("NavMesh Surface").GetComponent<NavMeshSurface>();
            nm.UpdateNavMesh(nm.navMeshData);
        }


        
        private Vector3 GetPosByInitMethod(CombatLevelInfo level, InitPosition method)
        {
            if(method == InitPosition.EnemySpawnCenter)
            {
                return new Vector3(level.LevelRule.EnemySpawn.center.x, 0, level.LevelRule.EnemySpawn.center.y);
            }
            else if(method == InitPosition.EnemySpawnScatter)
            {
                var spawn = level.LevelRule.EnemySpawn;
                return new Vector3(spawn.x + Random.Range(0, spawn.width), 0, spawn.y + Random.Range(0, spawn.height));
            }
            else if(method == InitPosition.MapScatter)
            {
                return new Vector3(Random.Range(0, level.Map.Length), 0, Random.Range(0, level.Map[0].Length));
            }
            else
            {
                Debug.LogWarning("cannot match a init type");
                return Vector3.zero;
            }
        }

    }
}
