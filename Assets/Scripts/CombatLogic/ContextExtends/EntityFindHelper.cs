using Assets.Scripts.CombatLogic.CombatEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.ContextExtends
{
    internal static class EntityFindHelper
    {
        /// <summary>
        /// 根据条件返回CombatOperator
        /// </summary>
        /// <param name="context"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static List<CombatOperator> FindCombatOperators(this CombatContextManager context, Func<CombatOperator, bool> condition)
        {
            var ret = new List<CombatOperator>();
            foreach (var x in context.Operators.Values)
            {
                if (condition(x)) ret.Add(x);
            }
            return ret;
        }
    }
}
