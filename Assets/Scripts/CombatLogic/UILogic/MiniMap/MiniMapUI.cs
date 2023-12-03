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
        private void Start()
        {
            setPlayer();
        }
        private void setPlayer()
        {
            bl_MiniMap.Target = _context.CombatVM.PlayerTrans;
        }
    }
}
