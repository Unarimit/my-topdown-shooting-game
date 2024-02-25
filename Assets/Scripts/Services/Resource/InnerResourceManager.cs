using Assets.Scripts.Services.Resource.Asset;
using System;

namespace Assets.Scripts.Services.Resource
{
    /// <summary>
    /// 管理ab包和资源加载的内部管理类
    /// </summary>
    internal class InnerResourceManager
    {
        private const string MANIFEST_BUNDLE = "manifest.ab";
        private const string RESOURCE_ASSET_NAME = "Assets/Temp/Resource.bytes";
        private const string BUNDLE_ASSET_NAME = "Assets/Temp/Bundle.bytes";
        private const string DEPENDENCY_ASSET_NAME = "Assets/Temp/Dependency.bytes";
        static InnerResourceManager()
        {

        }

        public IAssetAgent<T> Load<T>(string url) where T :  UnityEngine.Object
        {
            throw new NotImplementedException();
        }
    }
}
