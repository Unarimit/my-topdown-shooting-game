using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic.ActionUIs
{
    internal class ActionUI : MonoBehaviour
    {
        private HomeContextManager _context = HomeContextManager.Instance;
        public void Enter()
        {
            ((RectTransform)transform).sizeDelta = new Vector2(800, 0);
            gameObject.SetActive(true);
            ((RectTransform)transform).DOSizeDelta(new Vector2(800, 600), 0.5f);
        }
        private void Start()
        {
            transform.Find("Scroll View").GetComponent<ActionScrollViewUI>().Inject(_context.GetLevelRules());
        }
    }
}
