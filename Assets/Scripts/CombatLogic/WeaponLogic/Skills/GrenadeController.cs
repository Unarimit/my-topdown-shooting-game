using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    /// <summary>
    /// 控制爆炸效果的演出
    /// </summary>
    public class GrenadeController : MonoBehaviour, IDelayWeaponController
    {
        public ParticleSystem Explosion;
        public GameObject SmallRed;
        private bool isTrigger = false;
        public void DoDelayAction()
        {
            isTrigger = true;
            transform.eulerAngles = Vector3.zero;
            Explosion.Play();
            Destroy(SmallRed);
            StartCoroutine(DelayDestroySelf());
        }

        public void DoShootAction(Vector3 aim)
        {
            throw new NotImplementedException();
        }

        public bool IsTrigger()
        {
            return isTrigger;
        }

        IEnumerator DelayDestroySelf()
        {
            yield return new WaitForSeconds(3f); // TODO: 想办法改成效果结束时间
            Destroy(transform.gameObject);
            yield break;
        }
        
    }
}
