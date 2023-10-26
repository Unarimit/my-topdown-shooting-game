using Assets.Scripts.ComputerControllers;
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
        private DelayWeapon _entity; 

        private CombatContextManager _context;

        public void Start()
        {
            _context = CombatContextManager.Instance;
        }

        public void DoDelayAction()
        {
            isTrigger = true;
            transform.eulerAngles = Vector3.zero; // 方便播放动画，不然动画会边转边播

            // 造成伤害（使用碰撞器检测
            GetComponent<SphereCollider>().enabled = true;

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

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.transform == _entity.Caster) return;
            _context.DellDamage(_entity.Caster, collision.transform, _entity.Damage);
        }

        IEnumerator DelayDestroySelf()
        {
            yield return new WaitForSeconds(3f); // TODO: 想办法改成效果结束时间
            Destroy(transform.gameObject);
            yield break;
        }

        public void SetDalayWeaponEntity(DelayWeapon entity)
        {
            _entity = entity;
        }

    }
}
