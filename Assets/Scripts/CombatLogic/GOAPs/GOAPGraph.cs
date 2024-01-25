using Assets.Scripts.CombatLogic.GOAPs.JobVersion;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.GOAPs
{
    /// <summary>
    /// GOAP点的集合
    /// </summary>
    internal class GOAPGraph
    {
        public string Name { get; set;}
        public List<GOAPAction> Actions = new List<GOAPAction>();

        public GOAPGraph(string name)
        {
            Name = name;
        }
        public GOAPGraphPro ToPro()
        {
            return new GOAPGraphPro
            {
                Actions = new Unity.Collections.NativeArray<GOAPAction>(Actions.ToArray(), Unity.Collections.Allocator.Persistent)
            };
        }
        public List<GOAPAction> DoPlan(uint initState)
        {
            return GOAPGraphHelper.DoPlanForActions(initState, Actions);
        }
    }
    internal static class GOAPGraphHelper 
    { 
        public static List<GOAPAction> DoPlanForActions(uint initState, IEnumerable<GOAPAction> Actions)
        {
            var res = new List<GOAPAction>();
            int actionsLength = Actions.Count();

            // use bfs and visit
            var queue = new Queue<uint>();

            // 状态(uint)的下一个最优状态是(uint)，代价是(float)，通过哪个动作id(int)
            var trace = new Dictionary<uint, (uint, float, int)>();
            var dpCost = new Dictionary<uint, float>();
            dpCost.Add(initState, 0);
            queue.Enqueue(initState);
            while (queue.Count > 0)
            {
                int len = queue.Count;
                for (int _ = 0; _ < len; _++)
                {
                    var s = queue.Dequeue();
                    // 正文
                    for (int i = 0; i < actionsLength; i++)
                    {
                        if (Actions.ElementAt(i).IsMatchCondtions(s))
                        {
                            var ns = Actions.ElementAt(i).Transfer(s); // next state
                            var cost = dpCost[s] + Actions.ElementAt(i).GetCost(s);
                            if (dpCost.ContainsKey(ns))
                            {
                                if (cost < dpCost[ns]) dpCost[ns] = cost;
                                else continue;
                            }
                            else dpCost.Add(ns, cost);

                            if (trace.ContainsKey(s))
                            {
                                if (cost < trace[s].Item2)
                                {
                                    trace[s] = (ns, cost, i);
                                    if (isGoal(ns) is false) queue.Enqueue(ns);
                                }
                            }
                            else
                            {
                                trace.Add(s, (ns, cost, i));
                                if (isGoal(ns) is false) queue.Enqueue(ns);
                            }
                        }
                    }
                }
            }

            var p = initState;
            while (isGoal(p) is false)
            {
                res.Add(Actions.ElementAt(trace[p].Item3));
                p = trace[p].Item1;
            }
            return res;
        }
        private static bool isGoal(uint state)
        {
            return (state & ((uint)1 << (int)GOAPStatus.Win)) != 0;
        }
    }
}
