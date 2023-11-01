using Assets.Scripts.ComputerControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        public RawImage CockpitShow;

        private CockpitManager _cockpitManager;

        private CombatContextManager _context;
        private void Start()
        {
            _context = CombatContextManager.Instance;

            // prepare render cockpit
            // SceneManager.LoadScene("Cockpit 1", LoadSceneMode.Additive);
            _cockpitManager = CockpitManager.Instance;
        }

        private void OnGUI()
        {
            Skill1Mask.fillAmount = _context.GetCoolDownRatio(0, Time.time);
            Skill2Mask.fillAmount = _context.GetCoolDownRatio(1, Time.time);
            HPSlider.value = (float)_context.Operators[_context.PlayerTrans].CurrentHP / _context.Operators[_context.PlayerTrans].MaxHP;

            if (_context.Operators[_context.PlayerTrans].Speed <= 1)
            {
                _cockpitManager.ChangeState(CockpitWalkState.Idle);
            }
            else if (_context.Operators[_context.PlayerTrans].Speed <= 4)
            {
                _cockpitManager.ChangeState(CockpitWalkState.Walking);
            }
            else
            {
                _cockpitManager.ChangeState(CockpitWalkState.Running);
            }
        }
    }
}
