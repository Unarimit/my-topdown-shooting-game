using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

public class ShipInfoOverviewPanelUI : MonoBehaviour
{
    [SerializeField]
    Button hideButton;
    [SerializeField]
    Button showButtonBg;

    private RectTransform m_RectTransform;
    private Vector2 m_InitSize;
    private readonly Vector2 m_SmallSize = new Vector2(150, 100);
    private bool _isSmall = false;
    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_InitSize = m_RectTransform.sizeDelta;
        
        hideButton.transform.AddComponent<CanvasGroup>();
        hideButton.onClick.AddListener(TweenScaleSmall);
        showButtonBg.onClick.AddListener(TweenScaleBig);
    }

    private void SetBtnActive()
    {
        var cg = hideButton.transform.GetComponent<CanvasGroup>();
        cg.alpha = _isSmall ? 0 : 1;
        cg.blocksRaycasts = !_isSmall;
    }

    private void TweenScaleSmall()
    {
        if (_isSmall is true) return;
        _isSmall = true;
        m_RectTransform.DOSizeDelta(m_SmallSize, 0.3f).SetEase(Ease.InSine);
        SetBtnActive();
    }

    private void TweenScaleBig()
    {
        if (_isSmall is false) return;
        _isSmall = false;
        m_RectTransform.DOSizeDelta(m_InitSize, 0.3f).SetEase(Ease.InSine).OnComplete(SetBtnActive);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
