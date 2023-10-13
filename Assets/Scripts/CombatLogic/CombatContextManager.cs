using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.ComputerControllers
{

    public class CombatContextManager : MonoBehaviour
    {
        public List<Transform> PlayerTeamTrans;

        public List<Transform> EnemyTeamTrans;

        public static CombatContextManager Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
            TeammateStatu = TeammateStatus.Follow;
        }

        private void Start()
        {
            TeammateText.text = TeammateStatu.ToString();
        }

        // Teammate Statu
        public enum TeammateStatus
        {
            Follow,
            Forward,
            StandBy
        }
        public TextMeshProUGUI TeammateText;
        [HideInInspector]
        public TeammateStatus TeammateStatu;
        public void ChangeTeammateStatu()
        {
            TeammateStatu = (TeammateStatus)(((int)TeammateStatu + 1) % 3);
            TeammateText.text = TeammateStatu.ToString();
        }

        // Engage Channel
        public delegate void OnEnemyEngageHandler(Transform sender, Vector3 poi);
        public event OnEnemyEngageHandler OnEnemyEngageEvent;
        public void EnemyFindCounter(Transform sender, Vector3 poi)
        {
            OnEnemyEngageEvent.Invoke(sender, poi);
        }
        /*
        public delegate void OnTeammateEngageHandler(Transform sender, Vector3 poi);
        public event OnTeammateEngageHandler OnTeammateEngageEvent;
        */

        public bool IsPlayer(Transform transform)
        {
            return transform == PlayerTeamTrans[0];
        }
    }
}
