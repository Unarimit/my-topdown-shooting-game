using Assets.Scripts.CombatLogic.LevelLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    /// <summary>
    /// 存储管理，用于做条件判断和战利品？
    /// </summary>
    public class StorageManager : MonoBehaviour
    {
        public static StorageManager Instance;
        Dictionary<string, int> storages = new Dictionary<string, int>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }
        public void AddObject(string key, int amount)
        {
            if(!storages.ContainsKey(key)) storages[key] = 0;
            storages[key] += amount;
            GameLevelManager.Instance.CheckAim(key);
        }
        public void AddObject(string key)
        {
            AddObject(key, 1);
        }
        public void LostObject(string key)
        {

        }
        public int GetValue(string key)
        {
            if (storages.ContainsKey(key)) return storages[key];
            else return 0;
        }


    }
}
