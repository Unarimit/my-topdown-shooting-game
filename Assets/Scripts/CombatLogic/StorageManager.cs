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

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }

        // for game win
        private int KillAimCnt = 0;
        public void InitSet(int killAimCnt) { KillAimCnt = killAimCnt; }
        public void KillOne()
        {
            KillAimCnt -= 1;
            if (KillAimCnt == 0) UIManager.Instance.ShowFinish();
        }
    }
}
