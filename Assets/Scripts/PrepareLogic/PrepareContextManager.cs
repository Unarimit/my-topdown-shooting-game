using Assets.Scripts.Entities;
using Assets.Scripts.PrepareLogic.PrepareEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic
{
    public class PrepareContextManager : MonoBehaviour
    {
        public static PrepareContextManager Instance;

        [SerializeField]
        private SkillListConfig skillConfig;

        public List<PrepareOperator> data;
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
    }
}
