using Assets.Scripts.Entities;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

namespace Assets.Scripts.PrepareLogic.UILogic.LevelUIs
{
    internal class LevelUI : PrepareUIBase
    {
        private TextMeshProUGUI _levelNameTMP;
        private TextMeshProUGUI _levelWinAimTMP;
        private TextMeshProUGUI _levelLossAimTMP;
        private LevelMapUI _levelMapUI;
        private void Start()
        {
#if UNITY_EDITOR
            if(_context.Level == null)
            {
                // TODO: 可能处于debug模式下
                return;
            }
#endif
            _levelNameTMP = transform.Find("LevelNameTMP").GetComponent<TextMeshProUGUI>();
            _levelWinAimTMP = transform.Find("LevelWinAimTMP").GetComponent<TextMeshProUGUI>();
            _levelLossAimTMP = transform.Find("LevelLossAimTMP").GetComponent<TextMeshProUGUI>();
            _levelMapUI = transform.Find("MiniMapRawImage").GetComponent<LevelMapUI>();

            _levelNameTMP.text = _context.Level.LevelRule.LevelName;
            _levelWinAimTMP.text = $"胜利条件：\n{_context.Level.GetWinDesc()}";
            _levelLossAimTMP.text = $"失败条件：\n{_context.Level.GetLossDesc()}";
            _levelMapUI.DrawMap(_context.Level.Map, _context.Level.LevelRule.TeamSpawn, _context.Level.LevelRule.EnemySpawn);

            // drop outs
            var dropouts = new List<GameItem>();
            foreach(var enemy in _context.Level.LevelRule.OperatorPrefabs)
            {
                if (enemy.Dropouts == null) continue;
                foreach(var x in enemy.Dropouts)
                {
                    dropouts.Add(x.DropItem);
                }
            }
            dropouts.DistinctBy(x => x.ItemId);
            transform.Find("LevelDropoutPanel").Find("Scroll View").GetComponent<LevelDropoutScrollViewUI>().Inject(dropouts);
        }
    }
}
