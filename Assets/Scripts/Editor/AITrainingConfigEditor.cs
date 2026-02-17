#if UNITY_EDITOR
using Assets.Scripts.CombatLogic.LevelLogic;
using Assets.Scripts.Entities.Level;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    /// <summary>
    /// AI训练配置的编辑器工具
    /// 用于快速创建和管理训练配置
    /// </summary>
    public class AITrainingConfigEditor : EditorWindow
    {
        private string _configName = "New Training Config";
        private string _savePath = "Assets/Scenes/Playground";
        private bool _useAIForPlayerSlot = true;
        private bool _useMultipleOpponents = true;
        private float _maxBattleDuration = 120f;

        [MenuItem("Tools/AI Training/Create Training Config")]
        public static void ShowWindow()
        {
            GetWindow<AITrainingConfigEditor>("AI Training Config");
        }

        private void OnGUI()
        {
            GUILayout.Label("创建AI训练配置", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _configName = EditorGUILayout.TextField("配置名称", _configName);
            _savePath = EditorGUILayout.TextField("保存路径", _savePath);
            
            EditorGUILayout.Space();
            GUILayout.Label("基础设置", EditorStyles.boldLabel);
            
            _useAIForPlayerSlot = EditorGUILayout.Toggle("AI接管第0单位", _useAIForPlayerSlot);
            _useMultipleOpponents = EditorGUILayout.Toggle("多对手测试", _useMultipleOpponents);
            _maxBattleDuration = EditorGUILayout.FloatField("最大战斗时长(秒)", _maxBattleDuration);

            EditorGUILayout.Space();

            if (GUILayout.Button("创建配置", GUILayout.Height(30)))
            {
                CreateTrainingConfig();
            }

            EditorGUILayout.Space();
            GUILayout.Label("快捷操作", EditorStyles.boldLabel);

            if (GUILayout.Button("在GameScene目录创建默认配置"))
            {
                CreateDefaultGameSceneConfig();
            }
        }

        private void CreateTrainingConfig()
        {
            var config = ScriptableObject.CreateInstance<AITrainingConfig>();
            config.ConfigName = _configName;
            config.UseAIForPlayerSlot = _useAIForPlayerSlot;
            config.UseMultipleOpponents = _useMultipleOpponents;
            config.MaxBattleDuration = _maxBattleDuration;
            config.TeamOperatorCount = 5;
            config.EnemyOperatorCount = 5;
            config.MapType = MapType.Small;

            string path = $"{_savePath}/{_configName.Replace(" ", "_")}.asset";
            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("成功", $"训练配置已创建:\n{path}", "确定");
            Selection.activeObject = config;
        }

        private void CreateDefaultGameSceneConfig()
        {
            var config = ScriptableObject.CreateInstance<AITrainingConfig>();
            config.ConfigName = "GameScene AI Training";
            config.UseAIForPlayerSlot = true;
            config.UseMultipleOpponents = true;
            config.MaxBattleDuration = 120f;
            config.VerboseLogging = true;
            
            // 阵容配置
            config.TeamOperatorCount = 5;
            config.EnemyOperatorCount = 5;
            config.TeamLevelOffset = 0;
            config.EnemyLevelOffset = 0;
            config.EnemyAiAggressive = true;

            // 地图配置
            config.MapType = MapType.Small;

            string path = "Assets/Scenes/Playground/GameScene_TrainingConfig.asset";
            
            // 确保目录存在
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));

            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("成功", $"默认训练配置已创建:\n{path}\n\n请在Inspector中配置行为树引用。", "确定");
            Selection.activeObject = config;
        }
    }

    /// <summary>
    /// 场景中的AI训练设置编辑器
    /// </summary>
    [CustomEditor(typeof(AITrainingSceneSetup))]
    public class AITrainingSceneSetupInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var setup = (AITrainingSceneSetup)target;

            EditorGUILayout.Space();
            GUILayout.Label("快捷操作", EditorStyles.boldLabel);

            if (GUILayout.Button("立即开始训练"))
            {
                if (EditorApplication.isPlaying)
                {
                    setup.StartTrainingNow();
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "请先进入Play模式", "确定");
                }
            }

            EditorGUILayout.Space();
            
            if (GUILayout.Button("创建训练配置ScriptableObject"))
            {
                AITrainingConfigEditor.ShowWindow();
            }
        }
    }
}
#endif