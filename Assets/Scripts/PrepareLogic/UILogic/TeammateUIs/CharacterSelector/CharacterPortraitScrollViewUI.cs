using Assets.Scripts.PrepareLogic.PrepareEntities;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic.UILogic.TeammateUIs.CharacterSelector
{
    /// <summary>
    /// 控制prepareOprators, hierarchy里叫PortraitsScrollView
    /// </summary>
    public class CharacterPortraitScrollViewUI : MonoBehaviour
    {

        public GameObject CharacterPortraitPrefab;
        public Transform PortraitsContentTrans;
        private PrepareContextManager _context => PrepareContextManager.Instance;
        private List<CharacterPortraitScrollViewItemUI> _characterPortraits;
        private TeammateUI _teammateUI;
        public void Inject(TeammateUI teammateUI)
        {
            _teammateUI = teammateUI;
        }
        /// <summary>
        /// 生成可选项
        /// </summary>
        public void GeneratePortrait()
        {
            var ops = _context.data;
            _characterPortraits = new List<CharacterPortraitScrollViewItemUI>();
            foreach (var op in ops)
            {
                var go = Instantiate(CharacterPortraitPrefab, PortraitsContentTrans);

                // set content
                var cp = go.GetComponent<CharacterPortraitScrollViewItemUI>();
                cp.Inject(op, TeammatePortraitPage.ChoosePage, this);

                _characterPortraits.Add(cp);
            }
        }
        /// <summary>
        /// 变更到编辑页面或选择页面
        /// </summary>
        /// <param name="page"></param>
        public void ChangePage(TeammatePortraitPage page)
        {
            foreach (var cp in _characterPortraits)
            {
                cp.ChangePage(page);
            }
            if(page == TeammatePortraitPage.EditPage) // 默认选中第一个角色
            {
                InEditSelect(_characterPortraits[0], _context.data[0]);
                _characterPortraits[0].SetChoose(true);
            }
        }

        /// <summary>
        /// 子组件在显示自己前调用
        /// </summary>
        public void InEditSelect(CharacterPortraitScrollViewItemUI portraitUI, PrepareOperator model)
        {
            foreach (var cp in _characterPortraits)
            {
                if(cp != portraitUI) cp.SetChoose(false);
            }
            _teammateUI.ShowEditCharacter(model);
        }

    }

}
