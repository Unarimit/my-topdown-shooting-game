using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

namespace Assets.Scripts.CombatLogic.UILogic
{
    public class CursorDrawUI : SubUIBase
    {
        RectTransform cursorSideTransform1;
        RectTransform cursorSideTransform2;
        RectTransform cursorSideTransform3;
        RectTransform cursorSideTransform4;
        private void Awake()
        {
            cursorSideTransform1 = (RectTransform)transform.Find("CorsurCenterSide1");
            cursorSideTransform2 = (RectTransform)transform.Find("CorsurCenterSide2");
            cursorSideTransform3 = (RectTransform)transform.Find("CorsurCenterSide3");
            cursorSideTransform4 = (RectTransform)transform.Find("CorsurCenterSide4");

            beginPos1 = cursorSideTransform1.anchoredPosition;
            beginPos2 = cursorSideTransform2.anchoredPosition;
            beginPos3 = cursorSideTransform3.anchoredPosition;
            beginPos4 = cursorSideTransform4.anchoredPosition;
        }
        private void OnEnable()
        {
            Cursor.visible = false;
            CombatContextManager.Instance.CombatVM.IsPlayerAimmingEvent += Aim;
        }
        private void OnGUI()
        {
            transform.position = Mouse.current.position.ReadValue();
        }
        private void OnDisable()
        {
            Cursor.visible = true;
            CombatContextManager.Instance.CombatVM.IsPlayerAimmingEvent -= Aim;
        }
        Vector2 startPos = new Vector2(0, 0);
        Vector2 beginPos1;
        Vector2 beginPos2;
        Vector2 beginPos3;
        Vector2 beginPos4;
        float time = 0.5f;
        private void Aim(bool aim)
        {
            if (aim)
            {
                cursorSideTransform1.DOAnchorPos(startPos, time);
                cursorSideTransform2.DOAnchorPos(startPos, time);
                cursorSideTransform3.DOAnchorPos(startPos, time);
                cursorSideTransform4.DOAnchorPos(startPos, time);
            }
            else
            {
                cursorSideTransform1.DOAnchorPos(beginPos1, time);
                cursorSideTransform2.DOAnchorPos(beginPos2, time);
                cursorSideTransform3.DOAnchorPos(beginPos3, time);
                cursorSideTransform4.DOAnchorPos(beginPos4, time);
            }
        }
    }
}
