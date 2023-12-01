using Assets.Scripts.CombatLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BulletLogic
{
    public class DestructiblePersonController : MonoBehaviour
    {
        private CombatContextManager _context;

        // prefab in character
        private ParticleSystem Shield;
        private void Start()
        {
            _context = CombatContextManager.Instance;

            // init prefab
            // shield
            var s_prefab = ResourceManager.Load<GameObject>("Effects/Shield");
            Shield = Instantiate(s_prefab, transform).GetComponent<ParticleSystem>();
            Shield.Stop();

        }
        public void DoDied()
        {
            //HP0Event.Invoke(transform);
        }
        public void GotDMG()
        {
            Shield.startColor = new Color(1, 1, 1) *
                (float)_context.GetOperatorCurrentHP(transform) / _context.GetOperatorMaxHP(transform);
            Shield.Simulate(1.0f);
            Shield.Play();
            if (_context.GetOperatorCurrentHP(transform) == 2) Shield.Stop();
        }

    }
}
