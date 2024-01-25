using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Assets.Scripts.CombatLogic.GOAPs.JobVersion
{
    internal static class JobHelper
    {
        public static UnsafeList<T> ToUnsafeList<T>(this IList<T> source) where T : unmanaged
        {
            var res = new UnsafeList<T>(source.Count, Allocator.Persistent);
            foreach(var x in source) res.Add(x);
            return res;
        }
    }
}
