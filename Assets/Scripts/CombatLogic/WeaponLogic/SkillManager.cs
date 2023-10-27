using Assets.Scripts.ComputerControllers;
using Assets.Scripts.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private HashSet<SkillController> TriggeringSkills;
        private CombatContextManager _context;
        private Dictionary<int, CombatSkill> skills;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }
        private void Start()
        {
            _context = CombatContextManager.Instance;
            TriggeringSkills = new HashSet<SkillController>();
            skills = new Dictionary<int, CombatSkill>();
            foreach(var x in skillConfig.CombatSkills)
            {
                skills[x.Id] = x;
            }

        }
        private void FixedUpdate()
        {
            TriggerSkill();
        }

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        public void CastSkill(Transform Caster, CombatSkill skill, Vector3 aim)
        {
            CastSkill2(Caster, skill, aim, Caster.position + new Vector3(0, 0.5f, 0), Caster.eulerAngles);
        }

        private void CastSkill2(Transform Caster, CombatSkill skill, Vector3 aim, Vector3 position , Vector3 angles)
        {
            // 初始化技能prefab
            var prefab = Resources.Load<GameObject>("Skills/" + skill.PrefabResourceUrl);
            var skillGo = Instantiate(prefab, _context.Enviorment);
            skillGo.transform.position = position;
            skillGo.transform.eulerAngles = angles;

            // 初始化技能控制器
            var controller = skillGo.GetComponent<SkillController>();
            controller.Aim = aim;
            controller.CSkill = skill;
            controller.Caster = Caster;

            // 发射技能
            controller.Cast(Time.time);
            if (skill.IsHaveNextSkill) TriggeringSkills.Add(controller);
        }

        /// <summary>
        /// 触发链式技能
        /// </summary>
        private void TriggerSkill()
        {
            var keysToTrigger = new List<SkillController>();
            foreach (var x in TriggeringSkills)
            {
                if (x.IsEnd(Time.time))
                {
                    keysToTrigger.Add(x);
                }
            }
            foreach (var x in keysToTrigger)
            {
                CastSkill2(x.Caster, skills[x.CSkill.NextSkillId], x.Aim, x.transform.position, new Vector3(0, 0, 0));
                TriggeringSkills.Remove(x);
                Destroy(x.gameObject);
            }
        }
    }
}
