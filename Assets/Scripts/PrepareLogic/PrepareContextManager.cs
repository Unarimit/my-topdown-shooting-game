using Assets.Scripts.Common;
using Assets.Scripts.Common.EscMenu;
using Assets.Scripts.Entities;
using Assets.Scripts.PrepareLogic.PrepareEntities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.PrepareLogic
{
    public class PrepareContextManager : MonoBehaviour
    {
        // singleton
        public static PrepareContextManager Instance;

        // inspector
        [SerializeField]
        private SkillListConfig skillConfig;

        // prop
        public LevelInfo Level { get; private set; }
        public List<PrepareOperator> PrepareOps { get; private set; }
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");

        }
        public void Inject(LevelInfo level)
        {
            Level = level;
            dataInit();
        }
        private void dataInit()
        {
            PrepareOps = new List<PrepareOperator>();
            var ops = MyServices.Database.Operators;
            foreach(var op in ops)
            {
                PrepareOps.Add(new PrepareOperator(op));
            }
        }

        public Texture2D GetSkillIcon(int skillId)
        {
            return ResourceManager.Load<Texture2D>("Skills/" + skillConfig.CombatSkills[skillId].IconUrl);
        }

        public void GoToCombat()
        {
            Level.TeamOperators = new List<Operator>();
            foreach (var x in PrepareOps)
            {
                if (x.IsChoose) Level.TeamOperators.Add(x.OpInfo);
            }
            if(Level.TeamOperators.Count == 0)
            {
                TipsUI.GenerateNewTips("请至少选择一名干员");
                return;
            }
            SlideUI.CreateSlideUI();
            StartCoroutine(SceneLoadHelper.MyLoadSceneAsync("Playground"));
        }
        public void ReturnHome()
        {
            SlideUI.CreateSlideUI();
            StartCoroutine(SceneLoadHelper.MyLoadSceneAsync("Home"));
        }


        private bool isEscMenu = false;
        public void OnEscMenu(InputValue value)
        {
            if (isEscMenu) return;
            isEscMenu = true;
            var ui = EscMenuUI.OpenEscMenuUI();
            ui.ReturnBtn.onClick.AddListener(() =>
            {
                isEscMenu = false;
            });
        }
    }
}
