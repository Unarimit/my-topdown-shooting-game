﻿using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic.UILogic
{
    public class CharacterEditorUI : PrepareUIBase
    {
        public RawImage m_RawImage;
        public RectTransform m_InfoPanelTrans;

        public override void Enter()
        {
            base.Enter();
            m_InfoPanelTrans.DOSizeDelta(new Vector2(530, 780), 0.5f);
            m_RawImage.color = new Color(1, 1, 1, 0);
            m_RawImage.DOFade(1, 0.5f);
        }

        public override void Quit()
        {
            StartCoroutine(QuitAsync());
        }

        IEnumerator QuitAsync()
        {
            m_RawImage.DOFade(0, 0.2f);
            m_InfoPanelTrans.DOSizeDelta(new Vector2(530, 0), 0.2f);
            yield return new WaitForSeconds(0.2f);
            base.Quit();

        }
    }
}
