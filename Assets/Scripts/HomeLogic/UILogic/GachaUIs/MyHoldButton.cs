using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.GachaUIs
{
    /// <summary>
    /// 长按按钮的实现
    /// </summary>
    internal class MyHoldButton : Button
    {
        public Slider Slider { get; set; }

        // config
        float shakeAmount = 20f;
        float shakeSpeed = 10f;
        float pressTime = 2f;

        // inner flag
        Vector3 originalPosition;
        bool isShaking = false;

        public delegate void OnHoldButtonFinish();
        public event OnHoldButtonFinish OnHoldButtonFinishEvent;

        public delegate bool OnHoldButtonPress();
        public event OnHoldButtonPress OnHoldButtonPressEvent;

        protected override void Start()
        {
            base.Start();
            originalPosition = transform.localPosition;
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (OnHoldButtonPressEvent.Invoke() is false) return;
            isShaking = true;
            StartCoroutine(ShakeCoroutine());
        }


        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            isShaking = false;
        }
        /// <summary>
        /// 按钮抖动效果，可以配合Slider显示进度
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShakeCoroutine()
        {
            float time = 0;
            if (Slider != null) Slider.gameObject.SetActive(true);
            while (isShaking)
            {
                if (Slider != null) Slider.value = time / pressTime;
                time += Time.deltaTime;
                if (time >= pressTime)
                {
                    isShaking = false;
                    OnHoldButtonFinishEvent.Invoke();
                    break;
                }
                float offsetX = Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) * shakeAmount - (shakeAmount / 2f);
                float offsetY = Mathf.PerlinNoise(0f, Time.time * shakeSpeed) * shakeAmount - (shakeAmount / 2f);

                transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

                yield return null;
            }
            if (Slider != null) Slider.gameObject.SetActive(false);
            transform.localPosition = originalPosition;
        }
    }
}
