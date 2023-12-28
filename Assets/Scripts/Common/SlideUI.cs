using DG.Tweening;
using System;
using System.Collections;
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
        public static SlideUI CreateSlideUI(bool releaseBySelf = false)
        {
            Time.timeScale = 1;
            var prefab = ResourceManager.Load<GameObject>("UIs/SlideCanvas");
            var go = Instantiate(prefab);
            DontDestroyOnLoad(go);
            var res = go.AddComponent<SlideUI>();
            res.ReleaseBySelf = releaseBySelf;
            return res;
        }
        RectTransform _panelRect;
        /// <summary>
        /// 是否完全遮挡屏幕
        /// </summary>
        public bool IsFinish { get; private set; } = false;
        public bool ReleaseBySelf { get; private set; } = false;
        private void Start()
        {
            _panelRect = transform.Find("Panel").GetComponent<RectTransform>();
            var cg = GetComponent<CanvasGroup>();
            cg.alpha = 0;
            cg.DOFade(1, 0.2f).OnComplete(() =>
            {
                IsFinish = true;
                if(ReleaseBySelf is false) End();
            });
        }

        /// <summary>
        /// TODO: 传入异步加载的参数
        /// </summary>
        public void End()
        {
            _panelRect.DOSizeDelta(new Vector2(0, _panelRect.rect.height), 0.3f).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}
