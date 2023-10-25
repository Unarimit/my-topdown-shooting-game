using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    public class BeamController : MonoBehaviour, IDelayWeaponController
    {
        public void DoDelayAction()
        {
            // move
            transform.position += transform.forward * 20;
            // 造成伤害（使用碰撞器检测
            
            // 延迟销毁
            StartCoroutine(DelayDestroySelf());
        }

        public void DoShootAction(Vector3 aim)
        {
            throw new NotImplementedException();
        }

        public bool IsTrigger()
        {
            throw new NotImplementedException();
        }

        IEnumerator DelayDestroySelf()
        {
            yield return new WaitForSeconds(2f); // TODO: 想办法改成效果结束时间
            Destroy(gameObject);
            yield break;
        }
    }
}
