using System.Collections;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.LevelLogic
{
    public class GameLevelManager : MonoBehaviour
    {
        public static GameLevelManager Instance;
        private StorageManager _storageContext;

        const string WIN_OBJECT = "win";
        const string LOSS_OBJECT = "loss";
        private int winob_aim = 10;
        private int lossob_aim = 6;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }
        private void Start()
        {
            _storageContext = StorageManager.Instance;
            StartCoroutine(DelayOneFrame());
            
        }
        IEnumerator DelayOneFrame()
        {
            yield return new WaitForEndOfFrame();
            AimChangeEvent.Invoke(generateText());
        }

        public delegate void AimChangeEventHandler(string text);
        public event AimChangeEventHandler AimChangeEvent;
        public void CheckAim(string key)
        {

            if (key == WIN_OBJECT)
            {
                if (_storageContext.GetValue(key) >= winob_aim)
                {
                    UIManager.Instance.ShowFinish(true);
                    CombatContextManager.Instance.GameFinish();
                }
            }
            if (key == LOSS_OBJECT)  
            {
                if (_storageContext.GetValue(key) >= lossob_aim)
                {
                    UIManager.Instance.ShowFinish(false);
                    CombatContextManager.Instance.GameFinish();
                }
            }
            AimChangeEvent.Invoke(generateText());

        }
        private string generateText()
        {
            return $"Win: Kill enemy ({_storageContext.GetValue(WIN_OBJECT)}/{winob_aim}) \n" +
                $"Loss: Team be killed ({_storageContext.GetValue(LOSS_OBJECT)}/{lossob_aim})";
        }
    }
}
