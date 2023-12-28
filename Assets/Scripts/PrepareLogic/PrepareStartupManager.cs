using Assets.Scripts.Common;
using Assets.Scripts.Entities.Level;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic
{
    internal class PrepareStartupManager : MonoBehaviour
    {
        private void Awake()
        {
            if(MyServices.Database.CurCombatLevelInfo == null)
            {
                Debug.Log("DB has no level info, create 0 index level");
                MyServices.Database.CurCombatLevelInfo = CombatLevelGenerator.GeneratorLevelInfo((CombatLevelRule)MyServices.Database.LevelRules[0]);
                //TODO：对非测试情况下的处理
            }

            GetComponent<PrepareContextManager>().Inject(MyServices.Database.CurCombatLevelInfo);
            Time.timeScale = 1; // avoid dotween stop
        }
    }
}
