using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.CombatLogic.UILogic
{
    public class PlayerUI : SubUIBase
    {
        public TextMeshProUGUI CharacterName;
        public RawImage Skill1Icon;
        public RawImage Skill2Icon;
        public Image Skill1Mask;
        public Image Skill2Mask;

        public Slider HPSlider;

        public RawImage CockpitShow;

        private CockpitManager _cockpitManager;
        private void Awake()
        {
            CharacterName.text = _context.CombatVM.Player.OpInfo.Name;
            if (_context.CombatVM.Player.CombatSkillList.Count >= 1)
            {
                Skill1Icon.texture = ResourceManager.Load<Texture2D>($"Skills/{_context.CombatVM.Player.CombatSkillList[0].SkillInfo.IconUrl}");
                Skill1Mask.sprite = ResourceManager.Load<Sprite>($"Skills/{_context.CombatVM.Player.CombatSkillList[0].SkillInfo.IconUrl}");
            }
            else
            {
                Skill1Icon.gameObject.SetActive(false);
                Skill1Mask.gameObject.SetActive(false);
            }

            if (_context.CombatVM.Player.CombatSkillList.Count >= 2)
            {
                Skill2Icon.texture = ResourceManager.Load<Texture2D>($"Skills/{_context.CombatVM.Player.CombatSkillList[1].SkillInfo.IconUrl}");
                Skill2Mask.sprite = ResourceManager.Load<Sprite>($"Skills/{_context.CombatVM.Player.CombatSkillList[1].SkillInfo.IconUrl}");
            }
            else
            {
                Skill2Icon.gameObject.SetActive(false);
                Skill2Mask.gameObject.SetActive(false);
            }


        }
        private void Start()
        {
            _cockpitManager = CockpitManager.Instance;
        }

        private void OnGUI()
        {
            if (Skill1Mask.gameObject.activeSelf) Skill1Mask.fillAmount = getCoolDownRatio(0, Time.time);
            if (Skill2Mask.gameObject.activeSelf) Skill2Mask.fillAmount = getCoolDownRatio(1, Time.time);

            HPSlider.value = (float)_context.Operators[_context.PlayerTrans].CurrentHP / _context.Operators[_context.PlayerTrans].MaxHP;

            if (_context.Operators[_context.PlayerTrans].Speed <= 1)
            {
                _cockpitManager.ChangeState(CockpitWalkState.Idle);
            }
            else if (_context.Operators[_context.PlayerTrans].Speed <= 4)
            {
                _cockpitManager.ChangeState(CockpitWalkState.Walking);
            }
            else
            {
                _cockpitManager.ChangeState(CockpitWalkState.Running);
            }
        }
        /// <summary>
        /// 获得UI显示用的冷却比值
        /// </summary>
        private float getCoolDownRatio(int index, float time)
        {
            if (_context.CombatVM.Player.CombatSkillList[index].IsCoolDowning(time))
            {
                return (_context.CombatVM.Player.CombatSkillList[index].CoolDownEndTime - time) / _context.CombatVM.Player.CombatSkillList[index].SkillInfo.CoolDown;
            }
            else
            {
                return 0f;
            }
        }

        private Vector2 initPos;
        public override void TweenQuit(float duration)
        {
            
            var rect = GetComponent<RectTransform>();
            initPos = rect.anchoredPosition;
            rect.DOAnchorPos(rect.anchoredPosition - new Vector2(rect.rect.width, 0), duration)
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
