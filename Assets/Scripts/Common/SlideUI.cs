using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Common
{
    /// <summary>
    /// 加载时的slide ui
    /// </summary>
    internal class SlideUI : MonoBehaviour
    {
        public static SlideUI CreateSlideUI()
        {
            Time.timeScale = 1;
            var prefab = ResourceManager.Load<GameObject>("UIs/SlideCanvas");
            var go = Instantiate(prefab);
            DontDestroyOnLoad(go);
            return go.AddComponent<SlideUI>();
        }
        RectTransform _panelRect;
        public bool IsFinish { get; private set; } = false;
        private void Start()
        {
            _panelRect = transform.Find("Panel").GetComponent<RectTransform>();
            var cg = GetComponent<CanvasGroup>();
            cg.alpha = 0;
            cg.DOFade(1, 0.2f).OnComplete(() =>
            {
                IsFinish = true;
                End();
            });
        }

        /// <summary>
        /// TODO: 传入异步加载的参数
        /// </summary>
        private void End()
        {
            _panelRect.DOSizeDelta(new Vector2(0, _panelRect.rect.height), 0.3f).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}
