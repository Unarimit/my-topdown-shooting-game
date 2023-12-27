using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic
{
    internal class HomeUIBase : MonoBehaviour
    {
        protected HomeContextManager _context => HomeContextManager.Instance;
        protected UIManager _rootUI => UIManager.Instance;

        CanvasGroup cg;
        /// <summary>
        /// 小心重写
        /// </summary>
        protected virtual void Awake()
        {
            cg = GetComponent<CanvasGroup>();
        }
        public virtual void Enter()
        {
            if (cg != null) cg.alpha = 1;
            gameObject.SetActive(true);
        }

        public virtual void Quit()
        {
            if (cg != null)
            {
                cg.DOFade(0, 0.5f).OnComplete(() => gameObject.SetActive(false));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public virtual void Refresh()
        {
            // do nothing
        }
    }
}
