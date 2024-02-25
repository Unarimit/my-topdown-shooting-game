using Assets.Scripts.CombatLogic.Skill.Impactor;
using Assets.Scripts.CombatLogic.Skill.Releaser;
using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XLua;

namespace Assets.Scripts.CombatLogic.Skill.Selector
{
    /// <summary>
    /// 利用碰撞箱的选择器，用lua指定轨迹
    /// </summary>
    internal class LuaTriggerSelector : MonoBehaviour, ISelector
    {
        List<IImpactor> _impactors;
        CombatSkill _skill;
        Action<GameObject, Transform, Vector3> action;
        public void Init(List<IImpactor> impectors, BaseReleaser releaser)
        {
            _impactors = impectors;
            _skill = releaser.Skill;
            action =  MyServices.LuaEnv.Global.GetInPath<Action<GameObject, Transform, Vector3>>(_skill.SkillSelector.Data);
            action(gameObject, releaser.Caster, releaser.Aim);

            // reset碰撞系统
            GetComponent<Collider>().enabled = true;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
        private void OnTriggerEnter(Collider collision)
        {
            if (SkillManager.IsValidCollision(collision))
            {
                foreach (var imp in _impactors)
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
                GetComponent<Collider>().enabled = false;
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
