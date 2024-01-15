using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using BehaviorDesigner.Runtime.Tactical;
using System;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Characters
{
    internal class OperatorBehaviorTreeController : MonoBehaviour, IAttackAgent, IDamageable
    {
        CombatContextManager _context => CombatContextManager.Instance;

        private void Awake()
        {
            
        }

        public void Attack(Vector3 targetPosition)
        {
            GetComponent<AgentController>().Shoot(new Vector3(targetPosition.x, 0.8f, targetPosition.z));
        }

        public float AttackAngle()
        {
            return 30f;
        }

        public float AttackDistance()
        {
            return 10f;
        }

        public bool CanAttack()
        {
            return true;
        }

        public void Damage(float amount)
        {
            throw new NotImplementedException();
        }

        public bool IsAlive()
        {
            return _context.Operators[transform].IsDead is false;
        }
    }
}
