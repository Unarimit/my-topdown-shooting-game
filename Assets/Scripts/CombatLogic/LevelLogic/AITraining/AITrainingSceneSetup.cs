using Assets.Scripts.Entities.Level;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.LevelLogic
{
    /// <summary>
    /// 场景中的AI训练配置组件 - 简化为只提供配置
    /// 主要训练逻辑由 AITrainingSceneManager 处理
    /// </summary>
    public class AITrainingSceneSetup : MonoBehaviour
    {
        [Header("训练模式")]
        public bool EnableTrainingMode = false;
        public bool AutoStartTraining = true;

        [Header("训练配置")]
        public AITrainingConfig TrainingConfigAsset;

        [Header("行为树配置")]
        public ExternalBehaviorTree TeamBehaviorTree;
        public ExternalBehaviorTree EnemyBaseBehaviorTree;
        public ExternalBehaviorTree BestBehaviorTree;

        private AITrainingSceneManager _sceneManager;

        private void Awake()
        {
            if (!EnableTrainingMode) return;

            _sceneManager = GetComponent<AITrainingSceneManager>();
            if (_sceneManager == null)
            {
                _sceneManager = gameObject.AddComponent<AITrainingSceneManager>();
            }

            // 传递配置
            SetupSceneManager();
        }

        private void SetupSceneManager()
        {
            if (TrainingConfigAsset != null)
            {
                _sceneManager.TrainingConfig = TrainingConfigAsset;
            }
            else
            {
                // 创建运行时配置
                var config = ScriptableObject.CreateInstance<AITrainingConfig>();
                config.TeamBehaviorTree = TeamBehaviorTree;
                config.EnemyBehaviorTree = EnemyBaseBehaviorTree;
                config.BaseBehaviorTree = EnemyBaseBehaviorTree;
                config.BestBehaviorTree = BestBehaviorTree;
                _sceneManager.TrainingConfig = config;
            }

            _sceneManager.AutoStartTraining = AutoStartTraining;
            _sceneManager.EnableTrainingMode = true;
        }

        [ContextMenu("Start Training")]
        public void StartTrainingNow()
        {
            if (_sceneManager == null)
            {
                _sceneManager = GetComponent<AITrainingSceneManager>();
                SetupSceneManager();
            }
            _sceneManager.ManualStartTraining();
        }
    }
}