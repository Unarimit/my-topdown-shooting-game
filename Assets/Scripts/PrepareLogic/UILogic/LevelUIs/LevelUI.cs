using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

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

        }
    }
}
