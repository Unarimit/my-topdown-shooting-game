using Assets.Scripts.Entities;
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
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }

        public List<Operator> GetOperators()
        {
            // TODO: these just for test
            return new List<Operator>() { 
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko" },
            };
        }
    }
}
