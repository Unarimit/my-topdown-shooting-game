using Assets.Scripts.Common;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.Entities.HomeMessage;
using Assets.Scripts.Entities.Level;
using Assets.Scripts.Entities.Mechas;
using Assets.Scripts.Entities.Save;
using Assets.Scripts.HomeLogic;
using Assets.Scripts.Services.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;

namespace Assets.Scripts.Services
{
    internal class FileDatabase : IGameDatabase
    {
        // saves
        public IList<SaveAbstract> SaveAbstracts { get; private set; }

        // save 
        public IList<Operator> Operators { get; private set; }

        public IList<MechaBase> Mechas { get; private set; }

        public IDictionary<string, int> Inventory { get; private set; }

        public BuildingArea BuildingArea { get; private set; }

        public HomeMessageQueue HomeMessages { get; private set; }

        public CombatLevelInfo CurCombatLevelInfo { get; set; }
        public bool OnNewDay { get; set; }

        // config
        public IList<LevelRule> LevelRules { get; private set; }

        public IDictionary<string, Building> Buildings { get; private set; }

        public IList<CombatSkill> CombatSkills { get; private set; }

        public List<string> ModelList { get; private set; }

        /// <summary> 存储delegate </summary>
        private Dictionary<string, Action<HomeContextManager>> HomeEvents {get; set;}

        public FileDatabase()
        {
            // 加载配置信息
            ModelList = GameConfigInitHelper.GetConfigModelList();
            LevelRules = GameConfigInitHelper.GetConfigLevels();
            Buildings = GameConfigInitHelper.GetConfigBuildings();
            CombatSkills = GameConfigInitHelper.GetConfigCombatSkills();
            HomeEvents = GameConfigInitHelper.GetConfigHomeEvents();

            if (FileHelper.HasFile(FileHelper.SAVE_ABSTRACTS_FILENAME))
                SaveAbstracts = FileHelper.LoadFile<List<SaveAbstract>>(FileHelper.SAVE_ABSTRACTS_FILENAME);
            else
                SaveAbstracts = new List<SaveAbstract>();

            // 提前加载存档，为了展示角色
            if(SaveAbstracts.Count != 0)
            {
                LoadSaveData(SaveAbstracts[^1].SaveId);
            }
            else
            {
                NewGame();
            }
        }

        public CombatLevelRule GetInvasionLevel()
        {
            throw new NotImplementedException();
        }


        public void NewGame()
        {
            Inventory = GameConfigInitHelper.GetConfigInitInventory();
            Operators = new List<Operator>();
            Mechas = new List<MechaBase>();
            BuildingArea = GameConfigInitHelper.GetConfigInitBuildingArea();
            HomeMessages = GameConfigInitHelper.GetConfigInitHomeMessages(); 
            CurCombatLevelInfo = null;
            OnNewDay = false;

            // index
            registerDatabind();

        }
        public void LoadSaveData(string saveId)
        {
            clearOldBind();
            var save = FileHelper.LoadFile<Entities.Save.SaveData>(saveId);
            Inventory = save.Inventory;
            Operators = save.Operators; // 需要链接舰载机
            Mechas = save.Mechas; // 需要链接Operator
            BuildingArea = save.BuildingArea; // TODO：需要链接Operator管理员，先不做
            HomeMessages = new HomeMessageQueue(); // 需要链接事件
            HomeMessages.AddRange(save.HomeMessages);
            CurCombatLevelInfo = null;
            OnNewDay = false;

            // 处理Operators， 链接舰载机
            var tempOpDic = new Dictionary<string, Operator>();
            foreach (var x in Operators)
            {
                tempOpDic[x.Id] = x;
            }
            foreach (var x in Operators)
            {
                if(x.Fighters != null)
                {
                    foreach(var fi in x.Fighters)
                    {
                        if (fi.OperatorId == null) continue;

                        fi.Operator = tempOpDic[fi.OperatorId];
                    }
                }
            }

            // 处理Mechas， 链接Operator
            foreach (var x in Mechas)
            {
                if (x.OperatorId == null) continue;

                x.DataBind(tempOpDic[x.OperatorId]);
                if (x is MechaBody body) tempOpDic[x.OperatorId].McBody = body;
                else if (x is MechaHead head) tempOpDic[x.OperatorId].McHead = head;
                else if (x is MechaLeg leg) tempOpDic[x.OperatorId].McLeg = leg;
                else
                {
                    throw new Exception($"Serilized a unmatch type {x.GetType()}, cannot match type MechaBody, MechaHead and MechaLeg");
                }
            }

            // 处理HomeMessages
            foreach (var x in HomeMessages)
            {
                x.MessageAction = HomeEvents[x.MessageActionId];
            }
            registerDatabind();

        }
        public bool SaveData()
        {
            var save = Entities.Save.SaveData.GenerateSaveData(this);
            if (FileHelper.SaveFile(save, save.SaveId) is false) return false;
            SaveAbstracts.Add(save.GenerateAbstract(save.SaveId));
            if (FileHelper.SaveFile(SaveAbstracts, FileHelper.SAVE_ABSTRACTS_FILENAME, true) is false) return false;
            return true;

        }
        // 暂时不做
        public void ReplaceAndSaveData(string saveId)
        {
            throw new NotImplementedException();
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
        private void clearOldBind()
        {
            if (Operators == null) return;

            foreach (var op in Operators)
            {
                op.MechaChangeEventHandler -= opMechaChangeEventHandler;
            }
        }

        private void opMechaChangeEventHandler(Operator @this, MechaBase oldMehca, MechaBase newMehca)
        {
            if (oldMehca.IsDefaultMecha() is false) oldMehca.DataBind(null);
            newMehca.DataBind(@this);
        }
    }
}
