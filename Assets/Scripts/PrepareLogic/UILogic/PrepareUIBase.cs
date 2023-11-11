using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic.UILogic
{
    public class PrepareUIBase : MonoBehaviour
    {
        public virtual void Enter()
        {
            gameObject.SetActive(true);
        }

        public virtual void Quit()
        {
            gameObject.SetActive(false);
        }
    }
}
