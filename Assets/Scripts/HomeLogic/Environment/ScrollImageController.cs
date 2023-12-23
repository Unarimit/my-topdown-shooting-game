using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.Environment
{
    internal class ScrollImageController : MonoBehaviour
    {
        RawImage ri;
        string textureName = "_MainTex";
        private void Awake()
        {
            ri = GetComponent<RawImage>();

        }
        public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
        Vector2 uvOffset = Vector2.zero;
        private void LateUpdate()
        {
            uvOffset += (uvAnimationRate * Time.deltaTime);
            ri.uvRect = new Rect(uvOffset, new Vector2(1, 1));
        }
    }
}
