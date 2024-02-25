using UnityEngine;

namespace Assets.Scripts.Services.Resource.Asset
{
    /// <summary>
    /// Asset获取代理，通过它获取Asset
    /// </summary>
    internal interface IAssetAgent<T> where T : Object
    {
        string url { get; }
        T GetAsset(GameObject assetUser);
        GameObject Instantiate(Transform parent);
    }
}
