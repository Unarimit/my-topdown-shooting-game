using Assets.Scripts.CombatLogic.Skill.Releaser;
using Assets.Scripts.Entities;
using Assets.Scripts.Services;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts.CombatLogic
{
    public class SkillManager : MonoBehaviour
    {
        //***************** inspector *********************

        public SkillListConfig skillConfig;

        //***************** inspector end *****************

        public static SkillManager Instance;
        private CombatContextManager _context => CombatContextManager.Instance;
        private Dictionary<int, CombatSkill> skills;
        private Dictionary<CombatSkill, ObjectPool<RangeReleaser>> rangeReleaserPool = new Dictionary<CombatSkill, ObjectPool<RangeReleaser>>();
        private Dictionary<MeeleReleaserPoolIndex, ObjectPool<MeleeReleaser>> meleeReleaserPool = new Dictionary<MeeleReleaserPoolIndex, ObjectPool<MeleeReleaser>>();

        public static bool IsValidCollision(Collider collision)
        {
            return collision.gameObject.layer == MyConfig.CHARACTER_LAYER || collision.gameObject.layer == MyConfig.DOBJECT_LAYER;
        }
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }
        public void Init()
        {
            skills = new Dictionary<int, CombatSkill>();
            foreach(var x in skillConfig.CombatSkills)
            {
                skills[x.Id] = x;
            }
        }

        /// <summary>
        /// 释放技能，在调用此函数前，检查冷却
        /// </summary>
        public void CastSkill(Transform Caster, CombatSkill skill, Vector3 aim, Vector3 startPos, Vector3 startAngle)
        {
            // 配置 releaser
            BaseReleaser releaser = null;
            if (skill.ReleaserType == SkillReleaserType.Melee)
            {
                releaser = getMeeleReleaser(Caster, skill);
            }
            else if(skill.ReleaserType == SkillReleaserType.Range)
            {
                releaser = getRangeReleaser(skill);
                var skillGo = releaser.gameObject;
                skillGo.transform.position = startPos;
                skillGo.transform.eulerAngles = startAngle;
                skillGo.SetActive(true);
            }
            releaser.Release(this, Caster, skill, aim);
            // 监听连锁技能事件
            if (skill.IsHaveNextSkill) releaser.TriggerChainSkillEventHandler += CastSkill;
        }

        /// <summary>
        /// 技能释放完成，调用终止逻辑，在里面进行回收工作
        /// </summary>
        /// <param name="releaser"></param>
        internal void FinalizerSkill(BaseReleaser releaser)
        {
            if(releaser is MeleeReleaser)
            {
                meleeReleaserPool[new MeeleReleaserPoolIndex { Caster = releaser.Caster, Skill = releaser.Skill }].Release(releaser as MeleeReleaser);
            }
            else if(releaser is RangeReleaser)
            {
                rangeReleaserPool[releaser.Skill].Release(releaser as RangeReleaser);
            }
        }
        class MeeleReleaserPoolIndex
        {
            public Transform Caster;
            public CombatSkill Skill;

            public override bool Equals(object obj)
            {
                var t = obj as MeeleReleaserPoolIndex;
                return t.Caster == this.Caster && t.Skill == this.Skill;
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(Caster, Skill);
            }
        }
        private MeleeReleaser getMeeleReleaser(Transform caster, CombatSkill skill)
        {
            var index = new MeeleReleaserPoolIndex { Caster = caster, Skill = skill };
            if (meleeReleaserPool.ContainsKey(index) is false)
            {
                meleeReleaserPool.Add(index, new ObjectPool<MeleeReleaser>(
                        () => // createfunc
                        {
                            return caster.AddComponent<MeleeReleaser>();
                        },
                        (melee) => // actionOnGet
                        {
                            melee.enabled = true;
                        },
                        (melee) => // actionOnRelease
                        {
                            melee.enabled = false;
                        },
                        (melee) =>// actionOnDestroy
                        {
                            Destroy(melee);
                        }
                    ));
            }
            return meleeReleaserPool[index].Get();
        }
        private RangeReleaser getRangeReleaser(CombatSkill skill)
        {
            if (rangeReleaserPool.ContainsKey(skill) is false)
            {
                rangeReleaserPool.Add(skill, new ObjectPool<RangeReleaser>(
                        () => // createfunc
                        {
                            var prefab = ResourceManager.Load<GameObject>("Skills/" + skill.PrefabResourceUrl);
                            var skillGo = Instantiate(prefab, _context.Enviorment);
                            return skillGo.AddComponent<RangeReleaser>();
                        },
                        (range) => // actionOnGet
                        {
                            range.gameObject.SetActive(true);
                        },
                        (range) => // actionOnRelease
                        {
                            range.gameObject.SetActive(false);
                        },
                        (range) =>// actionOnDestroy
                        {
                            Destroy(range.gameObject);
                        }
                    ));
            }
            return rangeReleaserPool[skill].Get();
        }
    }
}
