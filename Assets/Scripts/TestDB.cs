﻿using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Mechas;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    internal static class TestDB
    {
        #region 常量配置
        public static readonly int CHARACTER_LAYER = LayerMask.NameToLayer("Character");
        public static readonly int DOBJECT_LAYER = LayerMask.NameToLayer("DestructibleObject");
        public enum SkillSelectorStr
        {
            Trigger
        }
        #endregion

        public static LevelInfo Level { get; set; }

        public static List<Operator> GetOperators()
        {
            // TODO: these just for test
            return new List<Operator>() {
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino", WeaponSkillId = 4 },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", Type = OperatorType.CV, 
                    WeaponSkillId = 6,
                    Fighters = new List<Fighter>{ 
                        new Fighter { Operator = new Operator { Name = "ho", ModelResourceUrl = "Hoshino" } },
                        new Fighter { Operator = new Operator { Name = "shi", ModelResourceUrl = "Shiroko" } }
                    } 
                },

                new Operator { Name = "aru", ModelResourceUrl = "Aru",  },
                new Operator { Name = "akrin", ModelResourceUrl = "Karin",  },
                new Operator { Name = "mashiro", ModelResourceUrl = "Mashiro",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino",  },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko",  },
            };
        }

        public static List<MechaBase> GetMechas()
        {
            return new List<MechaBase>
            {
                MechaBody.DefaultMecha(),
                MechaHead.DefaultMecha(),
                MechaLeg.DefaultMecha()
            };
        }

        static List<string> clist = new List<string> { "Hoshino", "Shiroko", "Aru", "Karin", "Mashiro"};
        public static Operator GetRandomCA()
        {
            return new Operator
            {
                Name = "random test",
                ModelResourceUrl = clist[Random.Range(0, clist.Count)],
                WeaponSkillId = 4,
                Type = OperatorType.CA
            };
        }
        public static Operator GetRandomCV()
        {
            return new Operator
            {
                Name = "random test",
                ModelResourceUrl = clist[Random.Range(0, clist.Count)],
                WeaponSkillId = 6,
                Type = OperatorType.CV,
                Fighters = new List<Fighter>{
                    new Fighter { Operator = new Operator { Name = "r1", ModelResourceUrl = clist[Random.Range(0, clist.Count)] } },
                    new Fighter { Operator = new Operator { Name = "r2", ModelResourceUrl = clist[Random.Range(0, clist.Count)] } }
                }
            };
        }
        public static List<Operator> GetRandomOperator(int t)
        {
            var ans = new List<Operator>(t);
            for(int i = 0; i < t; i++) ans.Add(GetRandomCA());

            return ans;
        }
    }
}
