using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic
{
    internal class ActionUI : MonoBehaviour
    {
        public void Enter()
        {
            ((RectTransform)transform).sizeDelta = new Vector2(800, 0);
            gameObject.SetActive(true);
            ((RectTransform)transform).DOSizeDelta(new Vector2(800, 600), 0.5f);
        }
    }
}
