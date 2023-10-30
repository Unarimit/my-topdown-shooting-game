using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CombatLogic.UILogic
{
    public class BreakHUDController : SubUIBase
    {
        Image _image;
        private void Start()
        {
            _image = GetComponent<Image>();    
        }

        private void OnGUI()
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0.3f + Math.Abs(0.5f - (Time.time * 100) % 500 / 500)); // 0.3~0.8
        }
    }
}
