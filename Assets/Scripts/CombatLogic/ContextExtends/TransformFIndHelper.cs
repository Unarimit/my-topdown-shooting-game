using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.ContextExtends
{
    static internal class TransformFIndHelper
    {
        public static List<Transform> FindOpTransform(this CombatContextManager context, Func<Transform, bool> condition)
        {
            var ret = new List<Transform>();
            foreach(var x in context.Operators.Keys)
            {
                if (condition(x)) ret.Add(x);
            }
            return ret;
        }
    }
}
