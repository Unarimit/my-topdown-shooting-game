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
            GetComponent<AgentController>().Aim(true, new Vector3(targetPosition.x, 0.8f, targetPosition.z));
            GetComponent<AgentController>().Shoot(new Vector3(targetPosition.x, 0.8f, targetPosition.z));
        }

        public float AttackAngle()
        {
            return 60f;
        }

        float? distance;
        public float AttackDistance()
        {
            if(distance == null)
            {
                distance = GetComponent<OperatorController>().Model.WeaponSkill.SkillInfo.RangeTip;
            }

            return distance.Value;
        }

        public bool CanAttack()
        {
            return true;
        }

        public bool IsHurt()
        {
            return _context.Operators[transform].CurrentHP < _context.Operators[transform].MaxHP;
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
