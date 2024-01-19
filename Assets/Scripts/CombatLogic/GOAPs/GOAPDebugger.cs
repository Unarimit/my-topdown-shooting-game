using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.GOAPs
{
    public class GOAPDebugger : MonoBehaviour
    {
        public static GOAPDebugger Instance { get; set; }

        private void Awake()
        {
            ActionsResult = "not init";
#if UNITY_EDITOR
            Instance = this;
#endif
        }
        public string ActionsResult { get; private set; }
        Dictionary<string, string> plansDic = new Dictionary<string, string>();
        private void Update()
        {
            var sb = new StringBuilder();
            foreach(var val in plansDic.Values)
                sb.Append($"{val}\n");
            ActionsResult = sb.ToString();
        }

        internal void PrintActions(string graphName, List<GOAPAction> actions)
        {
            plansDic[graphName] = printActions(graphName, actions);
        }


        private string printActions(string graphName, List<GOAPAction> actions)
        {
            var sb = new StringBuilder();
            sb.Append($"GOAP Graph:'{graphName}' Planed: ");
            foreach (var x in actions)
            {
                sb.Append($"{x.ActionName},");
            }
            return sb.ToString();
        }

    }
}
