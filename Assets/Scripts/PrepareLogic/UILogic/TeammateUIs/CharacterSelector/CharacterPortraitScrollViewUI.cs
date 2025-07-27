using Assets.Scripts.PrepareLogic.PrepareEntities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic.UILogic.TeammateUIs.CharacterSelector
{
    /// <summary>
    /// ����prepareOprators, hierarchy���PortraitsScrollView
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
        /// ���ɿ�ѡ��
        /// </summary>
        public void GeneratePortrait()
        {
            var ops = _context.PrepareOps;
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
        /// ������༭ҳ���ѡ��ҳ��
        /// </summary>
        /// <param name="page"></param>
        public void ChangePage(TeammatePortraitPage page)
        {
            foreach (var cp in _characterPortraits)
            {
                cp.ChangePage(page);
            }
            if(page == TeammatePortraitPage.EditPage) // Ĭ��ѡ�е�һ����ɫ
            {
                InEditSelect(_characterPortraits[0], _context.PrepareOps[0]);
                _characterPortraits[0].SetChoose(true);
            }
        }

        /// <summary>
        /// ���������ʾ�Լ�ǰ����
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
