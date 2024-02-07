using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.Entities.HomeMessage;
using Assets.Scripts.Entities.Level;
using Assets.Scripts.Entities.Mechas;
using Assets.Scripts.Entities.Save;
using Assets.Scripts.Services.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.Services.MyConfig;

namespace Assets.Scripts.Services
{
    internal class TestDatabase : IGameDatabase
    {
        public IList<SaveAbstract> SaveAbstracts { get; private set; }

        public IList<Operator> Operators { get; private set; }
        public IList<MechaBase> Mechas { get; private set; }
        public IDictionary<string, int> Inventory { get; private set; }
        public CombatLevelInfo CurCombatLevelInfo { get; set; }
        public BuildingArea BuildingArea { get; private set; }
        public HomeMessageQueue HomeMessages { get; }
        public bool OnNewDay { get; set; } = true;

        public IList<LevelRule> LevelRules { get; private set; }
        public IDictionary<string, Building> Buildings { get; private set; }
        public List<string> ModelList { get; private set; }
        public IList<CombatSkill> CombatSkills { get; private set; }

        public TestDatabase()
        {
            // saves
            SaveAbstracts = getTestSaves();
            // config
            ModelList = GameConfigInitHelper.GetConfigModelList();
            LevelRules = GameConfigInitHelper.GetConfigLevels();
            Buildings = GameConfigInitHelper.GetConfigBuildings();
            CombatSkills = GameConfigInitHelper.GetConfigCombatSkills();
            // savedata
            Inventory = GameConfigInitHelper.GetConfigInitInventory();
            Operators = generateTestOperators();
            Mechas = generateTestMechas();
            BuildingArea = GameConfigInitHelper.GetConfigInitBuildingArea();
            HomeMessages = GameConfigInitHelper.GetConfigInitHomeMessages();

            // databind
            registerDatabind();
            Operators[0].McHead = (MechaHead)Mechas[0];
            Operators[0].McBody = (MechaBody)Mechas[1];
            Operators[0].McLeg = (MechaLeg)Mechas[2];
        }

        

        public CombatLevelRule GetInvasionLevel()
        {
            return (CombatLevelRule)LevelRules[^1];
        }


        int opId = 0;

        private List<Operator> generateTestOperators()
        {
            // TODO: these just for test
            return new List<Operator>() {
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino", WeaponSkillId = 3, Id = (++opId).ToString(), MainSkillId = 9 },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", Type = OperatorType.CV,
                    WeaponSkillId = 6,
                    Fighters = new List<Fighter>{
                        new Fighter { Operator = new Operator { Name = "ho", ModelResourceUrl = "Hoshino", Id = (++opId).ToString() } },
                        new Fighter { Operator = new Operator { Name = "shi", ModelResourceUrl = "Shiroko", Id = (++opId).ToString() } }
                    },
                    Id = (++opId).ToString()
                },
                new Operator { Name = "aru", ModelResourceUrl = "Aru",Id = (++opId).ToString(), Trait = OperatorTrait.Tactical },
                new Operator { Name = "akrin", ModelResourceUrl = "Karin", Id = (++opId).ToString() },
                new Operator { Name = "mashiro", ModelResourceUrl = "Mashiro",Id = (++opId).ToString()  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", Id = (++opId).ToString() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", Id = (++opId).ToString()},
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", Id = (++opId).ToString() },
            };
        }

        int mechaId = 0;
        private List<MechaBase> generateTestMechas()
        {
            return new List<MechaBase>
            {
                new MechaHead("Head II", "head2", 10, 10, ++mechaId),
                new MechaBody("Body II", "body2", 15, 2, ++mechaId),
                new MechaLeg(name: "Leg II", "leg2", 4, 10, ++mechaId),
                new MechaHead("Head II", "head2", 10, 10, ++mechaId),
                new MechaBody("Body II", "body2", 15, 2, ++mechaId),
                new MechaLeg(name: "Leg II", "leg2", 4, 10, ++mechaId),
            };
        }
        

        /// <summary>
        /// 注册数据绑定，面向1对1或1对多关系
        /// </summary>
        private void registerDatabind()
        {
            foreach (var op in Operators)
            {
                op.MechaChangeEventHandler += opMechaChangeEventHandler;
            }
        }

        private void opMechaChangeEventHandler(Operator @this, MechaBase oldMehca, MechaBase newMehca)
        {
            if (oldMehca != null && oldMehca.IsDefaultMecha() is false) oldMehca.DataBind(null);
            newMehca.DataBind(@this);
        }


        private List<SaveAbstract> getTestSaves()
        {
            return new List<SaveAbstract>
            {
                new SaveAbstract{
                    SaveName = "测试存档1",
                    SaveTime = DateTime.Now,
                    SaveDesc = "day0, 家园",
                    SaveId = "test123",
                    SaveDay = 0,
                    Resource = new Produce[]{
                        new Produce{ ItemId = ItemTable.Red.ToString(), Amount = 100 },
                        new Produce{ ItemId = ItemTable.Ammo.ToString(), Amount = 1000 },
                        new Produce{ ItemId = ItemTable.Iron.ToString(), Amount = 1000 },
                        new Produce{ ItemId = ItemTable.Al.ToString(), Amount = 1000 },
                    }
                }
            };
        }


        public void NewGame()
        {
            // do nothing
            Debug.Log($"generate new data 在TestDatabase中不会真的生成新档案");

        }
        public void LoadSaveData(string saveId)
        {
            // do nothing
            Debug.Log($"try load data:{saveId} 在TestDatabase中不会真的读取存档");
        }
        public void ReplaceAndSaveData(string saveId)
        {
            throw new NotImplementedException();
        }
        public bool SaveData()
        {
            Debug.Log($"try save new data, generate DataAbstract 在TestDatabase中不会真的保存存档");
            SaveAbstracts.Add(Entities.Save.SaveData.GenerateSaveData(this).GenerateAbstract(DateTime.Now.ToString()));
            return true;
        }

        public void LuaAddLevelRule(LevelRule rule)
        {
            if(LevelRules.Where(x => x.LevelId == rule.LevelId).Count() != 0)
            {
                Debug.LogWarning($"have same level id: {rule.LevelId}");
                return;
            }
            LevelRules.Add(rule);
        }
    }
}
