using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic.EffectLogic
{
    internal class EditRoomDisplayAnimationController : MonoBehaviour
    {
        Animator _animator;
        private int _animIDKick;
        private int _animIDWaving;
        private int _animIDIdle1;
        private int _animIDIdle2;
        private int _animIDIdle3;
        private List<int> _animIDs;
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _animIDKick = Animator.StringToHash("Kick");
            _animIDWaving = Animator.StringToHash("Waving");
            _animIDIdle1 = Animator.StringToHash("Idle1");
            _animIDIdle2 = Animator.StringToHash("Idle2");
            _animIDIdle3 = Animator.StringToHash("Idle3");
            _animIDs = new List<int> { _animIDKick, _animIDWaving, _animIDIdle1, _animIDIdle2, _animIDIdle3 };

        }

        private float time = 2;
        private const float TIME = 5;
        private void Update()
        {
            time -= Time.deltaTime;
            if(time < 0)
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    triggerAMotion();
                    time = TIME;
                }
            }
        }

        private void triggerAMotion()
        {

            int tr = _animIDs[Random.Range(0, _animIDs.Count)];
            _animator.SetTrigger(tr);
        }
    }
}
