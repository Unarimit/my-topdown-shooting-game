using Assets.Scripts.PrepareLogic.EffectLogic;
using Assets.Scripts.PrepareLogic.UILogic.TeammateUIs.CharacterSelector;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic.UILogic.EnemyUIs
{
    internal class EnemyPortraitScrollViewUI : MonoBehaviour
    {
        public GameObject EnemyPortraitPrefab;
        private PrepareContextManager _context => PrepareContextManager.Instance;

        private List<EnemyPortraitScrollViewItemUI> _characterPortraits;
        private Transform _portraitsContentTrans;
        private void Start()
        {
            _portraitsContentTrans = transform.Find("Viewport").Find("Content");
        }
        public void GeneratePortrait()
        {
            var ops = _context.Level.EnemyOperators;
            _characterPortraits = new List<EnemyPortraitScrollViewItemUI>();
            foreach (var op in ops)
            {
                var go = Instantiate(EnemyPortraitPrefab, _portraitsContentTrans);

                // set content
                var cp = go.GetComponent<EnemyPortraitScrollViewItemUI>();
                cp.Inject(op);

                _characterPortraits.Add(cp);
            }
        }
    }
}
