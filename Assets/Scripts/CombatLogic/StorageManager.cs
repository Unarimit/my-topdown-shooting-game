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
        const string WIN_OBJECT = "win";
        const string LOSS_OBJECT = "loss";

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }
        private void Start()
        {
            storages[WIN_OBJECT] = 0;
            storages[LOSS_OBJECT] = 0;
        }
        public void AddObject(string key, int amount)
        {
            storages[key] += amount;
            CheckAim(key);
        }
        public void AddObject(string key)
        {
            AddObject(key, 1);
        }
        public void LostObject(string key)
        {

        }
        private void CheckAim(string key)
        {

            if (key == WIN_OBJECT && storages[key] >= 10) UIManager.Instance.ShowFinish(true);
            if (key == LOSS_OBJECT && storages[key] >= 6) UIManager.Instance.ShowFinish(false);
        }
    }
}
