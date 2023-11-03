using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.ComputerControllers;
using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.LevelLogic
{
    public class CarbinController : MonoBehaviour
    {
        public CarbinConfig config;
        public Operator operatorInfo;


        private float _innerInterval = 0;
        private GameObject _agentPrefab;
        private HashSet<Transform> _agents = new HashSet<Transform>();
        private void Start()
        {
            _agentPrefab = Resources.Load<GameObject>("Characters/"+config.AgentPrefabURL);
        }
        private void FixedUpdate()
        {
            _innerInterval += Time.deltaTime;
            if(_innerInterval > config.SpawnInterval)
            {
                if (!checkCapacity()) return;
                _innerInterval -= config.SpawnInterval;
                var trans = CombatContextManager.Instance.GenerateAgent(_agentPrefab, transform.position + transform.forward * 3, new Vector3(), 
                    config.Team, operatorInfo, transform);
                _agents.Add(trans);
            }
        }
        /// <summary>
        /// 如果没达到上限返回true
        /// </summary>
        /// <returns></returns>
        private bool checkCapacity()
        {
            if (_agents.Count < config.MaxCapacity) return true;
            var removeList = new List<Transform>();
            foreach(var agent in _agents)
            {
                if(CombatContextManager.Instance.Operators.ContainsKey(agent)) continue;
                else
                {
                    removeList.Add(agent);
                }
            }

            foreach (var x in removeList) _agents.Remove(x);

            return _agents.Count < config.MaxCapacity;
        }
    }
}
