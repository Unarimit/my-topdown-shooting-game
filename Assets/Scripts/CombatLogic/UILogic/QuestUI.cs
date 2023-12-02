using Assets.Scripts.CombatLogic.LevelLogic;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.UILogic
{
    public class QuestUI : SubUIBase
    {

        public TextMeshProUGUI MainText;

        private void OnEnable()
        {
            if (GameLevelManager.Instance == null) return;
            GameLevelManager.Instance.AimChangeEvent += ChangeText; 
        }

        private void ChangeText(string text)
        {
            MainText.text = text;
        }

        private void OnDisable()
        {
            if (GameLevelManager.Instance == null) return;
            GameLevelManager.Instance.AimChangeEvent -= ChangeText;
        }
        public override void TweenQuit(float duration)
        {
            var rect = GetComponent<RectTransform>();
            rect.DOAnchorPos(rect.anchoredPosition + new Vector2(rect.rect.width, 0), duration)
                .OnComplete(() => base.TweenQuit(duration));
        }
    }
}
