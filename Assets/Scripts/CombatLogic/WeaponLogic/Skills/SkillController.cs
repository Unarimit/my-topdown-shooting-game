using Assets.Scripts.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    /// <summary>
    /// 控制技能的释放过程
    /// </summary>
    public class SkillController : MonoBehaviour
    {
        [HideInInspector]
        public float DelayEndTime;
        [HideInInspector]
        public Vector3 Aim;
        [HideInInspector]
        public CombatSkill CSkill;
        [HideInInspector]
        public Transform Caster;

        private CombatContextManager _context;
        private float durationEndTime;

        private void Awake() // 他总是创建出来的，所以可以在这里初始化，防止一些bug
        {
            _context = CombatContextManager.Instance;
        }
        /// <summary>
        /// 释放技能
        /// </summary>
        public void Cast(float time)
        {
            durationEndTime = time + CSkill.Duration;
            // 如果发射
            if(CSkill.EffectType == SkillEffectType.PaticalPrefab)
            {
                GetComponent<ParticleSystem>().Play();
            }
            else if(CSkill.EffectType == SkillEffectType.Shoot)
            {
                GetComponent<Rigidbody>().velocity = transform.forward * 20 / 0.2f;
                StartCoroutine(DelayStop());
            }
            else if (CSkill.EffectType == SkillEffectType.Throw)
            {
                GetComponent<Rigidbody>().velocity = Aim - transform.position;
            }

            // 伤害
            if (CSkill.DamageType == SkillDamageType.UseCollider)
            {
                GetComponent<Collider>().enabled = true;
            }

            // 如果有连接技能，让SkillManager销毁
            if(!CSkill.IsHaveNextSkill) StartCoroutine(DelayDestroySelf());
        }

        /// <summary>
        /// 持续时间是否结束
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool IsEnd(float time)
        {
            return time > durationEndTime;
        }

        // 如果有碰撞触发器，一定是伤害检测用的
        private void OnTriggerEnter(Collider collision)
        {
            _context.DellDamage(Caster, collision.transform, CSkill.Damage);
            //TODO: buffs
        }

        IEnumerator DelayStop()
        {
            yield return new WaitForSeconds(0.2f); // TODO: 想办法改成效果结束时间
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            yield break;
        }

        IEnumerator DelayDestroySelf()
        {
            yield return new WaitForSeconds(CSkill.Duration);
            Destroy(gameObject);
            yield break;
        }
    }
}
