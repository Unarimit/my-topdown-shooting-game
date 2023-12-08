using Assets.Scripts.Common;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic
{
    internal class PrepareStartupManager : MonoBehaviour
    {
        private void Awake()
        {
            if(MyServices.Database.CurLevel == null)
            {
                Debug.Log("DB has no level info, create 0 index level");
                MyServices.Database.CurLevel = LevelGenerator.GeneratorLevelInfo(MyServices.Database.LevelRules[0]);
                //TODO：对非测试情况下的处理
            }

            GetComponent<PrepareContextManager>().Inject(MyServices.Database.CurLevel);
            Time.timeScale = 1; // avoid dotween stop
        }
    }
}
