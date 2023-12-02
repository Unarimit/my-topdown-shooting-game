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
            MaxAmmoText.text = _context.CombatVM.PlayerGun.gunProperty.MaxAmmo.ToString();
            CurrentAmmoText.text = _context.CombatVM.PlayerGun.gunProperty.CurrentAmmo.ToString();
            _context.CombatVM.PlayerGun.CurrentAmmoChangeEvent += UpdateCurrentAmmo;
            Gunshape.sprite = ResourceManager.Load<Sprite>($"Skills/{_context.CombatVM.PlayerGun.Skill.SkillInfo.IconUrl}");
        }
        private void OnDisable()
        {
            _context.CombatVM.PlayerGun.CurrentAmmoChangeEvent -= UpdateCurrentAmmo;
        }
        /// <summary>
        /// 更新当前子弹数量，当数量小于最大子弹的1/5时，颜色变为红色
        /// </summary>
        public void UpdateCurrentAmmo()
        {
            int cur = _context.CombatVM.PlayerGun.gunProperty.CurrentAmmo;
            int max = _context.CombatVM.PlayerGun.gunProperty.MaxAmmo;
            CurrentAmmoText.text = cur.ToString();
            if (max != 0 && max > cur * 5)
            {
                CurrentAmmoText.color = AmmoWarning;
            }
            else
            {
                CurrentAmmoText.color = AmmoNormal;
            }
        }

        public override void TweenQuit(float duration)
        {
            var rect = GetComponent<RectTransform>();
            rect.DOAnchorPos(rect.anchoredPosition + new Vector2(rect.rect.width, 0), duration)
                .OnComplete(() => base.TweenQuit(duration));
        }
    }
}
