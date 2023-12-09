using System.Collections;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Common
{
    internal class SceneLoadHelper
    {
        public static IEnumerator MyLoadSceneAsync(string sceneName)
        {
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
            task.allowSceneActivation = true;
        }
    }
}
