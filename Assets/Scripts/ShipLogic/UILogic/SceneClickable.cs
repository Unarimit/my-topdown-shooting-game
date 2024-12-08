using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ShipLogic.UILogic
{
    internal class SceneClickable : MonoBehaviour
    {
        public Transform LeaderRoot;

        private void Start()
        {
            CommonLoader.Instance.FbxLoader.LoadModel("hoshino", LeaderRoot, false);
        }
    }
}
