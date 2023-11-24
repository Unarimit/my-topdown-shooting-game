using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Common
{
    internal class TipsUI : MonoBehaviour
    {
        public TextMeshProUGUI Text;
        public static void GenerateNewTips(string text)
        {
            var prefab = ResourceManager.Load<GameObject>("UIs/Tip");
            var go = Instantiate(prefab);
            go.GetComponent<TipsUI>().Text.text = text;
        }

        private void Start()
        {
            var cg = GetComponent<CanvasGroup>();
            cg.alpha = 0;
            cg.DOFade(1, 0.2f); // 渐入动画
            Destroy(gameObject, 1.5f); // 持续1.5秒后销毁
        }
    }
}
