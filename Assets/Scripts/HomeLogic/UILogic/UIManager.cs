﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic
{
    internal class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }
    }
}
