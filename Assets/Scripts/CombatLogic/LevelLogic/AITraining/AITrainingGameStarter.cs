using Assets.Scripts.CombatLogic.Characters;
using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using Assets.Scripts.CombatLogic.Characters.Player;
using Assets.Scripts.Entities.Level;
using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.LevelLogic
{
    /// <summary>
    /// 训练模式游戏启动器 - 简化为只提供基础接口
    /// 主要训练逻辑由 AITrainingSceneManager 处理
    /// </summary>
    public class AITrainingGameStarter : MonoBehaviour
    {
        [Header("训练配置")]
        public AITrainingConfig TrainingConfig;
        
        [Header("训练控制")]
        public bool AutoStartTraining = true;

        private AITrainingSceneManager _sceneManager;

        private void Awake()
        {
            _sceneManager = GetComponent<AITrainingSceneManager>();
            if (_sceneManager == null)
            {
                _sceneManager = gameObject.AddComponent<AITrainingSceneManager>();
            }
        }

        private void Start()
        {
            if (AutoStartTraining && TrainingConfig != null)
            {
                // 将配置传递给 SceneManager
                _sceneManager.TrainingConfig = TrainingConfig;
                _sceneManager.AutoStartTraining = true;
                _sceneManager.EnableTrainingMode = true;
            }
        }

        /// <summary>
        /// 手动触发训练（用于测试）
        /// </summary>
        [ContextMenu("Start Training")]
        public void ManualStartTraining()
        {
            if (TrainingConfig != null)
            {
                _sceneManager.TrainingConfig = TrainingConfig;
                _sceneManager.ManualStartTraining();
            }
            else
            {
                Debug.LogError("[AITraining] 未设置训练配置");
            }
        }
    }
}