using Assets.Scripts.CombatLogic.LevelLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace Assets.Scripts.CombatLogic.UILogic
{
    public class QuestUI : SubUIBase
    {

        public TextMeshProUGUI MainText;

        private void Start()
        {
            GameLevelManager.Instance.AimChangeEvent += ChangeText; 
        }

        private void ChangeText(string text)
        {
            MainText.text = text;
        }

        private void OnDestroy()
        {
            GameLevelManager.Instance.AimChangeEvent -= ChangeText;
        }
    }
}
