using Assets.Scripts.PrepareLogic.PrepareEntities;
using Assets.Scripts.PrepareLogic.UILogic.TeammateUIs.CharacterEditor;
using Assets.Scripts.PrepareLogic.UILogic.TeammateUIs.CharacterSelector;
using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic.UILogic.TeammateUIs
{
    public class TeammateUI : PrepareUIBase
    {
        public CharacterEditorUI m_CharacterEditorUI;
        public CharacterPortraitScrollViewUI m_TeammatePortraitUI;

        private void Start()
        {
            m_TeammatePortraitUI.Inject(this);
            m_TeammatePortraitUI.GeneratePortrait();
        }

        public void ShowEditCharacter(PrepareOperator model)
        {
            m_CharacterEditorUI.ChooseCharacter(model);
        }


        public override void Refresh()
        {
            StartCoroutine(RefreshAsync());
        }

        public IEnumerator RefreshAsync()
        {
            if (UIManager.Instance.Page == TeammatePortraitPage.ChoosePage)
            {
                m_TeammatePortraitUI.gameObject.SetActive(false);
                yield return m_CharacterEditorUI.QuitAsync();

                ((RectTransform)transform).DOSizeDelta(new Vector2(1300, 780f), 1);
                yield return new WaitForSeconds(1);

                m_TeammatePortraitUI.ChangePage(UIManager.Instance.Page);
                m_TeammatePortraitUI.gameObject.SetActive(true);
            }
            else if (UIManager.Instance.Page == TeammatePortraitPage.EditPage)
            {
                m_TeammatePortraitUI.gameObject.SetActive(false);

                ((RectTransform)transform).DOSizeDelta(new Vector2(500, 780f), 1);
                yield return new WaitForSeconds(1);

                m_CharacterEditorUI.Enter();
                m_TeammatePortraitUI.ChangePage(UIManager.Instance.Page);
                m_TeammatePortraitUI.gameObject.SetActive(true);
            }
        }
    }
}
