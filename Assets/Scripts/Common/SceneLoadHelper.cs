using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Common
{
    internal class SceneLoadHelper : MonoBehaviour
    {
        /// <summary>
        /// 异步加载场景，会创建过渡效果，不要重复创建了
        /// </summary>
        public static void MyLoadSceneAsync(string sceneName)
        {
            GameObject go = new GameObject("MyLoadSceneAsync");
            var comp = go.AddComponent<SceneLoadHelper>();
            comp.LoadSceneAsync(sceneName);
        }

        /// <summary>
        /// 为了防止协程切换场景被销毁，这样处理
        /// </summary>
        public void LoadSceneAsync(string sceneName)
        {
            DontDestroyOnLoad(gameObject);
            StartCoroutine(LoadSceneAsyncCore(sceneName));
        }

        IEnumerator LoadSceneAsyncCore(string sceneName)
        {
            DOTween.Clear();
            var slider = SlideUI.CreateSlideUI(true); // 创建黑屏
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
            while (task.isDone is not true)
            {
                yield return null;
            }
            slider.End();
            Destroy(gameObject);
        }
    }
}
