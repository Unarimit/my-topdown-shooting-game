using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.EnviormentLogic
{
    public class RagdollController : MonoBehaviour
    {
        private void Start()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                var go = transform.GetChild(i).gameObject;
                if(go.name == "bomb")
                {
                    go.transform.position += new Vector3(0, 0, (float)(Math.Sin((double)Time.time) / 4));
                    Debug.Log((float)(Math.Sin((double)Time.time) / 4));
                    StartCoroutine(DelayDestory(go));
                }
                else
                {
                    //StartCoroutine(DelayCloseCollider(go.transform));
                }
                
            }
            StartCoroutine(DelayDestorySelf());
        }

        private IEnumerator DelayCloseCollider(Transform t)
        {
            yield return new WaitForSeconds(1f);
            t.GetComponent<Collider>().enabled = false;
        }
        private IEnumerator DelayDestory(GameObject go)
        {
            yield return new WaitForSeconds(0.1f);
            Destroy(go);
        }
        private IEnumerator DelayDestorySelf()
        {
            yield return new WaitForSeconds(3f);
            Destroy(gameObject);
        }
    }
}
