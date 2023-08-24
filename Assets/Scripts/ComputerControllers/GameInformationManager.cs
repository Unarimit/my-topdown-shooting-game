using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ComputerControllers
{

    public class GameInformationManager : MonoBehaviour
    {
        public List<Transform> PlayerTeamTrans;

        public List<Transform> EnemyTeamTrans;

        public static GameInformationManager Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }
    }
}
