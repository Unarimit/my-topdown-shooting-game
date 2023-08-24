using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class LightFleshContoller : MonoBehaviour
    {
        [Tooltip("ms")]
        public int Interval = 500;

        [Tooltip("ms")]
        public int Start = 0;

        private int shake;
        private MeshRenderer BoxColliderClick;
        private void Awake()
        {
            shake += Start;
            BoxColliderClick = gameObject.GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            shake += (int)(Time.deltaTime*1000);
            if (shake % Interval > Interval / 2)
            {
                BoxColliderClick.enabled = true;
            }
            else
            {
                BoxColliderClick.enabled = false;
            }
        }
    }
}
