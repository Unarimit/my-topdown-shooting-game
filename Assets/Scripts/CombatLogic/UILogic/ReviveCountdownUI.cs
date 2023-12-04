using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.UILogic
{
    public class ReviveCountdownUI : SubUIBase
    {
        public TextMeshProUGUI text;
        private void OnGUI()
        {
            text.text = _context.Operators[_context.PlayerTrans].CurrentReviveTime.ToString("0.00");
            if(_context.Operators[_context.PlayerTrans].CurrentReviveTime == 0)
            {
                StartCoroutine(DelayClose());
            }
        }

        private IEnumerator DelayClose()
        {
            yield return new WaitForSeconds(1);
            SetVisible(false);
        }

        public override void TweenEnter(float duration)
        {
            if (_context.Operators[_context.PlayerTrans].CurrentReviveTime != 0)  SetVisible(true);
        }
    }
}
