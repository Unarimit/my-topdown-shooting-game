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
            var prefab = ResourceManager.Load<GameObject>("UIs/SlideCanvas");
            var go = Instantiate(prefab);
            DontDestroyOnLoad(go);
            return go.AddComponent<SlideUI>();
        }
        private void Start()
        {
            var pt = transform.Find("Panel").GetComponent<RectTransform>();
            pt.DOSizeDelta(new Vector2(0, pt.rect.height), 0.5f).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}
