using UnityEngine;

namespace Assets.Scripts.PrepareLogic
{
    internal class PrepareStartupManager : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<PrepareContextManager>().Level = TestDB.Level;
            Time.timeScale = 1; // avoid dotween stop
        }
    }
}
