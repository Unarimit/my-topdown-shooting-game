using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    public enum CockpitWalkState
    {
        Idle,
        Walking,
        Running
    }
    public class CockpitManager : MonoBehaviour
    {
        public Camera Cam;
        public Transform Cockpit;

        public GameObject BlueDisplay;
        public GameObject RedDisplay;

        public Transform CockpitPos;
        public Animator CharacterAnimator { get; private set; }

        // ************ inspector end **********************

        private Vector3 cockpitInitPos;

        private CockpitWalkState _state;

        public static CockpitManager Instance;

        int _animIDAngry;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }

        private void Start()
        {
            Cam = Camera.main;
            cockpitInitPos = Cockpit.position; 
            CombatContextManager.Instance.PlayerDiedEvent += ChangeDiedStatu;
            _animIDAngry = Animator.StringToHash("Angry");
        }

        private void OnDestroy()
        {
            CombatContextManager.Instance.PlayerDiedEvent -= ChangeDiedStatu;
        }


        private void Update()
        {
            if(_state != CockpitWalkState.Idle) QuakeSimulate();
        }


        public void ChangeState(CockpitWalkState state)
        {
            _state = state;
            if (state == CockpitWalkState.Walking) _INTERVAL = 1;
            else if(state == CockpitWalkState.Running) _INTERVAL = 0.3f;
        }

        private void ChangeDiedStatu(object sender,bool isDied)
        {
            CharacterAnimator.SetBool(_animIDAngry, isDied);
            RedDisplay.SetActive(isDied);
            BlueDisplay.SetActive(!isDied);
        }



        private float _INTERVAL = 1f;
        private float _intervalClock = 0;
        /// <summary>
        /// 模拟机甲内部的周期震动
        /// </summary>
        private void QuakeSimulate()
        {
            _intervalClock += Time.deltaTime;
            if (_intervalClock > _INTERVAL) _intervalClock -= _INTERVAL;
            if ( _intervalClock < _INTERVAL / 2)
            {
                // normalize [0, 1]
                float normal = _intervalClock / (_INTERVAL / 2);
                // cal
                Cockpit.position = new Vector3(cockpitInitPos.x, cockpitInitPos.y + 0.05f * Mathf.Sin(normal * Mathf.PI), cockpitInitPos.z);
            }
            else// if (_intervalClock < _INTERVAL)
            {
                //pass
            }
        }
        void createDisplayer()
        {
            var prefab = ResourceManager.Load<GameObject>("Characters/CockpitDisplayer");
            var go = Instantiate(prefab, CockpitPos);
            CharacterAnimator = go.GetComponent<Animator>();
        }
        internal void ResetAnimator()
        {
            if(CharacterAnimator != null)
            {
                CharacterAnimator.gameObject.SetActive(false);
                Destroy(CharacterAnimator.gameObject);
            }
            createDisplayer();
        }
    }
}
