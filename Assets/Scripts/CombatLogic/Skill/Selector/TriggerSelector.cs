using Assets.Scripts.CombatLogic.Skill.Impactor;
using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Selector
{
    internal class TriggerSelector : MonoBehaviour, ISelector
    {
        List<IImpactor> _impactors;
        CombatSkill _skill;
        Vector3 _aim;
        public void Init(List<IImpactor> impectors, Transform caster, CombatSkill skill, Vector3 aim)
        {
            _impactors = impectors;
            _skill = skill;
            _aim = aim;

            if (_skill.EffectType == SkillEffectType.PaticalPrefab)
            {
                transform.eulerAngles = Vector3.zero;
                GetComponent<ParticleSystem>().Play();
            }
            else if (_skill.EffectType == SkillEffectType.Shoot || _skill.EffectType == SkillEffectType.ShootAndFreeze)
            {
                GetComponent<Rigidbody>().velocity = (_aim - transform.position).normalized * 20 / 0.2f;
            }
            else if (_skill.EffectType == SkillEffectType.Throw)
            {
                GetComponent<Rigidbody>().velocity = _aim - transform.position;
            }
            GetComponent<Collider>().enabled = true;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (SkillManager.IsValidCollision(collision))
            {
                foreach(var imp in _impactors)
                {
                    imp.Impact(collision.transform);
                }
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (_skill.EffectType == SkillEffectType.ShootAndFreeze)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
            if (SkillManager.IsValidCollision(collision.collider))
            {
                foreach (var imp in _impactors)
                {
                    imp.Impact(collision.transform);
                }
            }
        }
    }
}
