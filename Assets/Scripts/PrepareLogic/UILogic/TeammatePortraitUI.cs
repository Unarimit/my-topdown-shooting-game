using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic.UILogic
{
    public enum TeammatePortraitPage
    {
        ChoosePage,
        EditPage
    }
    /// <summary>
    /// ½ÐPortraitsScrollView
    /// </summary>
    public class TeammatePortraitUI : PrepareUIBase
    {

        public GameObject CharacterPortraitPrefab;
        public Transform PortraitsContentTrans;
        private PrepareContextManager _context => PrepareContextManager.Instance;
        private List<CharacterPortraitUI> _characterPortraits;
        void Start()
        {
            generatePortrait();
        }
        private void generatePortrait()
        {
            var ops = _context.data;
            _characterPortraits = new List<CharacterPortraitUI>();
            foreach (var op in ops)
            {
                var go = Instantiate(CharacterPortraitPrefab, PortraitsContentTrans);

                // set content
                var cp = go.GetComponent<CharacterPortraitUI>();
                cp.Inject(op, TeammatePortraitPage.ChoosePage);

                _characterPortraits.Add(cp);
            }

        }
        public void ChangePage(TeammatePortraitPage page)
        {
            foreach (var cp in _characterPortraits)
            {
                cp.ChangePage(page);
            }
        }
        private void ClearChooseState()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

    }

}
