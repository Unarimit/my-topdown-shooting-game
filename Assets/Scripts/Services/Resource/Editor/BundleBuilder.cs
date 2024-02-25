using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Editor
{
    internal class BundleBuilder : EditorWindow
    {
        [MenuItem("Tools/Build AssetBundle")]
        private static void BuildBundleMenu()
        {

            var helper = new BuilderHelper();
            Debug.Log($"build bindle complete");
        }

        class BuilderHelper
        {
            public void BuildBundle()
            {
                // 1. 考虑build平台的转换
                // TODO

                // 2. 收集资源目录的依赖信息，遇到循环依赖会报错
                var bundleDic = collectDependency();

                // 3. 根据依赖信息打ab包
                var manfinfect = buildBundle(bundleDic);

                // 4. 保存Manifest（依赖信息，用于读取ab包时解析）
                saveManifest(manfinfect);
            }

            

            private Dictionary<string, List<string>> collectDependency()
            {
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

        }
    }
}
