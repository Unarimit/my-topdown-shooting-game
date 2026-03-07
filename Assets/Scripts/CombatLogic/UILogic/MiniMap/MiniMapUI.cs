using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.UILogic.MiniMap
{
    [RequireComponent(typeof(bl_MiniMap))]
    internal class MiniMapUI : SubUIBase
    {
        private bl_MiniMap bl_MiniMap;
        private void Awake()
        {
            bl_MiniMap = GetComponent<bl_MiniMap>();
            _context.CombatVM.PlayerChangeEvent += setPlayer;
        }

        public override void SetVisible(bool isVisible)
        {
            base.SetVisible(isVisible);
            if (isVisible) Start();

        }

        private void OnEnable()
        {
            StartCoroutine(waitToActive());
            IEnumerator waitToActive()
            {
                yield return null;
                bl_MiniMap.SetAsActiveMiniMap();
            }
        }
        private void Start()
        {
            StopCoroutine(StartCorotine());
            StartCoroutine(StartCorotine());
            IEnumerator StartCorotine()
            {
                if (_context.CombatVM.PlayerTrans != null) setPlayer();
                else
                {
                    while (_context.Operators.Any() is false)
                    {
                        yield return null;
                    }
                    bl_MiniMap.Target = _context.Operators.First().Key;
                }
            }   
        }
        private void setPlayer()
        {
            bl_MiniMap.Target = _context.CombatVM.PlayerTrans;
        }
    }
}
