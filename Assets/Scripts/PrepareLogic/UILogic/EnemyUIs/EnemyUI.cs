using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic.UILogic.EnemyUIs
{
    public class EnemyUI : PrepareUIBase
    {
        private EnemyPortraitScrollViewUI _enemyScrollView;
        public void Start()
        {
            _enemyScrollView = transform.Find("PortraitsScrollView").GetComponent<EnemyPortraitScrollViewUI>();
            _enemyScrollView.GeneratePortrait();
        }

        public override void Refresh()
        {
            StartCoroutine(refreshAsync());
        }

        private IEnumerator refreshAsync()
        {
            if (UIManager.Instance.Page == TeammatePortraitPage.ChoosePage)
            {
                yield return new WaitForSeconds(0.2f); // wait teammate ui quit
                ((RectTransform)transform).DOAnchorPos(new Vector2(0, -780f), 1);
            }
            else
            {
                ((RectTransform)transform).DOAnchorPos(new Vector2(618, -780f), 1);
            }
        }
    }
}
