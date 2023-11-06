using UnityEngine;

namespace Assets.Scripts.CombatLogic.LevelLogic
{
    public class GameLevelManager : MonoBehaviour
    {
        public static GameLevelManager Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }

    }
}
