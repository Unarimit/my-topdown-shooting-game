using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Assets.Scripts.Entities;
using System;

namespace Assets.Scripts.Editor
{
    internal class GenerateSkillInfoByCsvFile : EditorWindow
    {
        private static readonly string ID = "Id";
        private static readonly string TYPE = "Type";
        private static readonly string NAME = "Name";
        private static readonly string DESC = "Description";
        private static readonly string RANGE_TIP = "RangeTip";
        private static readonly string TARGET_TIP = "TargetTip";
        private static readonly string COOL_DOWN = "CoolDown";
        private static readonly string DURATION = "Duration";
        private static readonly string AFTER_CAST_TIME = "AfterCastTime";
        private static readonly string CHARACTER_ANIME_ID = "CharacterAnimeId";
        private static readonly string RELESER_TYPE = "ReleaserType";
        private static readonly string SKILL_SELECTOR = "SkillSelector";
        private static readonly string EFFECT_TYPE = "EffectType";
        private static readonly string SKILL_IMPACTORS = "SkillImpactors";
        private static readonly string NEXT_SKILL_ID = "NextSkillId";
        private static readonly string AMMO = "Ammo";
        private static readonly string PREFAB_RESOURCE_URL = "PrefabResourceUrl";
        private static readonly string ICON_URL = "IconUrl";

        [MenuItem("Tools/Skill Import CSV")]
        private static void ImportCSV()
        {
            string filePath = EditorUtility.OpenFilePanel("Import CSV", "", "csv");
            if (!string.IsNullOrEmpty(filePath))
            {
                // 读取CSV文件内容
                string[] lines = File.ReadAllLines(filePath);

                // 获取ScriptableObject
                SkillListConfig myScriptableObject = AssetDatabase.LoadAssetAtPath<SkillListConfig>("Assets/Resources/SkillList.asset");


                // 处理CSV表头
                var dic = new Dictionary<string, int>();
                {
                    var head = lines[0].Split(',');
                    for (int i = 0; i < head.Length; i++)
                    {
                        dic.Add(head[i], i);
                    }
                }

                // 读取数据放入ScriptableObject，先清空
                myScriptableObject.CombatSkills.Clear();
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split(',');
                    var skill = new CombatSkill();

                    skill.Id = int.Parse(values[dic[ID]]);
                    skill.Type = Enum.Parse<SkillType>(values[dic[TYPE]]);
                    skill.Name = values[dic[NAME]];
                    skill.Description = values[dic[DESC]];
                    skill.RangeTip = float.Parse(values[dic[RANGE_TIP]]);
                    skill.TargetTip = Enum.Parse<SkillTargetTip>(values[dic[TARGET_TIP]]);
                    skill.CoolDown = float.Parse(values[dic[COOL_DOWN]]);
                    skill.Duration = float.Parse(values[dic[DURATION]]);
                    skill.AfterCastTime = float.Parse(values[dic[AFTER_CAST_TIME]]);
                    skill.CharacterAnimeId = values[dic[CHARACTER_ANIME_ID]];
                    skill.ReleaserType = Enum.Parse<SkillReleaserType>(values[dic[RELESER_TYPE]]);
                    var selector = values[dic[SKILL_SELECTOR]].Split(":");
                    skill.SkillSelector = new SkillSelector
                    {
                        SelectorName = selector[0],
                        Data = selector[1]
                    };
                    skill.EffectType = Enum.Parse<SkillEffectType>(values[dic[EFFECT_TYPE]]);
                    if(values[dic[SKILL_IMPACTORS]].Length != 0)
                    {
                        var impactors = values[dic[SKILL_IMPACTORS]].Trim().Split(";");
                        skill.SkillImpectors = new List<SkillImpactor>();
                        foreach (var x in impactors)
                        {
                            var im = x.Split(":");
                            skill.SkillImpectors.Add(new SkillImpactor
                            {
                                ImpectorName = im[0],
                                Data = im[1]
                            });
                        }
                    }
                    
                    skill.NextSkillId = int.Parse(values[dic[NEXT_SKILL_ID]]);
                    skill.Ammo = int.Parse(values[dic[AMMO]]);
                    skill.PrefabResourceUrl = values[dic[PREFAB_RESOURCE_URL]];
                    skill.IconUrl = values[dic[ICON_URL]];

                    myScriptableObject.CombatSkills.Add(skill);
                }

                // 保存ScriptableObject的修改
                EditorUtility.SetDirty(myScriptableObject);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("读取成功！");
            }
        }
    }
}
