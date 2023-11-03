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
            text.text = _context.Operators[_context.PlayerTrans].ReviveTime.ToString();
            if(_context.Operators[_context.PlayerTrans].ReviveTime == 0)
            {
                StartCoroutine(DelayClose());
            }
        }

        private IEnumerator DelayClose()
        {
            yield return new WaitForSeconds(1);
            SetVisible(false);
        }
    }
}
