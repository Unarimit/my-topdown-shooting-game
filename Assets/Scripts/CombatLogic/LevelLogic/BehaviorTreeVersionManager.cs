using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.LevelLogic
{
    /// <summary>
    /// 行为树版本管理器，管理原始版本、历史最优版本和候选版本
    /// 支持版本保存、加载、对比和迭代
    /// </summary>
    public class BehaviorTreeVersionManager : MonoBehaviour
    {
        public static BehaviorTreeVersionManager Instance { get; private set; }

        [Header("版本存储路径")]
        [Tooltip("行为树版本存储的相对路径")]
        public string VersionsPath = "Resources/BehaviorTreeVersions";

        [Header("基础行为树")]
        [Tooltip("原始基础行为树（只读，用于对比）")]
        public ExternalBehaviorTree BaseBehaviorTree;

        [Header("当前版本")]
        [Tooltip("当前最优行为树")]
        public ExternalBehaviorTree BestBehaviorTree;

        [Tooltip("当前候选行为树（正在测试的版本）")]
        public ExternalBehaviorTree CandidateBehaviorTree;

        // 版本历史记录
        private List<BehaviorTreeVersionInfo> _versionHistory = new List<BehaviorTreeVersionInfo>();

        // 版本迭代计数
        private int _iterationCount = 0;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            LoadVersionHistory();
        }

        /// <summary>
        /// 行为树版本信息
        /// </summary>
        [Serializable]
        public class BehaviorTreeVersionInfo
        {
            public string VersionId;
            public string VersionName;
            public string CreatedTime;
            public float WinRate;
            public float AverageVictoryScore;
            public string ParentVersionId; // 父版本ID（用于追溯迭代路径）
            public string Description; // 版本描述（如"增加撤退逻辑"）
            public string FilePath; // 存储路径
            public bool IsBaseVersion; // 是否是基础版本
            public List<string> TestedAgainst = new List<string>(); // 测试过的对手
        }

        /// <summary>
        /// 从候选版本创建新版本
        /// </summary>
        public BehaviorTreeVersionInfo CreateNewVersion(string description, float winRate, float avgScore)
        {
            _iterationCount++;

            var versionInfo = new BehaviorTreeVersionInfo
            {
                VersionId = Guid.NewGuid().ToString("N")[..8],
                VersionName = $"BT_v{_iterationCount}",
                CreatedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                WinRate = winRate,
                AverageVictoryScore = avgScore,
                Description = description,
                ParentVersionId = BestBehaviorTree != null ? GetVersionIdOf(BestBehaviorTree) : null
            };

            // 保存行为树资源
            string fileName = $"{versionInfo.VersionName}_{versionInfo.VersionId}.asset";
            string fullPath = Path.Combine(Application.dataPath, VersionsPath, fileName);
            
            // 确保目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // 复制候选行为树
            if (CandidateBehaviorTree != null)
            {
#if UNITY_EDITOR
                AssetDatabase.CopyAsset(
                    AssetDatabase.GetAssetPath(CandidateBehaviorTree),
                    $"Assets/{VersionsPath}/{fileName}"
                );
                versionInfo.FilePath = $"Assets/{VersionsPath}/{fileName}";
                
                // 更新最优版本
                BestBehaviorTree = AssetDatabase.LoadAssetAtPath<ExternalBehaviorTree>(versionInfo.FilePath);
#endif
            }

            _versionHistory.Add(versionInfo);
            SaveVersionHistory();

            Debug.Log($"[BehaviorTreeVersion] 创建新版本: {versionInfo.VersionName} (胜率: {winRate:P1}, 得分: {avgScore:F1})");

            return versionInfo;
        }

        /// <summary>
        /// 将候选版本提升为最优版本
        /// </summary>
        public bool PromoteCandidateToBest(string description)
        {
            if (CandidateBehaviorTree == null)
            {
                Debug.LogError("[BehaviorTreeVersion] 没有候选版本可提升");
                return false;
            }

            // 这里需要获取训练结果，暂时使用默认值
            // 实际应该在训练完成后调用
            var trainingManager = AITrainingManager.Instance;
            if (trainingManager == null || trainingManager.IsTrainingMode)
            {
                Debug.LogWarning("[BehaviorTreeVersion] 训练未完成，无法提升版本");
                return false;
            }

            // 从训练结果获取数据
            // 这里简化处理，实际应该传递训练结果
            CreateNewVersion(description, 0.7f, 150f);

            return true;
        }

        /// <summary>
        /// 加载指定版本
        /// </summary>
        public ExternalBehaviorTree LoadVersion(string versionId)
        {
            var versionInfo = _versionHistory.Find(v => v.VersionId == versionId);
            if (versionInfo == null)
            {
                Debug.LogError($"[BehaviorTreeVersion] 找不到版本: {versionId}");
                return null;
            }

#if UNITY_EDITOR
            var bt = AssetDatabase.LoadAssetAtPath<ExternalBehaviorTree>(versionInfo.FilePath);
            if (bt != null)
            {
                CandidateBehaviorTree = bt;
                Debug.Log($"[BehaviorTreeVersion] 加载版本: {versionInfo.VersionName}");
            }
            return bt;
#else
            return null;
#endif
        }

        /// <summary>
        /// 回滚到历史版本
        /// </summary>
        public bool RollbackToVersion(string versionId)
        {
            var bt = LoadVersion(versionId);
            if (bt != null)
            {
                BestBehaviorTree = bt;
                Debug.Log($"[BehaviorTreeVersion] 回滚到版本: {versionId}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取版本迭代路径
        /// </summary>
        public List<BehaviorTreeVersionInfo> GetVersionLineage(string versionId = null)
        {
            var lineage = new List<BehaviorTreeVersionInfo>();
            
            string currentId = versionId ?? GetVersionIdOf(BestBehaviorTree);
            
            while (!string.IsNullOrEmpty(currentId))
            {
                var version = _versionHistory.Find(v => v.VersionId == currentId);
                if (version == null) break;
                
                lineage.Insert(0, version);
                currentId = version.ParentVersionId;
            }

            return lineage;
        }

        /// <summary>
        /// 获取最优的N个版本（用于多对手测试）
        /// </summary>
        public List<ExternalBehaviorTree> GetTopVersions(int count)
        {
            var topVersions = new List<ExternalBehaviorTree>();
            
            // 按胜率排序
            _versionHistory.Sort((a, b) => b.WinRate.CompareTo(a.WinRate));
            
            int takeCount = Mathf.Min(count, _versionHistory.Count);
            for (int i = 0; i < takeCount; i++)
            {
                var bt = LoadVersion(_versionHistory[i].VersionId);
                if (bt != null) topVersions.Add(bt);
            }

            return topVersions;
        }

        /// <summary>
        /// 获取所有版本信息
        /// </summary>
        public List<BehaviorTreeVersionInfo> GetAllVersions()
        {
            return new List<BehaviorTreeVersionInfo>(_versionHistory);
        }

        /// <summary>
        /// 获取当前最优版本的统计信息
        /// </summary>
        public string GetBestVersionStats()
        {
            var best = _versionHistory.Find(v => !string.IsNullOrEmpty(v.FilePath) && 
                BestBehaviorTree != null && 
                v.FilePath == AssetDatabase.GetAssetPath(BestBehaviorTree));
            
            if (best == null) return "No best version found";

            return $"Version: {best.VersionName}\n" +
                   $"Win Rate: {best.WinRate:P1}\n" +
                   $"Avg Score: {best.AverageVictoryScore:F1}\n" +
                   $"Created: {best.CreatedTime}\n" +
                   $"Description: {best.Description}";
        }

        /// <summary>
        /// 导出版本对比报告
        /// </summary>
        public string ExportComparisonReport()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== 行为树版本对比报告 ===");
            sb.AppendLine();

            // 基础版本
            if (BaseBehaviorTree != null)
            {
                sb.AppendLine("[基础版本]");
                sb.AppendLine($"  Name: {BaseBehaviorTree.name}");
                sb.AppendLine();
            }

            // 所有迭代版本
            sb.AppendLine("[迭代历史]");
            foreach (var version in _versionHistory)
            {
                sb.AppendLine($"  {version.VersionName}:");
                sb.AppendLine($"    胜率: {version.WinRate:P1}");
                sb.AppendLine($"    均分: {version.AverageVictoryScore:F1}");
                sb.AppendLine($"    描述: {version.Description}");
                sb.AppendLine($"    时间: {version.CreatedTime}");
                sb.AppendLine();
            }

            // 当前最优
            if (BestBehaviorTree != null)
            {
                sb.AppendLine("[当前最优]");
                sb.AppendLine($"  Name: {BestBehaviorTree.name}");
                var bestInfo = _versionHistory.Find(v => v.FilePath == AssetDatabase.GetAssetPath(BestBehaviorTree));
                if (bestInfo != null)
                {
                    sb.AppendLine($"  胜率: {bestInfo.WinRate:P1}");
                    sb.AppendLine($"  均分: {bestInfo.AverageVictoryScore:F1}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 保存版本历史到JSON
        /// </summary>
        private void SaveVersionHistory()
        {
            string json = JsonUtility.ToJson(new Serialization<BehaviorTreeVersionInfo>(_versionHistory));
            string path = Path.Combine(Application.dataPath, VersionsPath, "version_history.json");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// 加载版本历史
        /// </summary>
        private void LoadVersionHistory()
        {
            string path = Path.Combine(Application.dataPath, VersionsPath, "version_history.json");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var wrapper = JsonUtility.FromJson<Serialization<BehaviorTreeVersionInfo>>(json);
                _versionHistory = wrapper?.Items ?? new List<BehaviorTreeVersionInfo>();
                
                // 恢复迭代计数
                if (_versionHistory.Count > 0)
                {
                    _iterationCount = _versionHistory.Count;
                }
            }
        }

        /// <summary>
        /// 获取行为树对应的版本ID
        /// </summary>
        private string GetVersionIdOf(ExternalBehaviorTree bt)
        {
            if (bt == null) return null;
            
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(bt);
            var version = _versionHistory.Find(v => v.FilePath == path);
            return version?.VersionId;
#else
            return null;
#endif
        }

        [Serializable]
        private class Serialization<T>
        {
            public List<T> Items;
            public Serialization(List<T> items) { Items = items; }
            public Serialization() { Items = new List<T>(); }
        }

        /// <summary>
        /// 创建基础版本的副本作为初始最优版本
        /// </summary>
        [ContextMenu("Initialize From Base")]
        public void InitializeFromBase()
        {
            if (BaseBehaviorTree == null)
            {
                Debug.LogError("[BehaviorTreeVersion] 未设置基础行为树");
                return;
            }

#if UNITY_EDITOR
            // 复制基础版本作为初始最优版本
            string basePath = AssetDatabase.GetAssetPath(BaseBehaviorTree);
            string fileName = $"BT_v0_Base_{Guid.NewGuid().ToString("N")[..6]}.asset";
            string targetPath = $"Assets/{VersionsPath}/{fileName}";
            
            Directory.CreateDirectory(Path.Combine(Application.dataPath, VersionsPath));
            AssetDatabase.CopyAsset(basePath, targetPath);
            
            BestBehaviorTree = AssetDatabase.LoadAssetAtPath<ExternalBehaviorTree>(targetPath);
            CandidateBehaviorTree = BestBehaviorTree;

            // 记录基础版本
            var baseVersion = new BehaviorTreeVersionInfo
            {
                VersionId = "base",
                VersionName = "BT_Base",
                CreatedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                WinRate = 0f,
                AverageVictoryScore = 0f,
                Description = "原始基础版本",
                FilePath = targetPath,
                IsBaseVersion = true
            };
            
            _versionHistory.Add(baseVersion);
            SaveVersionHistory();

            Debug.Log("[BehaviorTreeVersion] 已从基础版本初始化");
#endif
        }

        /// <summary>
        /// 清理所有版本（谨慎使用）
        /// </summary>
        [ContextMenu("Clear All Versions")]
        public void ClearAllVersions()
        {
            _versionHistory.Clear();
            SaveVersionHistory();
            Debug.Log("[BehaviorTreeVersion] 已清空所有版本历史");
        }
    }
}