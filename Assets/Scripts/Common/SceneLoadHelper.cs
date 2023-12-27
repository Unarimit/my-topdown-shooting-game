using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Common
{
    internal class SceneLoadHelper
    {
        public static IEnumerator MyLoadSceneAsync(string sceneName)
        {
            DOTween.Clear();
            var slider = SlideUI.CreateSlideUI();
            var task = SceneManager.LoadSceneAsync(sceneName);
            task.allowSceneActivation = false;
            while (task.progress < 0.9f)
            {
                yield return null;
            }
            while (slider.IsFinish is not true)
            {
                yield return null;
            }
            // 如果非要卡主线程，那就在黑屏的时候再卡吧
            task.allowSceneActivation = true;
        }
    }
}
