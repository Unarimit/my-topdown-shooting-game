using Assets.Scripts.BulletLogic;
using Assets.Scripts.CombatLogic.Characters;
using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using Assets.Scripts.CombatLogic.Characters.Computer.Fighter;
using Assets.Scripts.CombatLogic.Characters.Player;
using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.CombatLogic.ContextExtends;
using Assets.Scripts.CombatLogic.LevelLogic;
using Assets.Scripts.Common;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.Entities.Level;
using Assets.Scripts.Services;
using Cinemachine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.CombatLogic
{

    public class CombatContextManager : MonoBehaviour
    {
        /// <summary>
        /// singleton
        /// </summary>
        public static CombatContextManager Instance;
        // ******************* inspector *************
        /// <summary>
        /// 己方干员列表，0号为玩家位
        /// </summary>
        public List<Transform> PlayerTeamTrans { get; private set; }

        public List<Transform> EnemyTeamTrans { get; private set; }

        public List<Transform> PlayerTeamFighterTrans { get; private set; }

        public List<Transform> EnemyTeamFighterTrans { get; private set; }

        public CinemachineVirtualCamera m_Camera;

        // ******************* end inspector *************

        /// <summary>
        /// 所有干员列表，在start初始化
        /// </summary>
        public Dictionary<Transform, CombatOperator> Operators { get; private set; }
        internal Dictionary<Transform, FighterController> Fighters { get; private set; } = new Dictionary<Transform, FighterController>();

        public Transform PlayerTrans => CombatVM.PlayerTrans;

        public ViewModel CombatVM { get; private set; }

        private SkillManager _skillContext;
        private Transform _agentsSpawnTrans;
        [HideInInspector]
        public Transform Enviorment;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
            TeammateStatu = TeammateStatus.Follow;

            // init
            Time.timeScale = 1;
            PlayerTeamTrans = new List<Transform>();
            EnemyTeamTrans = new List<Transform>();
            PlayerTeamFighterTrans = new List<Transform>();
            EnemyTeamFighterTrans = new List<Transform>();
            Operators = new Dictionary<Transform, CombatOperator>();
            CombatVM = new ViewModel();
            _agentsSpawnTrans = transform.Find("Agents");
            Enviorment = transform.Find("Effects");

        }

        private void Start()
        {
            _skillContext = SkillManager.Instance;

            // 队员跟随状态显示
            TeammateText.text = TeammateStatu.ToString();
        }
        private void FixedUpdate()
        {
            ForOperatorsLogic();
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


        // ************ Game logic ******************
        public void DellDamage(Transform from, Transform to, int val)
        {
            if(to.gameObject.layer == MyConfig.CHARACTER_LAYER)
            {
                // Process DMG
                val = Operators[to].TakeDamage(val);
                // statistic
                if (Operators.ContainsKey(from)) Operators[from].ActAttack(val);
                else if (Fighters.ContainsKey(from)) Operators[Fighters[from].CvBase].ActAttack(val);
                else Debug.LogWarning($"{from.gameObject.name}: can not be search in combat dic");
                
                // effect
                AnimeHelper.Instance.DamageTextEffect(val, to);
                if (Operators[to].CurrentHP <= 0) OperatorDied(to);
                else OperatorGotDMG(to);
            }
            else if(to.gameObject.layer == MyConfig.DOBJECT_LAYER)
            {
                to.GetComponent<DestructibleObjectController>().GotDMG(val);
            }
            else
            {
                Debug.Log("aim a unexist target!");
            }

        }

        

        private void OperatorDied(Transform aim)
        {
            Operators[aim].DoDied();
            GameLevelManager.Instance.CalculateDropout(Operators[aim]);
            aim.GetComponent<DestructiblePersonController>().DoDied();
            if (aim == PlayerTrans)
            {
                PlayerDiedEvent.Invoke(transform, true);
                UIManager.Instance.ShowReviveCountdown();
            }

            aim.gameObject.SetActive(false);
            AnimeHelper.Instance.ApplyRagdoll(aim);
            //Operators.Remove(aim);
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
        public bool UseSkill(Transform op, CombatCombatSkill skill, Vector3 aim)
        {
            return UseSkill(op, skill, aim, op.position + new Vector3(0, 0.5f, 0), op.eulerAngles);
        }
        public bool UseSkill(Transform op, CombatCombatSkill skill, Vector3 aim, Vector3 startPos, Vector3 startAngle)
        {
            if (skill.IsCoolDowning(Time.time)) return false;
            skill.CoolDownEndTime = Time.time + skill.SkillInfo.CoolDown;
            _skillContext.CastSkill(op, skill.SkillInfo, aim, startPos, startAngle);
            return true;
        }

        public void RecoverShield()
        {
            foreach(var p in Operators.Values)
            {
                p.TryRecover();
            }
        }
        public void ForOperatorsLogic()
        {
            foreach (var pair in Operators)
            {
                if (pair.Value.TryRevive())
                {
                    Respawn(pair.Key);
                }
            }
        }
        public void Respawn(Transform trans)
        {
            Operators[trans].Respawn();
            if(trans == PlayerTrans) PlayerDiedEvent.Invoke(transform, false);

            trans.gameObject.SetActive(true);
            trans.position = Operators[trans].SpawnBase.position;
        }

        // ********************* Level logic *********************

        public Transform GenerateAgent(Operator OpInfo, Vector3 pos, Vector3 angle, int team, Transform spawnBase)
        {
            // 初始化
            var model = new CombatOperator(OpInfo, team, spawnBase, false);

            var trans = GenerateCharacter(model, pos, angle, spawnBase);
            trans.AddComponent<AgentController>();

            // 设置队伍并放置到场景
            if (team == 1)
            {
                EnemyTeamTrans.Add(trans);
                return trans;
            }
            else if(team == 0)
            {
                PlayerTeamTrans.Add(trans);
                return trans;
            }
            else
            {
                Debug.LogWarning("can not match this team");
                return null;
            } 
        }

        public Transform GeneratePlayer(Operator OpInfo, Vector3 pos, Vector3 angle, Transform spawnBase)
        {
            var pInfo = new CombatOperator(OpInfo, 0, spawnBase, true);

            var trans = GenerateCharacter(pInfo, pos, angle, spawnBase);
            trans.AddComponent<PlayerController>();

            // 还有驾驶舱
            CockpitManager.Instance.ResetAnimator();
            GetComponent<FbxLoadManager>().LoadModel(OpInfo.ModelResourceUrl, CockpitManager.Instance.CharacterAnimator.transform, false);

            PlayerTeamTrans.Add(trans);

            return trans;
        }

        public Transform GenerateCharacter(CombatOperator cop, Vector3 pos, Vector3 angle, Transform spawnBase)
        {

            // 初始化
            var prefab = ResourceManager.Load<GameObject>("Characters/Character");
            var go = Instantiate(prefab, _agentsSpawnTrans);

            Operators.Add(go.transform, cop);

            // 挂components
            // animator component
            var res = GetComponent<FbxLoadManager>().LoadModel(cop.OpInfo.ModelResourceUrl, go.transform.Find("ModelRoot"), true);
            // gun component
            {
                var gun = go.GetComponent<GunController>();
                gun.GunFire = res.GunfireEffect;
                gun.BulletStartTrans = res.GunfireTransform;
            }
            // op component
            go.GetComponent<OperatorController>().Inject(cop);

            // 确认位置
            go.transform.position = pos;
            go.transform.eulerAngles = angle;

            return go.transform;
        }

        public void SwithAgentWithPlayer(Transform agent)
        {
            if (Operators[agent].Team != 0) return;
            if (Operators[agent].IsPlayer is true) return;

            var oldOpTrans = CombatVM.PlayerTrans;
            Operators[agent].IsPlayer = true;
            Operators[oldOpTrans].IsPlayer = false;

            Destroy(oldOpTrans.transform.GetComponent<PlayerController>());
            Destroy(agent.transform.GetComponent<AgentController>());
            agent.transform.AddComponent<PlayerController>();
            oldOpTrans.transform.AddComponent<AgentController>();

            agent.GetComponent<OperatorController>().RefreshPlayerUIRef();

            // 还有驾驶舱
            CockpitManager.Instance.ResetAnimator();
            GetComponent<FbxLoadManager>().LoadModel(Operators[agent].OpInfo.ModelResourceUrl, CockpitManager.Instance.CharacterAnimator.transform, false);

            TransformHelper.ActiveCharacter(this, agent, false);
            TransformHelper.ActiveCharacter(this, oldOpTrans, false);
        }

        public Transform GenerateFighter(Fighter fighter, Vector3 pos, Vector3 angle, int Team, Transform cvBase)
        {
            // 初始化
            var prefab = ResourceManager.Load<GameObject>("Characters/Fighter");
            var go = Instantiate(prefab, _agentsSpawnTrans);
            
            // 挂components
            // animator component
            GetComponent<FbxLoadManager>().LoadModel(fighter.Operator.ModelResourceUrl, go.transform.Find("modelroot"), go.transform, false);

            // Add FighterController, set cvBase and add to dictionary
            {
                var con = go.GetComponent<FighterController>();
                con.Inject(fighter, cvBase);
                Fighters.Add(go.transform, con);
            }

            go.transform.position = pos;
            go.transform.eulerAngles = angle;

            // 设置队伍并放置到场景
            if (Team == 1)
            {
                EnemyTeamFighterTrans.Add(go.transform);
                return go.transform;
            }
            else if (Team == 0)
            {
                PlayerTeamFighterTrans.Add(go.transform);
                return go.transform;
            }
            else
            {
                Debug.LogWarning("can not match this team");
                return null;
            }
        }

        internal Transform GenerateBuilding(CombatBuilding building, Vector2Int pos, int Team)
        {
            // 加载包装模型
            var prefab = ResourceManager.Load<GameObject>("Characters/BuildingBox");
            var go = Instantiate(prefab, _agentsSpawnTrans);
            // 加载建筑模型
            Instantiate(ResourceManager.Load<GameObject>($"Buildings/{building.ModelUrl}"), go.transform);
            // 根据建筑属性配置组件
            // -- 大小位置
            go.GetComponent<BoxCollider>().size = new Vector3(building.Dimensions.y, 2, building.Dimensions.x);
            go.GetComponent<BoxCollider>().center = new Vector3(0, 1, 0);
            go.transform.position = new Vector3(pos.x * 2 + building.Dimensions.x, 0, pos.y * 2 + building.Dimensions.y);
            go.transform.eulerAngles = new Vector3(0, -90, 0);
            // -- 生命
            go.GetComponent<DestructibleObjectController>().HP = building.Hp;
            // -- 小地图颜色
            if (Team == 0) go.GetComponent<bl_MiniMapEntity>().IconColor = MyConfig.TeamColor;
            else go.GetComponent<bl_MiniMapEntity>().IconColor = MyConfig.EnemyColor;
            // -- TODO: AI控制

            return go.transform;
        }

        public Transform CreateGO(GameObject prefab, Transform parent)
        {
            var go = Instantiate(prefab, parent);

            return go.transform;

        }

        public Transform GenerateMvpDisplayer(Operator opInfo, Vector3 pos, Vector3 angle)
        {
            var prefab = ResourceManager.Load<GameObject>("Characters/MvpDisplayer");
            var go = Instantiate(prefab, _agentsSpawnTrans);
            var modelRoot = go.transform.Find("modelroot");
            GetComponent<FbxLoadManager>().LoadModel(opInfo.ModelResourceUrl, modelRoot, modelRoot, false);
            go.transform.position = pos;
            modelRoot.eulerAngles = angle;

            return go.transform;
        }


        public void QuitScene()
        {
            MyServices.GameDataHelper.FinishLevel(CombatVM.LevelResult);
            SceneLoadHelper.MyLoadSceneAsync("Home");
        }

        // *********** Player Logic *****************
        public delegate void PlayerDiedEventHandler(object sender, bool isDied);
        public event PlayerDiedEventHandler PlayerDiedEvent;

        public class ViewModel {
            public bool IsPlayerAimming { 
                set { 
                    if(value == _isPlayerAimming) return;
                    _isPlayerAimming = value;
                    IsPlayerAimmingEvent.Invoke(value);
                } 
                get { return _isPlayerAimming; } 
            }
            public delegate void IsPlayerAimmingEventHandler(bool _isPlayerAimming);
            public event IsPlayerAimmingEventHandler IsPlayerAimmingEvent;
            private bool _isPlayerAimming = false;

            /// <summary>
            /// 玩家枪属性
            /// </summary>
            internal delegate void PlayerGunStatuChangeEventHandler(GunController gun);
            internal event PlayerGunStatuChangeEventHandler PlayerGunStatuChangeEvent;
            internal GunController InvokePlayerGunStatuChangeEventLastMsg = null;
            internal void InvokePlayerGunStatuChangeEvent(GunController gun)
            {
                if (PlayerGunStatuChangeEvent != null) PlayerGunStatuChangeEvent.Invoke(gun);
                else InvokePlayerGunStatuChangeEventLastMsg = gun;
            }

            /// <summary>
            /// 玩家坐标
            /// </summary>
            public Transform PlayerTrans { 
                get { return _playerTrans; }
                set {
                    _playerTrans = value;
                    if(PlayerChangeEvent != null) PlayerChangeEvent.Invoke();
                } }
            private Transform _playerTrans;
            public delegate void PlayerChangeEventHandler();
            public event PlayerChangeEventHandler PlayerChangeEvent;

            /// <summary>
            /// 玩家属性
            /// </summary>
            public CombatOperator Player => Instance.Operators[_playerTrans];
            public CombatLevelInfo Level { get; set; }

            public CombatLevelResult LevelResult { get; set; }
        }

    }
}
