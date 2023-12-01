using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CombatLogic.UILogic.CombatSummaryUIs
{
    /// <summary>
    /// 结算界面的资源项变动UI，主要是动画显示
    /// </summary>
    internal class MainResourceScrollViewItemUI : MonoBehaviour
    {
        // WIP，inspector中使用
        public string ItemName;
        public int LastValue;
        public int DeltaValue;

        private TextMeshProUGUI _itemNameTMP;
        private TextMeshProUGUI _itemValueTMP;
        private TextMeshProUGUI _itemDeltaValueTMP;
        private RawImage _upDeltaMarkRawImg;
        private RawImage _downDeltaMarkRawImg;

        private void Awake()
        {
            _itemNameTMP = transform.Find("ItemNameTextTMP").GetComponent<TextMeshProUGUI>();
            _itemValueTMP = transform.Find("ItemValTextTMP").GetComponent<TextMeshProUGUI>();
            _itemDeltaValueTMP = transform.Find("ItemDeltaTextTMP").GetComponent<TextMeshProUGUI>();
            _upDeltaMarkRawImg = transform.Find("UpDeltaMarkRawImage").GetComponent<RawImage>();
            _downDeltaMarkRawImg = transform.Find("DownDeltaMarkRawImage").GetComponent<RawImage>();


            _itemNameTMP.gameObject.SetActive(false);
            _itemValueTMP.gameObject.SetActive(false);
            _itemDeltaValueTMP.gameObject.SetActive(false);
            _downDeltaMarkRawImg.gameObject.SetActive(false);
            _upDeltaMarkRawImg.gameObject.SetActive(false);

            // TODO: 修改 WIP logic
            {
                DOVirtual.DelayedCall(1.2f, () =>
                {
                    Inject();
                });
                
            }
            
        }

        private void Inject()
        {
            tweenThis();
        }

        private void tweenThis()
        {
            // 显示item和初始值
            _itemNameTMP.gameObject.SetActive(true);
            _itemNameTMP.text = ItemName;
            _itemValueTMP.gameObject.SetActive(true);
            _itemValueTMP.text = LastValue.ToString();

            // 开始变动，0.5秒完成变动
            DOVirtual.DelayedCall(0.5f, () =>
            {
                showDelta();
                DOTween.To(() => LastValue, x => _itemValueTMP.text = x.ToString(), LastValue + DeltaValue, 0.5f);
            });
            

            
        }
        /// <summary>
        /// 显示变动内容（delta
        /// </summary>
        private void showDelta()
        {
            if (DeltaValue > 0)
            {
                _upDeltaMarkRawImg.gameObject.SetActive(true);
                _itemDeltaValueTMP.gameObject.SetActive(true);
                _itemDeltaValueTMP.text = DeltaValue.ToString();
                _itemDeltaValueTMP.color = _upDeltaMarkRawImg.color;
            }
            else if (DeltaValue < 0)
            {
                _downDeltaMarkRawImg.gameObject.SetActive(true);
                _itemDeltaValueTMP.gameObject.SetActive(true);
                _itemDeltaValueTMP.text = (-DeltaValue).ToString();
                _itemDeltaValueTMP.color = _downDeltaMarkRawImg.color;
            }
        }
    }
}
