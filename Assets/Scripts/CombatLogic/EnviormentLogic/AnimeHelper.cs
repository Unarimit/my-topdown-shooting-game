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
        public static AnimeHelper Instance;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }

        public void ApplyRagdoll(Transform transform)
        {
            var prefab = Resources.Load<GameObject>("Effects/RagDoll");
            var go = Instantiate(prefab, CombatContextManager.Instance.Enviorment);
            go.transform.position = transform.position;
            go.transform.rotation = transform.rotation;
        }
        private Dictionary<Transform, DamageNumEffectController> hitTextDic = new Dictionary<Transform, DamageNumEffectController>();
        public void DamageTextEffect(int dmg, Transform hitted)
        {
            if(hitTextDic.ContainsKey(hitted) && hitTextDic[hitted] != null && hitTextDic[hitted].InDestroy == false)
            {
                hitTextDic[hitted].AppendDamageNum(dmg);
                hitTextDic[hitted].transform.position = hitted.position + new Vector3(0, 1.5f, 0);
            }
            else
            {
                var prefab = Resources.Load<GameObject>("Effects/DamageTextEffect");
                var go = Instantiate(prefab, CombatContextManager.Instance.Enviorment);
                hitTextDic[hitted] = go.GetComponent<DamageNumEffectController>();
                hitTextDic[hitted].DamageNum = dmg;

                go.transform.position = hitted.position + new Vector3(0, 1.5f, 0);
            }
            
        }
    }
}
