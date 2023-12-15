using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic
{
    internal class HomeUIBase : MonoBehaviour
    {
        HomeContextManager _context => HomeContextManager.Instance;

        public virtual void Enter()
        {
            gameObject.SetActive(true);
        }

        public virtual void Quit()
        {
            gameObject.SetActive(false);
        }

        public virtual void Refresh()
        {
            // do nothing
        }
    }
}
