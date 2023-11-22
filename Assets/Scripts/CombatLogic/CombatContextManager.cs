using Assets.Scripts.BulletLogic;
using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.CombatLogic.ComputerControllers;
using Assets.Scripts.CombatLogic.ComputerControllers.Fighter;
using Assets.Scripts.CombatLogic.EnviormentLogic;
using Assets.Scripts.Common.EscMenu;
using Assets.Scripts.Entities;
using Cinemachine;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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

        public Transform PlayerTrans { get; private set; }

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
        public bool IsPlayer(Transform transform)
        {
            return transform == PlayerTeamTrans[0];
        }

        public void DellDamage(Transform from, Transform to, int val)
        {
            if(to.gameObject.layer == TestDB.CHARACTER_LAYER)
            {
                // Process DMG
                Operators[to].TakeDamage(val);
                AnimeHelper.Instance.DamageTextEffect(val, to);
                if (Operators[to].CurrentHP <= 0) OperatorDied(to);
                else OperatorGotDMG(to);
            }
            else if(to.gameObject.layer == TestDB.DOBJECT_LAYER)
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
            if (Operators[aim].Team == 1) StorageManager.Instance.AddObject("win");
            if (Operators[aim].Team == 0) StorageManager.Instance.AddObject("loss");
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

        public Transform GenerateAgent(Operator OpInfo, Vector3 pos, Vector3 angle, int Team, Transform spawnBase)
        {
            // 初始化
            var prefab = ResourceManager.Load<GameObject>("Characters/Agent");
            var go = Instantiate(prefab, _agentsSpawnTrans);

            // 挂components
            // animator component
            var res = GetComponent<FbxLoadManager>().LoadModel(OpInfo.ModelResourceUrl, go.transform, true);
            // gun component
            {
                var gun = go.GetComponent<GunController>();
                gun.GunFire = res.GunfireEffect;
                gun.BulletStartTrans = res.GunfireTransform;
            }

            go.transform.position = pos;
            go.transform.eulerAngles = angle;
            // 设置队伍并放置到场景
            if (Team == 1)
            {
                EnemyTeamTrans.Add(go.transform);
                Operators.Add(go.transform, new CombatOperator(OpInfo, Team, spawnBase));
                return go.transform;
            }
            else if(Team == 0)
            {
                PlayerTeamTrans.Add(go.transform);
                Operators.Add(go.transform, new CombatOperator(OpInfo, Team, spawnBase));
                return go.transform;
            }
            else
            {
                Debug.LogWarning("can not match this team");
                return null;
            } 
        }

        public Transform GeneratePlayer(Operator OpInfo, Vector3 pos, Vector3 angle, Transform spawnBase)
        {
            CombatVM.Player = new CombatOperator(OpInfo, 0, spawnBase);
            // 添加技能
            CombatVM.Player.CombatSkillList.Add(new CombatCombatSkill(SkillManager.Instance.skillConfig.CombatSkills[0]));
            CombatVM.Player.CombatSkillList.Add(new CombatCombatSkill(SkillManager.Instance.skillConfig.CombatSkills[1]));
            CombatVM.Player.CombatSkillList.Add(new CombatCombatSkill(SkillManager.Instance.skillConfig.CombatSkills[OpInfo.WeaponSkillId]));

            // 初始化
            var prefab = ResourceManager.Load<GameObject>("Characters/Player");
            var go = Instantiate(prefab, _agentsSpawnTrans);
            // 挂components
            // animator component
            var res = GetComponent<FbxLoadManager>().LoadModel(OpInfo.ModelResourceUrl, go.transform, true);
            // gun component
            {
                var gun = go.GetComponent<GunController>();
                gun.GunFire = res.GunfireEffect;
                gun.BulletStartTrans = res.GunfireTransform;
            }

            // 挂cinemachine
            m_Camera.Follow = go.transform.Find("Camera_Flowing");


            // 放入场景
            go.transform.position = pos;
            go.transform.eulerAngles = angle;
            PlayerTeamTrans.Add(go.transform);
            Operators.Add(go.transform, CombatVM.Player);

            // 还有驾驶舱
            GetComponent<FbxLoadManager>().LoadModel(OpInfo.ModelResourceUrl, CockpitManager.Instance.CharacterAnimator.transform, false);

            PlayerTrans = go.transform;

            return go.transform;
        }

        public Transform GenerateFighter(Operator OpInfo, Vector3 pos, Vector3 angle, int Team, Transform cvBase)
        {
            // 初始化
            var prefab = ResourceManager.Load<GameObject>("Characters/Fighter");
            var go = Instantiate(prefab, _agentsSpawnTrans);

            // 挂components
            // animator component
            GetComponent<FbxLoadManager>().LoadModel(OpInfo.ModelResourceUrl, go.transform.Find("modelroot"), go.transform, false);
            go.GetComponent<FighterController>().CvBase = cvBase;

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

        public Transform CreateGO(GameObject prefab, Transform parent)
        {
            var go = Instantiate(prefab, parent);

            return go.transform;

        }

        // ********************* UI logic *********************

        /// <summary>
        /// 获得UI显示用的冷却比值
        /// </summary>
        public float GetCoolDownRatio(int index, float time)
        {
            if (Operators[PlayerTrans].CombatSkillList[index].IsCoolDowning(time))
            {
                return (Operators[PlayerTrans].CombatSkillList[index].CoolDownEndTime - time) / Operators[PlayerTrans].CombatSkillList[index].SkillInfo.CoolDown;
            }
            else
            {
                return 0f;
            }
        }

        public bool IsPlayerNoShield()
        {
            return (float)Operators[PlayerTrans].CurrentHP / Operators[PlayerTrans].MaxHP < 0.5;
        }
        public void GameFinish()
        {
            Time.timeScale = 0;
        }
        
        public void QuitScene()
        {
            SceneManager.LoadScene("Home", LoadSceneMode.Single);
        }

        private bool isEscMenu = false;
        public void OnEscMenu(InputValue value)
        {
            if (isEscMenu) return;
            isEscMenu = true;
            Time.timeScale = 0;
            var ui = EscMenuUI.OpenEscMenuUI();
            ui.ReturnBtn.onClick.AddListener(() =>
            {
                isEscMenu = false;
                Time.timeScale = 1;
            });
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
            public GunController PlayerGun { get; set; }

            /// <summary>
            /// 玩家属性
            /// </summary>
            public CombatOperator Player { get; set; }
        }

    }
}
