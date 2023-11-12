using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic.UILogic.EnemyUIs
{
    public class EnemyUI : PrepareUIBase
    {
        public override void Refresh()
        {
            StartCoroutine(refreshAsync());
        }

        private IEnumerator refreshAsync()
        {
            if (UIManager.Instance.Page == TeammatePortraitPage.ChoosePage)
            {
                yield return new WaitForSeconds(0.2f); // wait teammate ui quit
                ((RectTransform)transform).DOSizeDelta(new Vector2(618, 780f), 1);
            }
            else
            {
                ((RectTransform)transform).DOSizeDelta(new Vector2(0, 780f), 1);
            }
        }
    }
}
