using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace Assets.Scripts.Editor
{
    internal class BundleBuilder : EditorWindow
    {
        [MenuItem("Tools/Build AssetBundle")]
        private static void BuildBundleMenu()
        {

            var helper = new BuilderHelper();
            helper.BuildBundle();
            Debug.Log($"build bindle complete");
        }

        class BuilderHelper
        {
            public void BuildBundle()
            {
                // 1. 考虑build平台的转换
                // TODO

                // 2. 按照某种规则收集ab包的资产，遇到循环依赖会报错
                var bundleDic = collectAssets();

                // 3. 根据依赖信息打ab包
                var manfinfect = buildBundle(bundleDic);

                // 4. 保存Manifest（依赖信息，用于读取ab包时解析）
                saveManifest(manfinfect);
            }

            
            /// <summary>
            /// 打包指定目录的assets和其依赖，使用并查集检测环，存在环则使用dfs输出依赖异常的地方
            /// </summary>
            private Dictionary<string, List<string>> collectAssets()
            {
                // 1. 获取需要被打包的Assets
                var assetsDic = new Dictionary<string, List<string>>();
                dfsGetFile(BundleConfig.BuildAssetsRoot, assetsDic);
                
                // 2. 检查循环依赖

                throw new NotImplementedException();
            }

            private AssetBundleManifest buildBundle(Dictionary<string, List<string>> bundleDic)
            {
                throw new NotImplementedException();
            }

            private void saveManifest(AssetBundleManifest manfinfect)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// 遍历指定目录下的所有文件
            /// </summary>
            private void dfsGetFile(string prefix, Dictionary<string, List<string>> dic)
            {
                // Directory.GetDirectories 获取的path以`\\`作为分割，unity的则以`/`分割，统一一下表示。
                prefix = prefix.Replace('\\', '/');
                dic.Add(prefix, Directory.GetFiles(prefix).ToList());

                foreach (var sub in Directory.GetDirectories(prefix))
                {
                    dfsGetFile(sub, dic);
                }
            }

        }
    }
}
