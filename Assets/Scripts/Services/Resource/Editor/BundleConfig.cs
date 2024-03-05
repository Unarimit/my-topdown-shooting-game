using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    internal static class BundleConfig
    {
        public static readonly string test = "";
        /// <summary> 需要被打包的资源的根目录 </summary>
        public static readonly string BuildAssetsRoot = $"{Application.dataPath}/AstBundleDirectory";
    }
}
