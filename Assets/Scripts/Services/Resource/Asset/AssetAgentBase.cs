using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services.Resource.Asset
{
    internal abstract class AssetAgentBase<T> : IAssetAgent<T>
        where T : UnityEngine.Object
    {
        public abstract string url { get; }

        public abstract T GetAsset(GameObject assetUser);
        public abstract GameObject Instantiate(Transform parent);
    }
}
