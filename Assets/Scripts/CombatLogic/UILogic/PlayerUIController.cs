﻿using Assets.Scripts.ComputerControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CombatLogic.UILogic
{
    public class PlayerUIController : SubUIBase
    {
        public RawImage Skill1Icon;
        public RawImage Skill2Icon;
        public Image Skill1Mask;
        public Image Skill2Mask;

        public Slider HPSlider;

        private CombatContextManager _context;
        private void Start()
        {
            _context = CombatContextManager.Instance;
        }

        private void OnGUI()
        {
            Skill1Mask.fillAmount = _context.GetCoolDownRatio(0, Time.time);
            Skill2Mask.fillAmount = _context.GetCoolDownRatio(1, Time.time);
            HPSlider.value = (float)_context.Operators[_context.PlayerTrans].CurrentHP / _context.Operators[_context.PlayerTrans].MaxHP;
        }
    }
}