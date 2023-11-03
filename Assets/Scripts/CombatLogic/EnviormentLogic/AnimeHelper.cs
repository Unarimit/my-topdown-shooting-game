using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    public class AnimeHelper : MonoBehaviour
    {
        public static void ApplyRagdoll(Transform transform)
        {
            var prefab = Resources.Load<GameObject>("Effects/RagDoll");
            var go = Instantiate(prefab, CombatContextManager.Instance.Enviorment);
            go.transform.position = transform.position;
            go.transform.rotation = transform.rotation;
        } 
    }
}
