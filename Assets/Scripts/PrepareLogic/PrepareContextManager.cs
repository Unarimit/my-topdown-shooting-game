using Assets.Scripts.Common;
using Assets.Scripts.Common.EscMenu;
using Assets.Scripts.Entities;
using Assets.Scripts.PrepareLogic.EffectLogic;
using Assets.Scripts.PrepareLogic.PrepareEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.PrepareLogic
{
    public class PrepareContextManager : MonoBehaviour
    {
        public static PrepareContextManager Instance;

        [SerializeField]
        private SkillListConfig skillConfig;

        public List<PrepareOperator> data;
        public LevelInfo Level;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");

            dataInit();
        }

        private void dataInit()
        {
            data = new List<PrepareOperator>();
            var ops = TestDB.GetOperators();
            foreach(var op in ops)
            {
                data.Add(new PrepareOperator(op));
            }
        }

        public Texture2D GetSkillIcon(int skillId)
        {
            return ResourceManager.Load<Texture2D>("Skills/" + skillConfig.CombatSkills[skillId].IconUrl);
        }

        public void GoToCombat()
        {
            if(TestDB.Level == null)
            {
                Debug.LogError("error: Db levelinfo is null");
            }
            else
            {
                TestDB.Level.TeamOperators = new List<Operator>();
                foreach (var x in data)
                {
                    if (x.IsChoose) TestDB.Level.TeamOperators.Add(x.OpInfo);
                }
                if(TestDB.Level.TeamOperators.Count == 0)
                {
                    TipsUI.GenerateNewTips("请至少选择一名干员");
                    return;
                }
                else
                {
                    foreach (var x in TestDB.Level.TeamOperators)
                        ResourceManager.AddModelHeadIcon(x.ModelResourceUrl, PhotographyManager.Instance.GetCharacterHeadIcon(x.ModelResourceUrl));
                }
                SlideUI.CreateSlideUI();
                SceneManager.LoadScene("Playground");
            }
        }
        public void ReturnHome()
        {
            SlideUI.CreateSlideUI();
            SceneManager.LoadScene("Home");
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
