using Assets.Scripts.CombatLogic.Skill.Releaser;
using Assets.Scripts.Entities;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

        public static bool IsValidCollision(Collider collision)
        {
            return collision.gameObject.layer == TestDB.CHARACTER_LAYER || collision.gameObject.layer == TestDB.DOBJECT_LAYER;
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
                releaser = Caster.AddComponent<MeleeReleaser>();
            }
            else if(skill.ReleaserType == SkillReleaserType.Range)
            {
                var prefab = ResourceManager.Load<GameObject>("Skills/" + skill.PrefabResourceUrl);
                var skillGo = Instantiate(prefab, _context.Enviorment);
                skillGo.transform.position = startPos;
                skillGo.transform.eulerAngles = startAngle;
                skillGo.SetActive(true);
                releaser = skillGo.AddComponent<RangeReleaser>();
            }
            releaser.Release(Caster, skill, aim);

            // 监听连锁技能事件
            if(skill.IsHaveNextSkill) releaser.TriggerChainSkillEventHandler += CastSkill;
        }

    }
}
