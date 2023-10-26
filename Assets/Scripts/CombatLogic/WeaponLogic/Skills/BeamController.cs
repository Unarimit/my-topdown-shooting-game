using Assets.Scripts.ComputerControllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Assets.Scripts.CombatLogic
{
    public class BeamController : MonoBehaviour, IDelayWeaponController
    {
        private DelayWeapon _entity;
        private CombatContextManager _context;

        public void Start()
        {
            _context = CombatContextManager.Instance;
        }
        public void DoDelayAction()
        {
            // move
            GetComponent<Rigidbody>().velocity = transform.forward * 20 / 0.2f;

            // 造成伤害（使用碰撞器检测
            GetComponent<SphereCollider>().enabled = true;

            // 垂直发射
            StartCoroutine(DelayStop());
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

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.transform == _entity.Caster) return;
            _context.DellDamage(_entity.Caster, collision.transform, _entity.Damage);
        }

        IEnumerator DelayStop()
        {
            yield return new WaitForSeconds(0.2f); // TODO: 想办法改成效果结束时间
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            yield break;
        }
        IEnumerator DelayDestroySelf()
        {
            yield return new WaitForSeconds(1f); // TODO: 想办法改成效果结束时间
            Destroy(gameObject);
            yield break;
        }

        public void SetDalayWeaponEntity(DelayWeapon entity)
        {
            _entity = entity;
        }
    }
}
