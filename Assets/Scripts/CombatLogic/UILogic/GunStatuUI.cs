using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CombatLogic.UILogic
{
    public class GunStatuUI : SubUIBase
    {
        public TextMeshProUGUI MaxAmmoText;
        public TextMeshProUGUI CurrentAmmoText;
        public Image Gunshape;
        private readonly Color AmmoWarning = new Color(1, 0, 0);
        private readonly Color AmmoNormal = new Color(1, 1, 1);

        private void OnEnable()
        {
            _context.CombatVM.PlayerGunStatuChangeEvent += UpdateCurrentAmmo;
            if(_context.CombatVM.InvokePlayerGunStatuChangeEventLastMsg != null)
            {
                UpdateCurrentAmmo(_context.CombatVM.InvokePlayerGunStatuChangeEventLastMsg);
                _context.CombatVM.InvokePlayerGunStatuChangeEventLastMsg = null;
            }
        }
        private void OnDisable()
        {
            _context.CombatVM.PlayerGunStatuChangeEvent -= UpdateCurrentAmmo;
        }
        GunController _gun = null;
        /// <summary>
        /// 更新当前子弹数量，当数量小于最大子弹的1/5时，颜色变为红色
        /// </summary>
        internal void UpdateCurrentAmmo(GunController gun)
        {
            int cur = gun.gunProperty.CurrentAmmo;
            int max = gun.gunProperty.MaxAmmo;
            CurrentAmmoText.text = cur.ToString();
            if (max != 0 && max > cur * 5)
            {
                CurrentAmmoText.color = AmmoWarning;
            }
            else
            {
                CurrentAmmoText.color = AmmoNormal;
            }
            if(_gun == null || _gun != gun)
            {
                MaxAmmoText.text = gun.gunProperty.MaxAmmo.ToString();
                Gunshape.sprite = ResourceManager.Load<Sprite>($"Skills/{gun.Skill.SkillInfo.IconUrl}");
                _gun = gun;
            }
            
        }

        private Vector2 initPos;
        public override void TweenQuit(float duration)
        {
            var rect = GetComponent<RectTransform>();
            initPos = rect.anchoredPosition;
            rect.DOAnchorPos(rect.anchoredPosition + new Vector2(rect.rect.width, 0), duration)
                .OnComplete(() => base.TweenQuit(duration));
        }

        public override void TweenEnter(float duration)
        {
            base.TweenEnter(duration);
            var rect = GetComponent<RectTransform>();
            rect.DOAnchorPos(initPos, duration);
        }
    }
}
