using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CombatLogic.GOAPs.Builders
{
    internal class GOAPGraphBuilder
    {
        GOAPGraph Graph;
        /// <summary>
        /// 创建GOAPGraph，设置name是为了debug方便
        /// </summary>
        public GOAPGraphBuilder(string name)
        {
            Graph = new GOAPGraph(name);
        }

        /// <summary>
        /// 设置name是为了debug方便
        /// </summary>
        public GOAPActionBuilder AddAction(string name)
        {
            var res = new GOAPActionBuilder(Graph, name);

            return res;
        }

        public GOAPGraph BuildGraph()
        {
            return Graph;
        }
    }
}
